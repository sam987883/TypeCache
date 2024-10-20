using System.Reflection;
using GraphQLParser.AST;
using System.Collections.Generic;
using TypeCache.Utilities;
using TypeCache.Extensions;

namespace GraphQL.Types;

/// <summary>
/// Represents an input object graph type.
/// </summary>
public interface IInputObjectGraphType : IGraphType
{
	FieldType? this[string field] => this.Fields.FirstOrDefault(_ => _.Name.EqualsIgnoreCase(field));

	FieldType? this[GraphQLName field] => this.Fields.FirstOrDefault(_ => _.Name.EqualsIgnoreCase(field.StringValue));

	ISet<FieldType> Fields { get; }

	/// <summary>
	/// Returns a boolean indicating if the provided value is valid as a default value for a
	/// field or argument of this type.
	/// </summary>
	bool IsValidDefault(object value);

	/// <summary>
	/// Converts a supplied dictionary of keys and values to an object.
	/// Overriding this method allows for customizing the deserialization process of input objects,
	/// much like a field resolver does for output objects. For example, you can set some 'computed'
	/// properties for your input object which were not passed in the GraphQL request.
	/// </summary>
	object ParseDictionary(IDictionary<string, object?> value);

	/// <summary>
	/// Converts a value to an AST representation. This is necessary for introspection queries
	/// to return the default value for fields of this scalar type. This method may throw an exception
	/// or return <see langword="null"/> for a failed conversion.
	/// </summary>
	GraphQLValue ToAST(object value);
}

/// <inheritdoc/>
public class InputObjectGraphType : InputObjectGraphType<object>
{
	internal const string ORIGINAL_EXPRESSION_PROPERTY_NAME = nameof(ORIGINAL_EXPRESSION_PROPERTY_NAME);
}

/// <inheritdoc cref="IInputObjectGraphType"/>
public class InputObjectGraphType<[NotAGraphType] TSourceType> : GraphType, IInputObjectGraphType
{
	public FieldType? this[string field] => this.Fields.FirstOrDefault(_ => _.Name.EqualsIgnoreCase(field));

	public ISet<FieldType> Fields { get; } = new HashSet<FieldType>();

	/// <summary>
	/// Converts a supplied dictionary of keys and values to an object.
	/// Overriding this method allows for customizing the deserialization process of input objects,
	/// much like a field resolver does for output objects. For example, you can set some 'computed'
	/// properties for your input object which were not passed in the GraphQL request.
	/// </summary>
	public virtual object ParseDictionary(IDictionary<string, object?> value)
	{
		if (value is null)
			return null!;

		// for InputObjectGraphType just return the dictionary
		if (typeof(TSourceType) == typeof(object))
			return value;

		// for InputObjectGraphType<TSourceType>, convert to TSourceType via ToObject.
		var result = typeof(TSourceType).Create();
		if (result is not null)
			value.MapTo(result);

		return result!;
	}

	/// <inheritdoc/>
	public virtual bool IsValidDefault(object value)
	{
		if (value is not TSourceType)
			return false;

		foreach (var field in this.Fields)
		{
			if (!field.ResolvedType!.IsValidDefault(GetFieldValue(field, value)))
				return false;
		}

		return true;
	}

	/// <summary>
	/// Converts a value to an AST representation. This is necessary for introspection queries
	/// to return the default value for fields of this input object type. Also AST representation
	/// is used while printing schema as SDL.
	/// <br/>
	/// This method may throw an exception or return <see langword="null"/> for a failed conversion.
	/// <br/><br/>
	/// The default implementation returns <see cref="GraphQLNullValue"/> if <paramref name="value"/>
	/// is <see langword="null"/> and <see cref="GraphQLObjectValue"/> filled with the values
	/// for all input fields except ones returning <see cref="GraphQLNullValue"/>.
	/// <br/><br/>
	/// Note that you may need to override this method if you have already overrided <see cref="ParseDictionary"/>.
	/// </summary>
	public virtual GraphQLValue ToAST(object? value)
	{
		if (value is null)
			return Singleton<GraphQLNullValue>.Instance;

		var objectValue = new GraphQLObjectValue
		{
			Fields = new List<GraphQLObjectField>(this.Fields.Count)
		};

		foreach (var field in this.Fields)
		{
			var fieldValue = field.ResolvedType!.ToAST(GetFieldValue(field, value));
			if (fieldValue is not GraphQLNullValue)
				objectValue.Fields.Add(new GraphQLObjectField(new GraphQLName(field.Name), fieldValue));
		}

		return objectValue;
	}

	private static object? GetFieldValue(FieldType field, object? value)
	{
		if (value is null)
			return null;

		// Given Field("FirstName", x => x.FName) and key == "FirstName" returns "FName"
		var propertyName = field.GetMetadata(InputObjectGraphType.ORIGINAL_EXPRESSION_PROPERTY_NAME, field.Name) ?? field.Name;
		var propertyInfo = value.GetType().GetPropertyInfo(propertyName, true);
		return propertyInfo?.CanRead is true ? propertyInfo.GetValueEx(value) : null;
	}
}
