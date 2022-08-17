// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using GraphQL.Types;
using GraphQL.Utilities;
using GraphQLParser;
using TypeCache.Collections.Extensions;
using TypeCache.Extensions;
using TypeCache.Reflection.Extensions;
using static TypeCache.Default;

namespace TypeCache.GraphQL.Types;

public abstract class GraphQLComplexType : IComplexGraphType
{
	public GraphQLComplexType(string name)
	{
		name.AssertNotBlank();
		NameValidator.ValidateName(name, NamedElement.Type);

		this.Name = name;
		this.Fields = new();
		this.Metadata = new(StringComparer.OrdinalIgnoreCase);
	}

	public string? Description { get; set; }

	public string? DeprecationReason { get; set; }

	public TypeFields Fields { get; }

	public Dictionary<string, object?> Metadata { get; }

	public string Name { get; set; }

	public FieldType AddField(FieldType fieldType)
	{
		fieldType.AssertNotNull();
		fieldType.Name.AssertNotBlank();

		if (this.HasField(fieldType.Name))
		{
			var output = new DefaultInterpolatedStringHandler(100, 2);
			output.AppendLiteral("A field with the name '");
			output.AppendFormatted(fieldType.Name);
			output.AppendLiteral("' is already registered for GraphType '");
			output.AppendFormatted(this.Name);
			output.AppendLiteral("'");
			throw new ArgumentOutOfRangeException(nameof(fieldType), output.ToStringAndClear());
		}

		if (fieldType.ResolvedType is null)
		{
			if (fieldType.Type is null)
			{
				var output = new DefaultInterpolatedStringHandler(100, 4);
				output.AppendLiteral("The declared field '");
				output.AppendFormatted(fieldType.Name);
				output.AppendLiteral("' on '");
				output.AppendFormatted(this.Name);
				output.AppendLiteral("' requires a field '");
				output.AppendFormatted("Type");
				output.AppendLiteral("' when no '");
				output.AppendFormatted("ResolvedType");
				output.AppendLiteral("' is provided.");
				throw new ArgumentOutOfRangeException(nameof(fieldType), output.ToStringAndClear());
			}

			if (!fieldType.Type.Implements<IGraphType>())
			{
				var output = new DefaultInterpolatedStringHandler(100, 3);
				output.AppendLiteral("The declared field '");
				output.AppendFormatted(fieldType.Name);
				output.AppendLiteral("' on '");
				output.AppendFormatted(this.Name);
				output.AppendLiteral("' has a field Type of '");
				output.AppendFormatted(fieldType.Type.Name);
				output.AppendLiteral("' that should derive from GraphType.");
				throw new ArgumentOutOfRangeException(nameof(fieldType), output.ToStringAndClear());
			}

			if (fieldType.Type.Implements<IInputObjectGraphType>() is true)
			{
				var output = new DefaultInterpolatedStringHandler(100, 2);
				output.AppendLiteral("Output type '");
				output.AppendFormatted(this.Name);
				output.AppendLiteral("' can only have fields of output types. Field '");
				output.AppendFormatted(fieldType.Name);
				output.AppendLiteral("' has an input type.");
				throw new ArgumentOutOfRangeException(nameof(fieldType), output.ToStringAndClear());
			}
		}

		// this.Fields.Add(fieldType);
		TypeOf<TypeFields>.InvokeMethod("Add", this.Fields, fieldType);

		return fieldType;
	}

	public FieldType? GetField(ROM name)
	{
		if (name.IsEmpty)
			return null;

		var fieldTypeName = new string(name.Span);
		return this.Fields.First(fieldType => fieldType.Name.Is(fieldTypeName, StringComparison.Ordinal));
	}

	public TType GetMetadata<TType>(string key, TType? defaultValue = default)
		=> this.Metadata.TryGetValue(key, out var value) ? (TType)value! : defaultValue!;

	public TType GetMetadata<TType>(string key, Func<TType> defaultValueFactory)
	{
		defaultValueFactory.AssertNotNull();
		return this.Metadata.TryGetValue(key, out var value) ? (TType)value! : defaultValueFactory();
	}

	public bool HasField(string name)
		=> name.IsNotBlank() && this.Fields.Any(fieldType => fieldType.Name.Is(name, StringComparison.Ordinal));

	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public bool HasMetadata(string key)
		=> this.Metadata.ContainsKey(key);

	public override bool Equals(object? item)
		=> this == item || (item is IGraphType graphType && this.Name.Is(graphType.Name, StringComparison.Ordinal));

	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public override int GetHashCode()
		=> this.Name.GetHashCode();

	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public override string ToString()
		=> this.Name;

	public abstract void Initialize(ISchema schema);
}
