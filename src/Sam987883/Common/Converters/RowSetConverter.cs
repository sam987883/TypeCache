// Copyright (c) 2020 Samuel Abraham

using Sam987883.Common.Extensions;
using Sam987883.Common.Models;
using Sam987883.Reflection;
using Sam987883.Reflection.Members;

namespace Sam987883.Common.Converters
{
	internal sealed class RowSetConverter<T> : IRowSetConverter<T>
		where T : class, new()
	{
		private readonly ITypeCache<T> _TypeCache;

		public RowSetConverter(ITypeCache<T> typeCache) =>
			this._TypeCache = typeCache;

		T[] IRowSetConverter<T>.FromRowSet(RowSet rowSet)
		{
			(IPropertyMember<T> Property, int Index)[] members = this._TypeCache.PropertyCache.Properties
				.GetValues(rowSet.Columns)
				.To(_ => (_, rowSet.Columns.ToIndex(_.Name).First().Value))
				.ToArray(rowSet.Columns.Length);

			var items = new T[rowSet.Rows.Length];

			rowSet.Rows.Do((row, rowIndex) =>
			{
				var item = this._TypeCache.ConstructorCache.Create();
				members.Do(member => member.Property[item] = row[member.Index]);
				items[rowIndex] = item;
			});

			return items;
		}

		RowSet IRowSetConverter<T>.ToRowSet(T[] items, params string[] columns)
		{
			var members = columns.Any()
				? this._TypeCache.PropertyCache.Properties
					.GetValues(columns)
					.ToList()
				: this._TypeCache.PropertyCache.Properties
					.If(_ => _.Value.GetMethod != null)
					.To(_ => _.Value)
					.ToList();

			var rowSet = new RowSet
			{
				Columns = members.To(_ => _.Name).ToList().ToArray(),
				Rows = new object?[items.Length][]
			};

			items.Do((item, rowIndex) =>
			{
				var row = new object?[rowSet.Columns.Length];
				members.Do((member, i) => row[i] = member[item]);
				items[rowIndex] = item;
			});

			return rowSet;
		}
	}
}
