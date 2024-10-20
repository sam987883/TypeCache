using System.Reflection;
using GraphQL.Utilities;
using TypeCache.Extensions;
using TypeCache.GraphQL.Extensions;
using TypeCache.GraphQL.Resolvers;
using TypeCache.GraphQL.Types;
using TypeCache.Utilities;

namespace GraphQL.Types;

/// <summary>
/// Represents a field of a graph type.
/// </summary>
[DebuggerDisplay("{Name,nq}: {ResolvedType,nq}")]
public class FieldType : MetadataProvider, IFieldType
{
	private string? _name;
	private Type? _type;

	public FieldType()
	{
	}

	public FieldType(PropertyInfo propertyInfo)
	{
		var type = propertyInfo.ToGraphQLType(false);
		var arguments = new List<QueryArgument>(8);

		if (type.IsAssignableTo<ScalarGraphType>() && !type.Implements(typeof(NonNullGraphType<>)))
			arguments.Add(new("null", type)
			{
				Description = "Return this if the value is null."
			});

		if (propertyInfo.PropertyType.IsAssignableTo<IFormattable>())
			arguments.Add(new("format", typeof(GraphQLStringType))
			{
				Description = "Use .NET format specifiers to format the data.",
			});

		if (type.Is<GraphQLStringType<DateTime>>() || type.Is<NonNullGraphType<GraphQLStringType<DateTime>>>())
			arguments.Add(new("timeZone", typeof(GraphQLStringType))
			{
				Description = Invariant($"{typeof(TimeZoneInfo).Namespace}.{nameof(TimeZoneInfo)}.{nameof(TimeZoneInfo.ConvertTimeBySystemTimeZoneId)}(value, [..., ...] | [UTC, ...])"),
			});

		if (type.Is<GraphQLStringType<DateTimeOffset>>() || type.Is<NonNullGraphType<GraphQLStringType<DateTimeOffset>>>())
			arguments.Add(new("timeZone", typeof(GraphQLStringType))
			{
				Description = Invariant($"{typeof(TimeZoneInfo).Namespace}.{nameof(TimeZoneInfo)}.{nameof(TimeZoneInfo.ConvertTimeBySystemTimeZoneId)}(value, ...)"),
			});

		if (type.Is<GraphQLStringType>() || type.Is<NonNullGraphType<GraphQLStringType>>())
		{
			arguments.AddRange(
			[
				new("case", typeof(GraphQLEnumType<StringCase>)) { Description = "value.ToLower(), value.ToLowerInvariant(), value.ToUpper(), value.ToUpperInvariant()" },
				new("length", typeof(GraphQLNumberType<int>)) { Description = "value.Left(length)" },
				new("match", typeof(GraphQLStringType)) { Description = "value.ToRegex(RegexOptions.Compiled | RegexOptions.Singleline).Match(match)" },
				new("trim", typeof(GraphQLStringType)) { Description = "value.Trim()" },
				new("trimEnd", typeof(GraphQLStringType)) { Description = "value.TrimEnd()" },
				new("trimStart", typeof(GraphQLStringType)) { Description = "value.TrimStart()" }
			]);
		}

		this.Arguments = arguments.ToArray();
		this.Type = type;
		this.Name = propertyInfo.GraphQLName();
		this.Description = propertyInfo.GraphQLDescription();
		this.DeprecationReason = propertyInfo.GraphQLDeprecationReason();
		this.Resolver = new PropertyFieldResolver(propertyInfo);
	}

	/// <inheritdoc/>
	public string Name
	{
		get => _name!;
		set => SetName(value, validate: true);
	}

	internal void SetName(string name, bool validate)
	{
		if (_name != name)
		{
			if (validate)
				NameValidator.ValidateName(name, NamedElement.Field);

			_name = name;
		}
	}

	public int GetHashCode([DisallowNull] IFieldType obj)
		=> obj.Name.GetHashCode();

	public bool Equals(IFieldType? other)
		=> this.Name.EqualsIgnoreCase(other?.Name);

	/// <inheritdoc/>
	public string? Description { get; set; }

	/// <inheritdoc/>
	public string? DeprecationReason { get; set; }

	/// <summary>
	/// Gets or sets the default value of the field. Only applies to fields of input object graph types.
	/// </summary>
	public object? DefaultValue { get; set; }

	/// <summary>
	/// Gets or sets the graph type of this field.
	/// </summary>
	public Type? Type
	{
		get => _type;
		set
		{
			if (value?.IsGenericTypeDefinition is true)
				throw new ArgumentOutOfRangeException(nameof(value), $"Type '{value}' should not be an open generic type definition.");

			_type = value;
		}
	}

	/// <summary>
	/// Gets or sets the graph type of this field.
	/// </summary>
	public IGraphType? ResolvedType { get; set; }

	/// <inheritdoc/>
	public QueryArgument[] Arguments { get; set; } = Array<QueryArgument>.Empty;

	/// <summary>
	/// Gets or sets a field resolver for the field. Only applicable to fields of output graph types.
	/// </summary>
	public IFieldResolver? Resolver { get; set; }

	/// <summary>
	/// Gets or sets a subscription resolver for the field. Only applicable to the root fields of subscription.
	/// </summary>
	public ISourceStreamResolver? StreamResolver { get; set; }
}
