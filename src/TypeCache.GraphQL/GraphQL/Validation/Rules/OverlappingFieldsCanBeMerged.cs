using GraphQL.Types;
using GraphQL.Validation.Errors;
using GraphQLParser;
using GraphQLParser.AST;
using TypeCache.GraphQL.Types;

namespace GraphQL.Validation.Rules;

/// <summary>
/// Overlapping fields are mergable:
///
/// If multiple field selections with the same response names are encountered during execution,
/// the field and arguments to execute and the resulting value should be unambiguous. Therefore
/// any two field selections which might both be encountered for the same object are only valid
/// if they are equivalent.
/// </summary>
public class OverlappingFieldsCanBeMerged : IValidationRule
{
	/// <inheritdoc/>
	/// <exception cref="OverlappingFieldsCanBeMergedError"/>
	public ValueTask<INodeVisitor?> ValidateAsync(ValidationContext context)
	{
		//TODO: make static instance when enabling this rule
		var comparedFragmentPairs = new PairSet();
		var cachedFieldsAndFragmentNames = new Dictionary<GraphQLSelectionSet, CachedField>();

		return new ValueTask<INodeVisitor?>(new MatchingNodeVisitor<GraphQLSelectionSet>((selectionSet, context) => //TODO:allocation of Action<GraphQLSelectionSet,ValidationContext>
		{
			var conflicts = FindConflictsWithinSelectionSet(
				context,
				cachedFieldsAndFragmentNames,
				comparedFragmentPairs,
				context.TypeInfo.GetParentType(),
				selectionSet);

			if (conflicts.Count > 0)
			{
				foreach (var conflict in conflicts)
				{
					context.ReportError(new OverlappingFieldsCanBeMergedError(context, conflict));
				}
			}
		}));
	}

	private static List<Conflict> FindConflictsWithinSelectionSet(
		ValidationContext context,
		Dictionary<GraphQLSelectionSet, CachedField> cachedFieldsAndFragmentNames,
		PairSet comparedFragmentPairs,
		IGraphType? parentType,
		GraphQLSelectionSet selectionSet)
	{
		var conflicts = new List<Conflict>();

		var cachedField = GetFieldsAndFragmentNames(
			context,
			cachedFieldsAndFragmentNames,
			parentType,
			selectionSet);

		var fieldMap = cachedField.NodeAndDef;
		var fragmentNames = cachedField.Names;

		CollectConflictsWithin(
			context,
			conflicts,
			cachedFieldsAndFragmentNames,
			comparedFragmentPairs,
			fieldMap);

		if (fragmentNames.Count > 0)
		{
			// (B) Then collect conflicts between these fields and those represented by
			// each spread fragment name found.
			var comparedFragments = new HashSet<ROM>();
			for (int i = 0; i < fragmentNames.Count; ++i)
			{
				CollectConflictsBetweenFieldsAndFragment(
					context,
					conflicts,
					cachedFieldsAndFragmentNames,
					comparedFragments,
					comparedFragmentPairs,
					false,
					fieldMap,
					fragmentNames[i]);

				// (C) Then compare this fragment with all other fragments found in this
				// selection set to collect conflicts between fragments spread together.
				// This compares each item in the list of fragment names to every other
				// item in that same list (except for itself).
				for (int j = i + 1; j < fragmentNames.Count; ++j)
				{
					CollectConflictsBetweenFragments(
						context,
						conflicts,
						cachedFieldsAndFragmentNames,
						comparedFragmentPairs,
						false,
						fragmentNames[i],
						fragmentNames[j]);
				}
			}
		}
		return conflicts;
	}

	private static void CollectConflictsWithin(
		ValidationContext context,
		List<Conflict> conflicts,
		Dictionary<GraphQLSelectionSet, CachedField> cachedFieldsAndFragmentNames,
		PairSet comparedFragmentPairs,
		Dictionary<ROM, List<FieldDefPair>> fieldMap)
	{
		// A field map is a keyed collection, where each key represents a response
		// name and the value at that key is a list of all fields which provide that
		// response name. For every response name, if there are multiple fields, they
		// must be compared to find a potential conflict.
		foreach (var entry in fieldMap)
		{
			var responseName = entry.Key;
			var fields = entry.Value;

			// This compares every field in the list to every other field in this list
			// (except to itself). If the list only has one item, nothing needs to
			// be compared.
			if (fields.Count > 1)
			{
				for (int i = 0; i < fields.Count; ++i)
				{
					for (int j = i + 1; j < fields.Count; ++j)
					{
						var conflict = FindConflict(
							context,
							cachedFieldsAndFragmentNames,
							comparedFragmentPairs,
							false, // within one collection is never mutually exclusive
							responseName,
							fields[i],
							fields[j]);

						if (conflict is not null)
							conflicts.Add(conflict);
					}
				}
			}

		}
	}

