// Copyright (c) 2020 Samuel Abraham

using static sam987883.Extensions.IEnumerableExtensions;
using static sam987883.Extensions.IReadOnlyDictionaryExtensions;

namespace sam987883.Reflection
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

		public void Map(FROM from, TO to)
		{
			var toProperties = this._ToFieldCache.Fields.Values
				.If(fieldMember => fieldMember.Public && fieldMember.SetValue != null)
				.To(fieldMember => fieldMember.Name)
				.ToArray();
			this._FromFieldCache.Fields
				.GetValues(toProperties)
				.If(fieldMember => fieldMember.Public && fieldMember.GetValue != null)
				.Do(fromField => this._ToFieldCache.Fields[fromField.Name][to] = fromField[from]);
		}
	}
}
