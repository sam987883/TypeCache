// Copyright (c) 2021 Samuel Abraham

using System.Data.Common;

namespace TypeCache.Data.Extensions;

public static class DbConnectionExtensions
{
	public static DbCommand CreateCommand(this DbConnection @this, SqlCommand sqlCommand)
	{
		var dbCommand = @this.CreateCommand();
		dbCommand.CommandType = sqlCommand.Type;
		dbCommand.CommandText = sqlCommand.ToSQL();
		if (sqlCommand.Timeout.HasValue)
			dbCommand.CommandTimeout = (int)sqlCommand.Timeout.Value.TotalSeconds;

		foreach (var pair in sqlCommand.Parameters)
			dbCommand.AddInputParameter(pair.Key, pair.Value);

		foreach (var pair in sqlCommand.OutputParameters)
			dbCommand.AddOutputParameter(pair.Key, pair.Value);

		return dbCommand;
	}
}
