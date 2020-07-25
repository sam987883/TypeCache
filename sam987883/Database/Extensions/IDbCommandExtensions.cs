// Copyright (c) 2020 Samuel Abraham

using sam987883.Common;
using sam987883.Common.Extensions;
using System.Data;

namespace sam987883.Database.Extensions
{
	public static class IDbCommandExtensions
	{
		public static void AddParameters<T>(this IDbCommand @this, T parameters) where T : class =>
			Factory.CreateFieldAccessor(parameters).Values.Do(pair => @this.AddInputParameter(pair.Key, pair.Value));

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