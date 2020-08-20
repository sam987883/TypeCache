// Copyright (c) 2020 Samuel Abraham

using Sam987883.Common.Extensions;
using System;
using System.Collections.Immutable;

namespace Sam987883.Database.Models
{
	public class ObjectSchema
	{
		/// <summary>
		/// The database object_id.
		/// </summary>
		public int Id { get; set; }

		public ObjectType Type { get; set; }

		/// <summary>
		/// Database table/view/function name.
		/// </summary>
		public string ObjectName { get; set; } = string.Empty;

		/// <summary>
		/// Database schema name ie. dbo.
		/// </summary>
		public string SchemaName { get; set; } = string.Empty;

		/// <summary>
		/// Database name - pulled from the connection string.
		/// </summary>
		public string DatabaseName { get; set; } = string.Empty;

		public IImmutableList<ColumnSchema> Columns { get; set; } = ImmutableArray<ColumnSchema>.Empty;

		public IImmutableList<ParameterSchema> Parameters { get; set; } = ImmutableArray<ParameterSchema>.Empty;

		public string Name =>
			$"[{this.DatabaseName}].[{this.SchemaName}].[{this.ObjectName}]";

		public bool HasColumn(string column) =>
			this.Columns.To(_ => _.Name).Has(column, StringComparer.OrdinalIgnoreCase);

		public bool HasParameter(string parameter) =>
			this.Parameters.To(_ => _.Name).Has(parameter, StringComparer.OrdinalIgnoreCase);
	}
}