	private static Conflict? FindConflict(
		ValidationContext context,
		Dictionary<GraphQLSelectionSet, CachedField> cachedFieldsAndFragmentNames,
		PairSet comparedFragmentPairs,
		bool parentFieldsAreMutuallyExclusive,
		ROM responseName,
		FieldDefPair fieldDefPair1,
		FieldDefPair fieldDefPair2)
	{
		var parentType1 = fieldDefPair1.ParentType;
		var node1 = fieldDefPair1.Field;
		var def1 = fieldDefPair1.FieldDef;

		var parentType2 = fieldDefPair2.ParentType;
		var node2 = fieldDefPair2.Field;
		var def2 = fieldDefPair2.FieldDef;

		// If it is known that two fields could not possibly apply at the same
		// time, due to the parent types, then it is safe to permit them to diverge
		// in aliased field or arguments used as they will not present any ambiguity
		// by differing.
		// It is known that two parent types could never overlap if they are
		// different Object types. Interface or Union types might overlap - if not
		// in the current state of the schema, then perhaps in some future version,
		// thus may not safely diverge.

		bool areMutuallyExclusive = parentFieldsAreMutuallyExclusive
			|| (parentType1 != parentType2 && isObjectType(parentType1) && isObjectType(parentType2));

		// return type for each field.
		var type1 = def1?.ResolvedType;
		var type2 = def2?.ResolvedType;

		if (!areMutuallyExclusive)
		{
			// Two aliases must refer to the same field.
			var name1 = node1.GetName();
			var name2 = node2.GetName();

			if (name1 != name2)
			{
				return new Conflict
				{
					Reason = new ConflictReason
					{
						Name = responseName.Span.ToString(), //ISSUE:allocation
						Message = new Message
						{
							Msg = $"{name1} and {name2} are different fields"
						}
					},
					FieldsLeft = new List<ISelectionNode> { node1 },
					FieldsRight = new List<ISelectionNode> { node2 }
				};
			}

			// Two field calls must have the same arguments.
			if (!SameArguments(node1.GetArguments(), node2.GetArguments()))
			{
				return new Conflict
				{
					Reason = new ConflictReason
					{
						Name = responseName.Span.ToString(), //ISSUE:allocation
						Message = new Message
						{
							Msg = "they have differing arguments"
						}
					},
					FieldsLeft = new List<ISelectionNode> { node1 },
					FieldsRight = new List<ISelectionNode> { node2 }
				};
			}
		}

		if (type1 is not null && type2 is not null && DoTypesConflict(type1, type2))
		{
			return new Conflict
			{
				Reason = new ConflictReason
				{
					Name = responseName.Span.ToString(), //ISSUE:allocation
					Message = new Message
					{
						Msg = $"they return conflicting types {type1} and {type2}"
					}
				},
				FieldsLeft = new List<ISelectionNode> { node1 },
				FieldsRight = new List<ISelectionNode> { node2 }
			};
		}

		// Collect and compare sub-fields. Use the same "visited fragment names" list
		// for both collections so fields in a fragment reference are never
		// compared to themselves.
		var selectionSet1 = node1.GetSelectionSet();
		var selectionSet2 = node2.GetSelectionSet();

		if (selectionSet1 is not null && selectionSet2 is not null)
		{
			var conflicts = FindConflictsBetweenSubSelectionSets(
				context,
				cachedFieldsAndFragmentNames,
				comparedFragmentPairs,
				areMutuallyExclusive,
				type1?.GetNamedGraphType(),
				selectionSet1,
				type2?.GetNamedGraphType(),
				selectionSet2);

			return SubfieldConflicts(conflicts, responseName, node1, node2);
		}

		return null;
	}

