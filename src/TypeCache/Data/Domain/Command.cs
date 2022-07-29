// Copyright (c) 2021 Samuel Abraham

using System.Collections.Generic;
using System.Data;
using System.Text.Json.Serialization;
using TypeCache.Converters;
using TypeCache.Extensions;
using static TypeCache.Default;

namespace TypeCache.Data.Domain
{
	/// <summary>
	/// Use Parameters to take in user input to avoid SQL Injection.
	/// </summary>
	public abstract class Command
	{
		/// <summary>
		/// <list type="bullet">
		/// <item><term><c><see cref="System.Data.CommandType.StoredProcedure"/></c></term> <description>Stored Procedure commands only.</description></item>
		/// <item><term><c><see cref="System.Data.CommandType.Text"/></c></term> <description>All other SQL commands.</description></item>
		/// </list>
		/// </summary>
		[JsonIgnore]
		public virtual CommandType CommandType { get; } = CommandType.Text;

		/// <summary>
		/// The data source name that contains the connection string and database provider to use.
		/// </summary>
		public string DataSource { get; set; } = DATASOURCE;

		/// <summary>
		/// JSON: <code>"InputParameters": { "ParameterName1": "ParameterValue1", "ParameterName2": null, "ParameterName3": 123 }</code>
		/// SQL:
		/// <code>
		/// SET @ParameterName1 = N'ParameterValue1';<br/>
		/// SET @ParameterName2 = NULL;<br/>
		/// SET @ParameterName3 = 123;
		/// </code>
		/// </summary>
		[JsonConverter(typeof(DictionaryJsonConverter))]
		public IDictionary<string, object?> InputParameters { get; set; } = new Dictionary<string, object?>(0, STRING_COMPARISON.ToStringComparer());

		/// <summary>
		/// JSON: <code>"InputParameters": { "ParameterName4": "Int32", "ParameterName5": "String", "ParameterName6": "DateTimeOffset" }</code>
		/// SQL:
		/// <code>
		/// DECLARE @ParameterName4 AS INTEGER;<br/>
		/// DECLARE @ParameterName5 AS NVARCHAR(MAX);<br/>
		/// DECLARE @ParameterName6 AS DATETIMEOFFSET;
		/// </code>
		/// </summary>
		[JsonConverter(typeof(DictionaryJsonConverter<DbType>))]
		public IDictionary<string, DbType> OutputParameters { get; set; } = new Dictionary<string, DbType>(0, STRING_COMPARISON.ToStringComparer());

		/// <summary>
		/// Converts the <see cref="Command"/> to SQL.
		/// </summary>
		public abstract string ToSQL();
	}
}
