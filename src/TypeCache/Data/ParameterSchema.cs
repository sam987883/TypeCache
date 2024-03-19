// Copyright (c) 2021 Samuel Abraham

using System.Data;
using TypeCache.Extensions;

namespace TypeCache.Data;

public sealed class ParameterSchema(string name, ParameterDirection direction) : IEquatable<ParameterSchema>
{
	public string Name { get; } = name;

	public ParameterDirection Direction { get; } = direction;

	public bool Equals(ParameterSchema? other)
		=> this.Name.Is(other?.Name);

	public override bool Equals(object? other)
		=> this.Equals(other as ParameterSchema);

	public override int GetHashCode()
		=> this.Name.GetHashCode();
}
