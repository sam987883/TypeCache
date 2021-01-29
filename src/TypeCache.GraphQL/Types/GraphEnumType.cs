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
			var graphAttribute = Enum<T>.Attributes.First<Attribute, GraphAttribute>();
			this.Name = graphAttribute?.Name ?? Enum<T>.Name;

			Enum<T>.Tokens.Do(token =>
			{
				graphAttribute = token.Attributes.First<Attribute, GraphAttribute>();
				if (graphAttribute?.Ignore != true)
					return;

				var name = token.Name;
				var description = graphAttribute?.Description;
				var deprecationReason = token.Attributes.First<Attribute, ObsoleteAttribute>()?.Message;

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
