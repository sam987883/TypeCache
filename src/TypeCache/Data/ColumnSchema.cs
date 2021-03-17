// Copyright (c) 2021 Samuel Abraham

using System;
using System.Data;
using System.Runtime.CompilerServices;
using TypeCache.Extensions;
using TypeCache.Collections.Extensions;

namespace TypeCache.Data
{
	public sealed record ColumnSchema() : IEquatable<ColumnSchema>
	{
		public int Id { get; init; }

		public string Name { get; init; } = string.Empty;

		public SqlDbType Type { get; init; }

		public bool Hidden { get; init; }

		public bool Identity { get; init; }

		public bool Nullable { get; init; }

		public bool PrimaryKey { get; init; }

		public bool Readonly { get; init; }

		public int Length { get; init; }

		public bool Equals(ColumnSchema? other)
			=> this.Id == other?.Id && this.Name.Is(other.Name) && this.Type == other.Type;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override int GetHashCode()
			=> HashCode.Combine(this.Id, this.Name, this.Type);
	}
}