	private static List<Conflict> FindConflictsBetweenSubSelectionSets(
		ValidationContext context,
		Dictionary<GraphQLSelectionSet, CachedField> cachedFieldsAndFragmentNames,
		PairSet comparedFragmentPairs,
		bool areMutuallyExclusive,
		IGraphType? parentType1,
		GraphQLSelectionSet selectionSet1,
		IGraphType? parentType2,
		GraphQLSelectionSet selectionSet2)
	{
		var conflicts = new List<Conflict>();

		var cachedField1 = GetFieldsAndFragmentNames(
			context,
			cachedFieldsAndFragmentNames,
			parentType1,
			selectionSet1);

		var fieldMap1 = cachedField1.NodeAndDef;
		var fragmentNames1 = cachedField1.Names;

		var cachedField2 = GetFieldsAndFragmentNames(
			context,
			cachedFieldsAndFragmentNames,
			parentType2,
			selectionSet2);

		var fieldMap2 = cachedField2.NodeAndDef;
		var fragmentNames2 = cachedField2.Names;

		// (H) First, collect all conflicts between these two collections of field.
		CollectConflictsBetween(
			context,
			conflicts,
			cachedFieldsAndFragmentNames,
			comparedFragmentPairs,
			areMutuallyExclusive,
			fieldMap1,
			fieldMap2);

		// (I) Then collect conflicts between the first collection of fields and
		// those referenced by each fragment name associated with the second.
		if (fragmentNames2.Count > 0)
		{
			var comparedFragments = new HashSet<ROM>();

			for (int j = 0; j < fragmentNames2.Count; j++)
			{
				CollectConflictsBetweenFieldsAndFragment(
					context,
					conflicts,
					cachedFieldsAndFragmentNames,
					comparedFragments,
					comparedFragmentPairs,
					areMutuallyExclusive,
					fieldMap1,
					fragmentNames2[j]);
			}
		}

		// (I) Then collect conflicts between the second collection of fields and
		// those referenced by each fragment name associated with the first.
		if (fragmentNames1.Count > 0)
		{
			var comparedFragments = new HashSet<ROM>();

			for (int i = 0; i < fragmentNames1.Count; i++)
			{
				CollectConflictsBetweenFieldsAndFragment(
					context,
					conflicts,
					cachedFieldsAndFragmentNames,
					comparedFragments,
					comparedFragmentPairs,
					areMutuallyExclusive,
					fieldMap2,
					fragmentNames1[i]);
			}
		}

		// (J) Also collect conflicts between any fragment names by the first and
		// fragment names by the second. This compares each item in the first set of
		// names to each item in the second set of names.
		for (int i = 0; i < fragmentNames1.Count; i++)
		{
			for (int j = 0; j < fragmentNames2.Count; j++)
			{
				CollectConflictsBetweenFragments(
					context,
					conflicts,
					cachedFieldsAndFragmentNames,
					comparedFragmentPairs,
					areMutuallyExclusive,
					fragmentNames1[i],
					fragmentNames2[j]);
			}
		}

		return conflicts;
	}

	private static void CollectConflictsBetweenFragments(
		ValidationContext context,
		List<Conflict> conflicts,
		Dictionary<GraphQLSelectionSet, CachedField> cachedFieldsAndFragmentNames,
		PairSet comparedFragmentPairs,
		bool areMutuallyExclusive,
		ROM fragmentName1,
		ROM fragmentName2)
	{
		// No need to compare a fragment to itself.
		if (fragmentName1 == fragmentName2)
			return;

		// Memoize so two fragments are not compared for conflicts more than once.
		if (comparedFragmentPairs.Has(fragmentName1, fragmentName2, areMutuallyExclusive))
			return;

		comparedFragmentPairs.Add(fragmentName1, fragmentName2, areMutuallyExclusive);

		var fragment1 = context.Document.FindFragmentDefinition(fragmentName1);
		var fragment2 = context.Document.FindFragmentDefinition(fragmentName2);

		if (fragment1 is null || fragment2 is null)
			return;

		var cachedField1 = GetReferencedFieldsAndFragmentNames(
			context,
			cachedFieldsAndFragmentNames,
			fragment1);

		var fieldMap1 = cachedField1.NodeAndDef;
		var fragmentNames1 = cachedField1.Names;

		var cachedField2 = GetReferencedFieldsAndFragmentNames(
			context,
			cachedFieldsAndFragmentNames,
			fragment2);

		var fieldMap2 = cachedField2.NodeAndDef;
		var fragmentNames2 = cachedField2.Names;

		// (F) First, collect all conflicts between these two collections of fields
		// (not including any nested fragments).
		CollectConflictsBetween(
			context,
			conflicts,
			cachedFieldsAndFragmentNames,
			comparedFragmentPairs,
			areMutuallyExclusive,
			fieldMap1,
			fieldMap2);

		// (G) Then collect conflicts between the first fragment and any nested
		// fragments spread in the second fragment.
		foreach (var fragmentName in fragmentNames2)
		{
			CollectConflictsBetweenFragments(
				context,
				conflicts,
				cachedFieldsAndFragmentNames,
				comparedFragmentPairs,
				areMutuallyExclusive,
				fragmentName1,
				fragmentName);
		}

		// (G) Then collect conflicts between the second fragment and any nested
		// fragments spread in the first fragment.
		foreach (var fragmentName in fragmentNames1)
		{
			CollectConflictsBetweenFragments(
				context,
				conflicts,
				cachedFieldsAndFragmentNames,
				comparedFragmentPairs,
				areMutuallyExclusive,
				fragmentName,
				fragmentName2);
		}
	}

