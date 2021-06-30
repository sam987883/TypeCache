// Copyright (c) 2021 Samuel Abraham

using System;
using System.Runtime.CompilerServices;
using GraphQL.Types;
using TypeCache.Collections.Extensions;
using TypeCache.GraphQL.Extensions;

namespace TypeCache.GraphQL.Types
{
	public sealed class GraphEnumType<T> : EnumerationGraphType<T> where T : struct, Enum
	{
		private readonly Func<string, string> _ChangeEnumCase = DefaultChangeEnumCase;

		public GraphEnumType()
		{
			this.Name = EnumOf<T>.Attributes.GraphName() ?? EnumOf<T>.Name;

			EnumOf<T>.Tokens.Values.If(token => !token.Attributes.GraphIgnore()).Do(token =>
			{
				var name = token.Attributes.GraphName() ?? token.Name;
				var description = token.Attributes.GraphDescription();
				var deprecationReason = token.Attributes.ObsoleteMessage();

				this.AddValue(name, description, token.Value, deprecationReason);
			});
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected override string ChangeEnumCase(string value)
			=> this._ChangeEnumCase(value);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static string DefaultChangeEnumCase(string value)
			=> value;
	}
}
