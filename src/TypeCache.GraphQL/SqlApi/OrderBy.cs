// Copyright (c) 2021 Samuel Abraham

using TypeCache.Data;
using TypeCache.Data.Extensions;
using static System.FormattableString;

namespace TypeCache.GraphQL.SqlApi;

public readonly record struct OrderBy(string Expression, Sort Sort)
{
	public string Display { get; } = Invariant($"{Expression}_{Sort.ToSQL()}");

	public override string ToString()
		=> Invariant($"{this.Expression} {this.Sort.ToSQL()}");
}
