// Copyright (c) 2020 Samuel Abraham

using System.Data;

namespace sam987883.Database.Extensions
{
	public static class IDbCommandExtensions
	{
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