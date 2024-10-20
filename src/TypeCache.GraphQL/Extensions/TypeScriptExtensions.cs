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
			GraphQLBooleanType when list => "boolean[]",
			GraphQLBooleanType => "boolean",
			GraphQLNumberType<sbyte> or GraphQLNumberType<short> or GraphQLNumberType<int> or GraphQLNumberType<byte> or GraphQLNumberType<ushort>
			or GraphQLNumberType when list => "number[]",
			GraphQLNumberType<sbyte> or GraphQLNumberType<short> or GraphQLNumberType<int> or GraphQLNumberType<byte> or GraphQLNumberType<ushort>
			or GraphQLNumberType => "number",
			GraphQLNumberType<int> or GraphQLNumberType<long> or GraphQLNumberType<ulong> or GraphQLNumberType<BigInteger> when list => "bigint[]",
			GraphQLNumberType<int> or GraphQLNumberType<long> or GraphQLNumberType<ulong> or GraphQLNumberType<BigInteger> => "bigint",
			GraphQLStringType or IdGraphType or GraphQLStringType<Guid> or GraphQLUriType
			or GraphQLStringType<DateOnly> or GraphQLStringType<DateTime> or GraphQLStringType<DateTimeOffset> when list => "string[]",
			GraphQLStringType or IdGraphType or GraphQLStringType<Guid> or GraphQLUriType
			or GraphQLStringType<DateOnly> or GraphQLStringType<DateTime> or GraphQLStringType<DateTimeOffset> => "string",
			_ when list => Invariant($"{@this.Name}[]"),
			_ => @this.Name
		};

	public static string ToTypeScript(this GraphQLEnumType @this)
	{
		var builder = new StringBuilder();
		if (@this.Description.IsNotBlank())
			builder.AppendLine(Invariant($"/* {@this.Description} */"));
		builder.Append("export enum ").AppendLine(@this.Name).Append('{');
		@this.Values.ToArray().ForEach(pair =>
		{
			builder.AppendLine().Append('\t').Append(pair.Value.Name);
			if (pair.Value.Value is not null && long.TryParse(pair.Value.Value.ToString(), out var id))
				builder.Append(" = ").Append(id);
		}, () => builder.Append(',').AppendLine());
		return builder.AppendLine().Append('}').AppendLine().ToString();
	}

	public static string ToTypeScript(this IObjectGraphType @this)
	{
		var builder = new StringBuilder();
		if (@this.Description.IsNotBlank())
			builder.AppendLine(Invariant($"/* {@this.Description} */"));
		builder.AppendLine(Invariant($"export type {@this.Name} = {{"));
		@this.Fields.ToArray().ForEach(field => builder.AppendLine(Invariant($"\t{field.Name}: {field.ResolvedType!.GetTypeScriptType()};")));
		return builder.Append('}').AppendLine().ToString();
	}
}
