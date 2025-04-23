// Copyright (c) 2021 Samuel Abraham

using TypeCache.Data.Mediation;

namespace TypeCache.Data.Extensions;

public static class SqlCommandExtensions
{
	public static SqlDataSetRequest ToSqlDataSetRequest(this SqlCommand @this)
		=> new()
		{
			Command = @this
		};

	public static SqlDataTableRequest ToSqlDataTableRequest(this SqlCommand @this)
		=> new()
		{
			Command = @this
		};

	public static SqlExecuteRequest ToSqlExecuteRequest(this SqlCommand @this)
		=> new()
		{
			Command = @this
		};

	public static SqlResultJsonRequest ToSqlJsonArrayRequest(this SqlCommand @this)
		=> new()
		{
			Command = @this
		};

	public static SqlModelsRequest ToSqlModelsRequest<T>(this SqlCommand @this, int listInitialCapacity = 0)
		=> new()
		{
			Command = @this,
			ListInitialCapacity = listInitialCapacity,
			ModelType = typeof(T)
		};

	public static SqlScalarRequest ToSqlScalarRequest(this SqlCommand @this)
		=> new()
		{
			Command = @this
		};
}
