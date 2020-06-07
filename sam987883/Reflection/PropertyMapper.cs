// Copyright (c) 2020 Samuel Abraham

using static sam987883.Extensions.IEnumerableExtensions;
using static sam987883.Extensions.IReadOnlyDictionaryExtensions;

namespace sam987883.Reflection
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

		public void Map(FROM from, TO to)
		{
			var toProperties = this._ToPropertyCache.Properties.Values
				.If(propertyMember => propertyMember.Public && propertyMember.SetMethod != null)
				.To(propertyMember => propertyMember.Name)
				.ToArray();
			this._FromPropertyCache.Properties
				.GetValues(toProperties)
				.If(propertyMember => propertyMember.Public && propertyMember.GetMethod != null)
				.Do(fromProperty => this._ToPropertyCache.Properties[fromProperty.Name][to] = fromProperty[from]);
		}
	}
}
