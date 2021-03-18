// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using TypeCache.Collections.Extensions;
using TypeCache.Extensions;

namespace TypeCache.Data
{
	public sealed record ObjectSchema(int Id, ObjectType Type, string DatabaseName, string SchemaName, string ObjectName,
		IImmutableList<ColumnSchema> Columns, IImmutableList<ParameterSchema> Parameters) : IEquatable<ObjectSchema>
	{
		/// <summary>
		/// The fully qualified database object name.
		/// </summary>
		public string Name { get; init; } = $"[{DatabaseName}].[{SchemaName}].[{ObjectName}]";

		public bool HasColumn(string column) =>
			this.Columns.To(_ => _.Name).Has(column, false);

		public bool HasParameter(string parameter) =>
			this.Parameters.To(_ => _.Name).Has(parameter, false);

		public bool Equals(ObjectSchema? other)
			=> this.Id == other?.Id && this.Name.Is(other.Name);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override int GetHashCode()
			=> HashCode.Combine(this.Id, this.Name);
	}
}
