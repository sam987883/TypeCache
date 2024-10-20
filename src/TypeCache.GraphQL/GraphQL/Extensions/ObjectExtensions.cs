using System.Collections;
using GraphQL.Types;
using TypeCache.Extensions;
using TypeCache.Utilities;

namespace GraphQL;

/// <summary>
/// Provides extension methods for objects and a method for converting a dictionary into a strongly typed object.
/// </summary>
public static class ObjectExtensions
{
	/// <summary>
	/// Converts the indicated value into a type that is compatible with fieldType.
	/// </summary>
	/// <param name="propertyValue">The value to be converted.</param>
	/// <param name="fieldType">The desired type.</param>
	/// <remarks>There is special handling for strings, IEnumerable&lt;T&gt;, Nullable&lt;T&gt;, and Enum.</remarks>
	public static object? GetPropertyValue(this object? propertyValue, Type fieldType)
	{
		// Short-circuit conversion if the property value already of the right type
		if (propertyValue is null || fieldType == typeof(object) || fieldType.IsInstanceOfType(propertyValue))
			return propertyValue;

		var enumerableInterface = fieldType.Name == "IEnumerable`1"
		  ? fieldType
		  : fieldType.GetInterface("IEnumerable`1");

		if (fieldType != typeof(string) && enumerableInterface is not null)
		{
			IList newCollection;
			var elementType = enumerableInterface.GetGenericArguments()[0];
			var underlyingType = Nullable.GetUnderlyingType(elementType) ?? elementType;
			var fieldTypeImplementsIList = fieldType.Implements(typeof(IList<>));

			var propertyValueAsIList = propertyValue as IList;

			// Custom container
			if (fieldTypeImplementsIList && !fieldType.IsArray)
				newCollection = (IList)fieldType.Create()!;
			// Array of known size is created immediately
			else if (fieldType.IsArray && propertyValueAsIList is not null)
				newCollection = Array.CreateInstance(elementType, propertyValueAsIList.Count);
			// List<T>
			else
			{
				var genericListType = typeof(List<>).MakeGenericType(elementType);
				newCollection = (IList)genericListType.Create()!;
			}

			if (propertyValue is not IEnumerable valueList)
				return newCollection;

			// Array of known size is populated in-place
			if (fieldType.IsArray && propertyValueAsIList is not null)
			{
				for (int i = 0; i < propertyValueAsIList.Count; ++i)
				{
					newCollection[i] = GetPropertyValue(propertyValueAsIList[i], underlyingType);
				}
			}
			// Array of unknown size is created only after populating list
			else
			{
				foreach (var listItem in valueList)
				{
					newCollection.Add(GetPropertyValue(listItem, underlyingType));
				}

				if (fieldType.IsArray)
					newCollection = ((dynamic)newCollection!).ToArray();
			}

			return newCollection;
		}

		if (propertyValue is IDictionary<string, object?> dictionary)
		{
			var result = fieldType.Create();
			if (result is not null)
				dictionary.MapTo(result);

			return result!;
		}

		if (fieldType.IsEnum)
		{
			var value = propertyValue;
			if (value is null)
			{
				var enumNames = Enum.GetNames(fieldType);
				value = enumNames[0];
			}

			if (!Enum.IsDefined(fieldType, value))
				throw new InvalidOperationException($"Unknown value '{value}' for enum '{fieldType.Name}'.");

			return Enum.Parse(fieldType, value.ToString()!, true);
		}

		return propertyValue.ConvertTo(fieldType);
	}
}
