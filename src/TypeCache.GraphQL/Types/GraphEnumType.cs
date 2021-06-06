// Copyright (c) 2021 Samuel Abraham

using System;
using System.Runtime.CompilerServices;
using GraphQL.Types;
using TypeCache.Collections.Extensions;
using TypeCache.GraphQL.Attributes;
using TypeCache.GraphQL.Extensions;

namespace TypeCache.GraphQL.Types
{
	public sealed class GraphEnumType<T> : EnumerationGraphType<T> where T : struct, Enum
	{
		private readonly Func<string, string> _ChangeEnumCase = DefaultChangeEnumCase;

#nullable disable

		public GraphEnumType()
		{
			var enumGraphAttribute = EnumOf<T>.Attributes.First<GraphNameAttribute>();
			this.Name = enumGraphAttribute?.Name ?? EnumOf<T>.Name;

			EnumOf<T>.Tokens.Values.If(token => !token.Attributes.Any<GraphIgnoreAttribute>()).Do(token =>
			{
				var name = token.GetGraphName();
				var description = token.GetGraphDescription();
				var deprecationReason = token.Attributes.First<ObsoleteAttribute>()?.Message;

				this.AddValue(name, description, token, deprecationReason);
			});
		}

#nullable enable

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected override string ChangeEnumCase(string value)
			=> this._ChangeEnumCase(value);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static string DefaultChangeEnumCase(string value)
			=> value;
	}
}
