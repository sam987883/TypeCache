// Copyright (c) 2021 Samuel Abraham

namespace TypeCache.Data
{
	/// <summary>
	/// JSON: <code>{ "ParameterName": "ParameterValue" }</code>
	/// SQL: <code>SET @ParameterName = N'ParameterValue';</code>
	/// </summary>
	public record Parameter(string Name, object? Value);
}
