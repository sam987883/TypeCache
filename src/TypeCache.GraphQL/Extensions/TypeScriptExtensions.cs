using System;
using System.Text;
using GraphQL.Types;
using TypeCache.Collections.Extensions;
using TypeCache.Extensions;
using TypeCache.GraphQL.Extensions;
using TypeCache.GraphQL.Types;
using TypeCache.Reflection.Extensions;

namespace TypeCache.GraphQL.Extensions
{
	public static class TypeScriptExtensions
	{
		public static string GetTypeScriptType(this IGraphType @this, bool list = false)
			=> @this switch
			{
				NonNullGraphType graphType => graphType.ResolvedType!.GetTypeScriptType(list),
				ListGraphType graphType => graphType.ResolvedType!.GetTypeScriptType(true),
				BooleanGraphType when list => "boolean[]",
				BooleanGraphType => "boolean",
				SByteGraphType or ShortGraphType or IntGraphType or ByteGraphType or UShortGraphType
				or FloatGraphType or DecimalGraphType or TimeSpanMillisecondsGraphType or TimeSpanSecondsGraphType when list => "number[]",
				SByteGraphType or ShortGraphType or IntGraphType or ByteGraphType or UShortGraphType
				or FloatGraphType or DecimalGraphType or TimeSpanMillisecondsGraphType or TimeSpanSecondsGraphType => "number",
				UIntGraphType or LongGraphType or ULongGraphType or BigIntGraphType when list => "bigint[]",
				UIntGraphType or LongGraphType or ULongGraphType or BigIntGraphType => "bigint",
				StringGraphType or IdGraphType or GuidGraphType or DateGraphType or DateTimeGraphType or DateTimeOffsetGraphType or UriGraphType when list => "string[]",
				StringGraphType or IdGraphType or GuidGraphType or DateGraphType or DateTimeGraphType or DateTimeOffsetGraphType or UriGraphType => "string",
				_ when list => $"{@this.Name}[]",
				_ => @this.Name
			};

		public static string ToTypeScript(this EnumerationGraphType @this)
		{
			var builder = new StringBuilder();
			if (!@this.Description.IsBlank())
				builder.AppendLine("/*").AppendLine(@this.Description).AppendLine("*/");
			builder.Append("export enum ").AppendLine(@this.Name).Append('{');
			@this.Values.Do(value =>
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
			if (!@this.Description.IsBlank())
				builder.AppendLine("/*").AppendLine(@this.Description).AppendLine("*/");
			builder.AppendLine($"export type {@this.Name} = {{");
			@this.Fields.Do(field => builder.AppendLine($"\t{field.Name}: {field.ResolvedType!.GetTypeScriptType()};"));
			return builder.Append('}').AppendLine().ToString();
		}
	}
}
