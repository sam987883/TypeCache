// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using TypeCache.Collections.Extensions;
using TypeCache.Extensions;

namespace TypeCache.Data
{
	public sealed record ObjectSchema() : IEquatable<ObjectSchema>
	{
		/// <summary>
		/// The database unique object_id.
		/// </summary>
		public int Id { get; set; }

		public ObjectType Type { get; set; }

		/// <summary>
		/// The database table/view/function/procedure name.
		/// </summary>
		public string ObjectName { get; set; } = string.Empty;

		/// <summary>
		/// The database schema name ie. dbo.
		/// </summary>
		public string SchemaName { get; set; } = string.Empty;

		/// <summary>
		/// The database name - pulled from the connection string.
		/// </summary>
		public string DatabaseName { get; set; } = string.Empty;

		public IImmutableList<ColumnSchema> Columns { get; init; } = ImmutableArray<ColumnSchema>.Empty;

		public IImmutableList<ParameterSchema> Parameters { get; init; } = ImmutableArray<ParameterSchema>.Empty;

		/// <summary>
		/// The fully qualified database object name.
		/// </summary>
		public string Name => this.ObjectName switch
		{
			_ when this.DatabaseName.IsBlank() && this.SchemaName.IsBlank() => $"[{this.ObjectName}]",
			_ when this.DatabaseName.IsBlank() => $"[{this.SchemaName}].[{this.ObjectName}]",
			_ when this.SchemaName.IsBlank() => $"[{this.DatabaseName}]..[{this.ObjectName}]",
			_ => $"[{this.DatabaseName}].[{this.SchemaName}].[{this.ObjectName}]"
		};

		public bool HasColumn(string column) =>
			this.Columns.To(_ => _.Name).Has(column, false);

		public bool HasParameter(string parameter) =>
			this.Parameters.To(_ => _.Name).Has(parameter, false);

		public bool Equals(ObjectSchema? other)
			=> this.Id == other?.Id && this.Name.Is(other.Name);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override int GetHashCode()
			=> base.GetHashCode();
	}
}
