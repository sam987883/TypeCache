// Copyright (c) 2021 Samuel Abraham

using System.Numerics;
using System.Text;
using GraphQL.Types;
using TypeCache.Extensions;
using TypeCache.GraphQL.Types;

namespace TypeCache.GraphQL.Extensions;

public static class TypeScriptExtensions
{
	public static string GetTypeScriptType(this IGraphType @this, bool list = false)
		=> @this switch
		{
			NonNullGraphType graphType => graphType.ResolvedType!.GetTypeScriptType(list),
			ListGraphType graphType => graphType.ResolvedType!.GetTypeScriptType(true),
			GraphQLScalarType<bool> when list => "boolean[]",
			GraphQLScalarType<bool> => "boolean",
			GraphQLScalarType<sbyte> or GraphQLScalarType<short> or GraphQLScalarType<int> or GraphQLScalarType<byte> or GraphQLScalarType<ushort>
			or GraphQLScalarType<Half> or GraphQLScalarType<float> or GraphQLScalarType<double> or GraphQLScalarType<decimal> when list => "number[]",
			GraphQLScalarType<sbyte> or GraphQLScalarType<short> or GraphQLScalarType<int> or GraphQLScalarType<byte> or GraphQLScalarType<ushort>
			or GraphQLScalarType<Half> or GraphQLScalarType<float> or GraphQLScalarType<double> or GraphQLScalarType<decimal> => "number",
			GraphQLScalarType<int> or GraphQLScalarType<long> or GraphQLScalarType<ulong> or GraphQLScalarType<BigInteger> when list => "bigint[]",
			GraphQLScalarType<int> or GraphQLScalarType<long> or GraphQLScalarType<ulong> or GraphQLScalarType<BigInteger> => "bigint",
			GraphQLScalarType<string> or IdGraphType or GuidGraphType or GraphQLScalarType<DateOnly> or GraphQLScalarType<DateTime> or GraphQLScalarType<DateTimeOffset> or GraphQLUriType when list => "string[]",
			GraphQLScalarType<string> or IdGraphType or GuidGraphType or GraphQLScalarType<DateOnly> or GraphQLScalarType<DateTime> or GraphQLScalarType<DateTimeOffset> or GraphQLUriType => "string",
			_ when list => Invariant($"{@this.Name}[]"),
			_ => @this.Name
		};

	public static string ToTypeScript(this EnumerationGraphType @this)
	{
		var builder = new StringBuilder();
		if (@this.Description.IsNotBlank())
			builder.AppendLine(Invariant($"/* {@this.Description} */"));
		builder.Append("export enum ").AppendLine(@this.Name).Append('{');
		@this.Values.ToArray().ForEach(value =>
		{
			builder.AppendLine().Append('\t').Append(value.Name);
			if (value.Value is not null && long.TryParse(value.Value.ToString(), out var id))
				builder.Append(" = ").Append(id);
		}, () => builder.Append(',').AppendLine());
		return builder.AppendLine().Append('}').AppendLine().ToString();
	}

	public static string ToTypeScript(this IComplexGraphType @this)
	{
		var builder = new StringBuilder();
		if (@this.Description.IsNotBlank())
			builder.AppendLine(Invariant($"/* {@this.Description} */"));
		builder.AppendLine(Invariant($"export type {@this.Name} = {{"));
		@this.Fields.ToArray().ForEach(field => builder.AppendLine(Invariant($"\t{field.Name}: {field.ResolvedType!.GetTypeScriptType()};")));
		return builder.Append('}').AppendLine().ToString();
	}
}
