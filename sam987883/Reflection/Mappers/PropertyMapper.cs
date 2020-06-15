// Copyright (c) 2020 Samuel Abraham

using sam987883.Reflection.Members;
using static sam987883.Extensions.IEnumerableExtensions;
using static sam987883.Extensions.IReadOnlyDictionaryExtensions;

namespace sam987883.Reflection.Mappers
{
	public class PropertyMapper<FROM, TO> : IPropertyMapper<FROM, TO>
	{
		private readonly IPropertyCache<FROM> _FromPropertyCache;

		private readonly IPropertyCache<TO> _ToPropertyCache;

		public PropertyMapper(IPropertyCache<FROM> fromPropertyCache, IPropertyCache<TO> toPropertyCache)
		{
			this._FromPropertyCache = fromPropertyCache;
			this._ToPropertyCache = toPropertyCache;
		}

		/// <summary>
		/// Homogenius mapping- property names and types match.
		/// Property types must be primitives or strings.
		/// Nullable<> to/from non-Nullable<> value types will be handled.
		/// </summary>
		/// <param name="from">Mapping from instance</param>
		/// <param name="to">Mapping to instance</param>
		public void Map(FROM from, TO to)
		{
			var toProperties = this._ToPropertyCache.Properties.Values
				.If(propertyMember => propertyMember.Public && propertyMember.SetMethod != null)
				.To(propertyMember => propertyMember.Name)
				.ToArray();
			this._FromPropertyCache.Properties
				.GetValues(toProperties)
				.If(propertyMember => propertyMember.Public && propertyMember.GetMethod != null)
				.Do(fromProperty =>
				{
					var toProperty = this._ToPropertyCache.Properties[fromProperty.Name];
					if (toProperty.IsValueType || toProperty.IsString)
					{
						if (toProperty.TypeHandle.Equals(fromProperty.TypeHandle) // Assign ValueType/String values, copying handled automatically
							|| toProperty.NullableTypeHandle == fromProperty.TypeHandle) // Assign ValueType to Nullable<ValueType>
							toProperty[to] = fromProperty[from];
						else if (toProperty.TypeHandle == fromProperty.NullableTypeHandle)
						{ // Assign Nullable<ValueType> to ValueType if not null
							var value = fromProperty[from];
							if (value != null)
								toProperty[to] = value;
						}
					}
				});
		}
	}
}
