// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Immutable;
using TypeCache.Extensions;

namespace TypeCache.Data
{
	public class ObjectSchema
	{
		/// <summary>
		/// The database object_id.
		/// </summary>
		public int Id { get; set; }

		public ObjectType Type { get; set; }

		/// <summary>
		/// Database table/view/function/procedure name.
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

		public string Name
		{
			get
			{
				if (this.DatabaseName.IsBlank() && this.SchemaName.IsBlank())
					return $"[{this.ObjectName}]";
				else if (this.DatabaseName.IsBlank())
					return $"[{this.SchemaName}].[{this.ObjectName}]";
				else if (this.SchemaName.IsBlank())
					return $"[{this.DatabaseName}]..[{this.ObjectName}]";
				else
					return $"[{this.DatabaseName}].[{this.SchemaName}].[{this.ObjectName}]";
			}
		}

		public bool HasColumn(string column) =>
			this.Columns.To(_ => _.Name).Has(column, StringComparer.OrdinalIgnoreCase);

		public bool HasParameter(string parameter) =>
			this.Parameters.To(_ => _.Name).Has(parameter, StringComparer.OrdinalIgnoreCase);
	}
}
