// Copyright (c) 2021 Samuel Abraham

using TypeCache.Data.Mediation;

namespace TypeCache.Data.Extensions;

public static class SqlCommandExtensions
{
	public static SqlDataSetRequest ToSqlDataSetRequest(this SqlCommand @this)
		=> new(@this);

	public static SqlDataTableRequest ToSqlDataTableRequest(this SqlCommand @this)
		=> new(@this);

	public static SqlExecuteRequest ToSqlExecuteRequest(this SqlCommand @this)
		=> new(@this);

	public static SqlJsonArrayRequest ToSqlJsonArrayRequest(this SqlCommand @this)
		=> new(@this);

	public static SqlModelsRequest ToSqlModelsRequest<T>(this SqlCommand @this, int listInitialCapacity = 0)
		=> new(@this, typeof(T))
		{
			ListInitialCapacity = listInitialCapacity,
		};

	public static SqlScalarRequest ToSqlScalarRequest(this SqlCommand @this)
		=> new(@this);
}
