// Copyright (c) 2020 Samuel Abraham

using sam987883.Common;
using sam987883.Reflection;

namespace sam987883.Database.Commands
{
	public class Update
	{
		public string Table { get; set; } = string.Empty;

		public (string Column, object Value)[] Set { get; set; } = new (string, object)[0];

		public ExpressionSet Where { get; set; } = new ExpressionSet();

		public Output Output { get; set; } = new Output();
	}

	public class Update<T> where T : class, new()
	{
		public Update(IPropertyCache<T> propertyCache) =>
			this.PropertyCache = propertyCache;

		internal IPropertyCache<T> PropertyCache { get; set; }

		public string Table { get; set; } = string.Empty;

		public (string Column, object Value)[] Set { get; set; } = new (string, object)[0];

		public ExpressionSet Where { get; set; } = new ExpressionSet();

		public Output<T> Output { get; set; } = new Output<T>();
	}
}
