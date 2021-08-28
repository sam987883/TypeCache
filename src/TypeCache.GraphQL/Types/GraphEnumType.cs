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
			this.Name = TypeOf<T>.Member.GraphName();

			EnumOf<T>.Tokens.Values.If(token => !token.GraphIgnore()).Do(token =>
			{
				var name = token.GraphName();
				var description = token.GraphDescription();
				var deprecationReason = token.ObsoleteMessage();

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