	private static void CollectConflictsBetweenFieldsAndFragment(
		ValidationContext context,
		List<Conflict> conflicts,
		Dictionary<GraphQLSelectionSet, CachedField> cachedFieldsAndFragmentNames,
		HashSet<ROM> comparedFragments,
		PairSet comparedFragmentPairs,
		bool areMutuallyExclusive,
		Dictionary<ROM, List<FieldDefPair>> fieldMap,
		ROM fragmentName)
	{
		// Memoize so a fragment is not compared for conflicts more than once.
		if (!comparedFragments.Add(fragmentName))
			return;

		var fragment = context.Document.FindFragmentDefinition(fragmentName);
		if (fragment is null)
			return;

		var cachedField = GetReferencedFieldsAndFragmentNames(
			context,
			cachedFieldsAndFragmentNames,
			fragment);

		var fieldMap2 = cachedField.NodeAndDef;
		var fragmentNames2 = cachedField.Names;

		// Do not compare a fragment's fieldMap to itself.
		if (fieldMap == fieldMap2)
			return;

		// (D) First collect any conflicts between the provided collection of fields
		// and the collection of fields represented by the given fragment.
		CollectConflictsBetween(
			context,
			conflicts,
			cachedFieldsAndFragmentNames,
			comparedFragmentPairs,
			areMutuallyExclusive,
			fieldMap,
			fieldMap2);

		// (E) Then collect any conflicts between the provided collection of fields
		// and any fragment names found in the given fragment.
		for (int i = 0; i < fragmentNames2.Count; i++)
		{
			CollectConflictsBetweenFieldsAndFragment(
				context,
				conflicts,
				cachedFieldsAndFragmentNames,
				comparedFragments,
				comparedFragmentPairs,
				areMutuallyExclusive,
				fieldMap,
				fragmentNames2[i]);
		}
	}

	private static void CollectConflictsBetween(
		ValidationContext context,
		List<Conflict> conflicts,
		Dictionary<GraphQLSelectionSet, CachedField> cachedFieldsAndFragmentNames,
		PairSet comparedFragmentPairs,
		bool parentFieldsAreMutuallyExclusive,
		Dictionary<ROM, List<FieldDefPair>> fieldMap1,
		Dictionary<ROM, List<FieldDefPair>> fieldMap2)
	{
		// A field map is a keyed collection, where each key represents a response
		// name and the value at that key is a list of all fields which provide that
		// response name. For any response name which appears in both provided field
		// maps, each field from the first field map must be compared to every field
		// in the second field map to find potential conflicts.

		foreach (var responseName in fieldMap1.Keys)
		{
			if (fieldMap2.TryGetValue(responseName, out var fields2) && fields2.Count != 0)
			{
				foreach (var field1 in fieldMap1[responseName])
				{
					foreach (var field2 in fields2)
					{
						var conflict = FindConflict(
							context,
							cachedFieldsAndFragmentNames,
							comparedFragmentPairs,
							parentFieldsAreMutuallyExclusive,
							responseName,
							field1,
							field2);

						if (conflict is not null)
							conflicts.Add(conflict);
					}
				}
			}
		}
	}

	private static bool DoTypesConflict(IGraphType type1, IGraphType type2)
		=> (type1, type2) switch
		{
			(ListGraphType type1List, ListGraphType type2List) => DoTypesConflict(type1List.ResolvedType!, type2List.ResolvedType!),
			(ListGraphType, _) or (_, ListGraphType) => true,
			(NonNullGraphType type1NonNull, NonNullGraphType type2NonNull) => DoTypesConflict(type1NonNull.ResolvedType!, type2NonNull.ResolvedType!),
			(NonNullGraphType, _) or (_, NonNullGraphType) => true,
			_ when type1.IsLeafType() || type2.IsLeafType() => !type1.Equals(type2),
			_ => false
		};

