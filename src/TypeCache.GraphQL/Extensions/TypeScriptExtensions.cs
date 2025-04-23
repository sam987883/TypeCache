// Copyright (c) 2021 Samuel Abraham

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
			BooleanGraphType when list => "boolean[]",
			BooleanGraphType => "boolean",
			SByteGraphType or ShortGraphType or IntGraphType or ByteGraphType or UShortGraphType or HalfGraphType or FloatGraphType or DecimalGraphType when list => "number[]",
			SByteGraphType or ShortGraphType or IntGraphType or ByteGraphType or UShortGraphType or HalfGraphType or FloatGraphType or DecimalGraphType => "number",
			BigIntGraphType when list => "bigint[]",
			BigIntGraphType => "bigint",
			StringGraphType or IdGraphType or GuidGraphType or DateOnlyGraphType or DateTimeGraphType or DateTimeOffsetGraphType or UriGraphType when list => "string[]",
			StringGraphType or IdGraphType or GuidGraphType or DateOnlyGraphType or DateTimeGraphType or DateTimeOffsetGraphType or UriGraphType => "string",
			_ when list => Invariant($"{@this.Name}[]"),
			_ => @this.Name
		};

	public static string ToTypeScript(this EnumerationGraphType @this)
	{
		var builder = new StringBuilder();
		if (@this.Description.IsNotBlank())
			builder.Append("// ").AppendLine(@this.Description);
		builder.Append("export enum ").AppendLine(@this.Name).Append('{');
		@this.Values.ForEach(value =>
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
			builder.Append("// ").AppendLine(@this.Description);
		builder.AppendLine(Invariant($"export type {@this.Name} = {{"));
		@this.Fields.ForEach(field => builder.AppendLine(Invariant($"\t{field.Name}: {field.ResolvedType!.GetTypeScriptType()};")));
		return builder.Append('}').AppendLine().ToString();
	}
}
