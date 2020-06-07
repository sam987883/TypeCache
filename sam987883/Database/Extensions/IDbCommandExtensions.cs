// Copyright (c) 2020 Samuel Abraham

using System.Data;
using sam987883.Common;
using sam987883.Extensions;
using sam987883.Reflection;

namespace sam987883.Database.Extensions
{
	public static class IDbCommandExtensions
	{
		public static void AddParameters<T>(this IDbCommand @this, ITypeCache<T> typeCache, T parameters) where T : class =>
			Factory.CreateFieldAccessor(parameters).Do(pair => @this.AddInputParameter(pair.Key, pair.Value));

		public static int AddInputParameter(this IDbCommand @this, string name, object? value, DbType? dbType = null)
		{
			var parameter = @this.CreateParameter();
			parameter.Direction = ParameterDirection.Input;
			parameter.ParameterName = name;
			parameter.Value = value;
			if (dbType.HasValue)
				parameter.DbType = dbType.Value;
			return @this.Parameters.Add(parameter);
		}
	}
}