	private static bool SameArguments(GraphQLArguments? arguments1, GraphQLArguments? arguments2)
		=> (arguments1, arguments2) switch
		{
			(null, null) => true,
			(null, _) or (_, null) => false,
			_ when arguments1!.Count != arguments2!.Count => false,
			_ => arguments1.All(arg1 =>
			{
				var arg2 = arguments2.FirstOrDefault(_ => _.Name == arg1.Name);
				return arg2 is not null ? SameValue(arg1, arg2) : false;
			})
		};

	private static bool SameValue(GraphQLArgument arg1, GraphQLArgument arg2)
	{
		// normalize values prior to comparison by using ASTNode.Print
		return arg1.Value is null && arg2.Value is null ||
			arg1.Value is not null && arg2.Value is not null && arg1.Value.Print() == arg2.Value.Print(); //TODO: change
	}

	private static CachedField GetFieldsAndFragmentNames(
		ValidationContext context,
		Dictionary<GraphQLSelectionSet, CachedField> cachedFieldsAndFragmentNames,
		IGraphType? parentType,
		GraphQLSelectionSet selectionSet)
	{
		if (!cachedFieldsAndFragmentNames.TryGetValue(selectionSet, out var cached))
		{
			var nodeAndDef = new Dictionary<ROM, List<FieldDefPair>>();
			var fragmentNames = new HashSet<ROM>();

			CollectFieldsAndFragmentNames(
				context,
				parentType,
				selectionSet,
				nodeAndDef,
				fragmentNames);

			cached = new CachedField { NodeAndDef = nodeAndDef, Names = fragmentNames.ToList() };
			cachedFieldsAndFragmentNames.Add(selectionSet, cached);
		}
		return cached;
	}

	// Given a reference to a fragment, return the represented collection of fields
	// as well as a list of nested fragment names referenced via fragment spreads.
	private static CachedField GetReferencedFieldsAndFragmentNames(
		ValidationContext context,
		Dictionary<GraphQLSelectionSet, CachedField> cachedFieldsAndFragmentNames,
		GraphQLFragmentDefinition fragment)
	{
		// Short-circuit building a type from the node if possible.
		if (cachedFieldsAndFragmentNames.TryGetValue(fragment.SelectionSet, out var cached))
		{
			return cached;
		}

		var fragmentType = fragment.TypeCondition.Type.GraphTypeFromType(context.Schema);
		return GetFieldsAndFragmentNames(
			context,
			cachedFieldsAndFragmentNames,
			fragmentType,
			fragment.SelectionSet);
	}

	private static void CollectFieldsAndFragmentNames(
		ValidationContext context,
		IGraphType? parentType,
		GraphQLSelectionSet selectionSet,
		Dictionary<ROM, List<FieldDefPair>> nodeAndDefs,
		HashSet<ROM> fragments)
	{
		for (int i = 0; i < selectionSet.Selections.Count; i++)
		{
			var selection = selectionSet.Selections[i];
			if (selection is GraphQLField field)
			{
				var fieldName = field.Name;
				FieldType? fieldDef = null;
				if (isObjectType(parentType) || isInterfaceType(parentType))
					fieldDef = (parentType as IObjectGraphType)![fieldName];

				var responseName = field.Alias is null ? fieldName : field.Alias.Name;

				if (!nodeAndDefs.ContainsKey(responseName))
					nodeAndDefs[responseName] = new List<FieldDefPair>();

				nodeAndDefs[responseName].Add(new FieldDefPair
				{
					ParentType = parentType,
					Field = field,
					FieldDef = fieldDef
				});
			}
			else if (selection is GraphQLFragmentSpread fragmentSpread)
			{
				fragments.Add(fragmentSpread.FragmentName.Name);

			}
			else if (selection is GraphQLInlineFragment inlineFragment)
			{
				var typeCondition = inlineFragment.TypeCondition?.Type;
				var inlineFragmentType =
					typeCondition is not null
						? typeCondition.GraphTypeFromType(context.Schema)
						: parentType;

				CollectFieldsAndFragmentNames(
					context,
					inlineFragmentType,
					inlineFragment.SelectionSet,
					nodeAndDefs,
					fragments);
			}
		}
	}

	private sealed class FieldDefPair
	{
		public IGraphType? ParentType { get; set; } = null!;

