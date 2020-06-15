// Copyright (c) 2020 Samuel Abraham

using static sam987883.Extensions.IEnumerableExtensions;
using static sam987883.Extensions.IReadOnlyDictionaryExtensions;

namespace sam987883.Reflection.Mappers
{
	public class FieldMapper<FROM, TO> : IFieldMapper<FROM, TO>
	{
		private readonly IFieldCache<FROM> _FromFieldCache;

		private readonly IFieldCache<TO> _ToFieldCache;

		public FieldMapper(IFieldCache<FROM> fromFieldCache, IFieldCache<TO> toFieldCache)
		{
			this._FromFieldCache = fromFieldCache;
			this._ToFieldCache = toFieldCache;
		}

		/// <summary>
		/// Homogenius mapping- field names and types match.
		/// Field types must be primitives or strings.
		/// Nullable<> to/from non-Nullable<> value types will be handled.
		/// </summary>
		/// <param name="from">Mapping from instance</param>
		/// <param name="to">Mapping to instance</param>
		public void Map(FROM from, TO to)
		{
			var toProperties = this._ToFieldCache.Fields.Values
				.If(fieldMember => fieldMember.Public && fieldMember.SetValue != null)
				.To(fieldMember => fieldMember.Name)
				.ToArray();
			this._FromFieldCache.Fields
				.GetValues(toProperties)
				.If(fieldMember => fieldMember.Public && fieldMember.GetValue != null)
				.Do(fromField =>
				{
					var toField = this._ToFieldCache.Fields[fromField.Name];
					if (toField.IsValueType || toField.IsString)
					{
						if (toField.TypeHandle.Equals(fromField.TypeHandle) // Assign ValueType/String values, copying handled automatically
							|| toField.NullableTypeHandle == fromField.TypeHandle) // Assign ValueType to Nullable<ValueType>
							toField[to] = fromField[from];
						else if (toField.TypeHandle == fromField.NullableTypeHandle)
						{ // Assign Nullable<ValueType> to ValueType if not null
							var value = fromField[from];
							if (value != null)
								toField[to] = value;
						}
					}
				});
		}
	}
}
