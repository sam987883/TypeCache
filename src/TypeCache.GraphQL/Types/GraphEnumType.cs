// Copyright (c) 2021 Samuel Abraham

using System;
using System.Runtime.CompilerServices;
using GraphQL.Types;
using TypeCache.Collections.Extensions;
using TypeCache.GraphQL.Attributes;

namespace TypeCache.GraphQL.Types
{
	public sealed class GraphEnumType<T> : EnumerationGraphType<T> where T : struct, Enum
	{
		private readonly Func<string, string> _ChangeEnumCase = DefaultChangeEnumCase;

		public GraphEnumType()
		{
			var enumGraphAttribute = Enum<T>.Attributes.First<GraphAttribute>();
			this.Name = enumGraphAttribute?.Name ?? Enum<T>.Name;

			Enum<T>.Tokens.Values.If(token => !token!.Attributes.Any<GraphIgnoreAttribute>()).Do(token =>
			{
				var tokenGraphAttribute = token!.Attributes.First<GraphAttribute>();
				var name = token.Name;
				var description = tokenGraphAttribute?.Description;
				var deprecationReason = token.Attributes.First<ObsoleteAttribute>()?.Message;

				this.AddValue(name, description, token, deprecationReason);
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
