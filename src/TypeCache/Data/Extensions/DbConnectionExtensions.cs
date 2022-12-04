// Copyright (c) 2021 Samuel Abraham

using System.Data.Common;

namespace TypeCache.Data.Extensions;

public static class DbConnectionExtensions
{
	/// <summary>
	/// <code>
	/// {<br/>
	/// <see langword="    var"/> dbCommand = @<paramref name="this"/>.CreateCommand();<br/>
	/// <see langword="    "/>dbCommand.CommandType = <paramref name="sqlCommand"/>.Type;<br/>
	/// <see langword="    "/>dbCommand.CommandText = <paramref name="sqlCommand"/>.ToSQL();<br/>
	/// <see langword="    if"/> (<paramref name="sqlCommand"/>.Timeout.HasValue)<br/>
	/// <see langword="        "/>dbCommand.CommandTimeout = (<see cref="int"/>)<paramref name="sqlCommand"/>.Timeout.Value.TotalSeconds;<br/>
	/// <br/>
	/// <see langword="    foreach"/> (<see langword="var"/> pair <see langword="in"/> <paramref name="sqlCommand"/>.Parameters)<br/>
	/// <see langword="        "/>dbCommand.AddInputParameter(pair.Key, pair.Value);<br/>
	/// <br/>
	/// <see langword="    foreach"/> (<see langword="var"/> pair <see langword="in"/> <paramref name="sqlCommand"/>.OutputParameters)<br/>
	/// <see langword="        "/>dbCommand.AddOutputParameter(pair.Key, pair.Value);<br/>
	/// <br/>
	/// <see langword="    return"/> dbCommand;<br/>
	/// }
	/// </code>
	/// </summary>
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