		public ISelectionNode Field { get; set; } = null!;

		public FieldType? FieldDef { get; set; }
	}

	private static bool isInterfaceType(IGraphType? parentType) => parentType is IInterfaceGraphType;

	private static bool isObjectType(IGraphType? parentType) => parentType is IObjectGraphType;

	// Given a series of Conflicts which occurred between two sub-fields,
	// generate a single Conflict.
	private static Conflict? SubfieldConflicts(
		List<Conflict>? conflicts,
		ROM responseName,
		ISelectionNode node1,
		ISelectionNode node2)
	{
		if (conflicts?.Count > 0)
		{
			return new Conflict
			{
				Reason = new ConflictReason
				{
					Name = (string)responseName, //ISSUE:allocation
					Message = new Message
					{
						Msgs = conflicts.Select(c => c.Reason).ToList()
					}
				},
				FieldsLeft = conflicts.Aggregate(new List<ISelectionNode> { node1 }, (allfields, conflict) =>
				{
					allfields.AddRange(conflict.FieldsLeft);
					return allfields;
				}),
				FieldsRight = conflicts.Aggregate(new List<ISelectionNode> { node2 }, (allfields, conflict) =>
				{
					allfields.AddRange(conflict.FieldsRight);
					return allfields;
				})
			};
		}

		return null;
	}

	/// <summary>
	/// Describes a conflict between two fields in a document.
	/// </summary>
	public class Conflict
	{
		/// <summary>
		/// Returns the reason for the conflict.
		/// </summary>
		public ConflictReason Reason { get; set; } = null!;

		/// <summary>
		/// Returns a list of fields that are in conflict.
		/// </summary>
		public List<ISelectionNode> FieldsLeft { get; set; } = null!;

		/// <summary>
		/// Returns a list of fields that are in conflict.
		/// </summary>
		public List<ISelectionNode> FieldsRight { get; set; } = null!;
	}

	/// <summary>
	/// Describes the reason for a conflict.
	/// </summary>
	public class ConflictReason
	{
		/// <summary>
		/// The name of the field in conflict.
		/// </summary>
		public string Name { get; set; } = null!;

		/// <summary>
		/// Returns a message descriptor describing the conflict.
		/// </summary>
		public Message Message { get; set; } = null!;
	}

	/// <summary>
	/// A message descriptor describing a conflict.
	/// </summary>
	public class Message
	{
		/// <summary>
		/// Returns the conflict message.
		/// </summary>
		public string? Msg { get; set; }

		/// <summary>
		/// Returns a list of conflict reasons that triggered this conflict.
		/// </summary>
		public List<ConflictReason>? Msgs { get; set; }
	}

	private sealed class CachedField
	{
		public Dictionary<ROM, List<FieldDefPair>> NodeAndDef { get; set; } = null!;

		public List<ROM> Names { get; set; } = null!;
	}

	private sealed class PairSet
	{
		private readonly Dictionary<ROM, Dictionary<ROM, bool>> _data = new();

		public bool Has(ROM a, ROM b, bool areMutuallyExclusive)
		{
			_data.TryGetValue(a, out var first);

			if (first?.TryGetValue(b, out bool result) is not true)
				return false;

			return areMutuallyExclusive ? true : !result;
		}

		public void Add(ROM a, ROM b, bool areMutuallyExclusive)
		{
			PairSetAdd(a, b, areMutuallyExclusive);
			PairSetAdd(b, a, areMutuallyExclusive);
		}

		private void PairSetAdd(ROM a, ROM b, bool areMutuallyExclusive)
		{
			_data.TryGetValue(a, out var map);

			if (map is null)
			{
				map = new Dictionary<ROM, bool>();
				_data[a] = map;
			}

			map[b] = areMutuallyExclusive;
		}
	}
}

internal static class ISelectionNodeExtensions
{
	public static ROM GetName(this ISelectionNode selection)
		=> selection switch
		{
			GraphQLField field => field.Name,
			GraphQLFragmentSpread fragmentSpread => fragmentSpread.FragmentName.Name,
			_ => default
		};

	public static GraphQLArguments? GetArguments(this ISelectionNode selection)
		=> selection is GraphQLField field ? field.Arguments : null;

	public static GraphQLSelectionSet? GetSelectionSet(this ISelectionNode selection)
		=> selection switch
		{
			GraphQLField field => field.SelectionSet,
			GraphQLInlineFragment inlineFragment => inlineFragment.SelectionSet,
			_ => null
		};
}
