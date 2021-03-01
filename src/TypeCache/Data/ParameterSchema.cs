// Copyright (c) 2021 Samuel Abraham

using System;
using System.Runtime.CompilerServices;
using TypeCache.Extensions;

namespace TypeCache.Data
{
	public sealed record ParameterSchema() : IEquatable<ParameterSchema>
	{
		public int Id { get; init; }

		public string Name { get; init; } = string.Empty;

		public bool Output { get; init; }

		public bool Return { get; init; }

		public bool Equals(ParameterSchema? other)
			=> this.Id == other?.Id && this.Name.Is(other.Name);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override int GetHashCode()
			=> base.GetHashCode();
	}
}
