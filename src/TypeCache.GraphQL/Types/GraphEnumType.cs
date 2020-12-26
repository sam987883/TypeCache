// Copyright (c) 2021 Samuel Abraham

using GraphQL.Types;
using System;
using System.Runtime.CompilerServices;
using TypeCache.Extensions;
using TypeCache.GraphQL.Attributes;
using TypeCache.Reflection;

namespace TypeCache.GraphQL.Types
{
	public sealed class GraphEnumType<T> : EnumerationGraphType<T> where T : struct, Enum
	{
		private readonly Func<string, string> _ChangeEnumCase;

		public GraphEnumType(IEnumCache<T> enumCache, Func<string, string> changeEnumCase = null)
		{
			var graphAttribute = enumCache.Attributes.First<object, GraphAttribute>();
			this.Name = graphAttribute?.Name ?? enumCache.Name;
			this._ChangeEnumCase = changeEnumCase ?? this.DefaultChangeEnumCase;

			foreach (var field in enumCache.Fields)
			{
				graphAttribute = field.Attributes.First<Attribute, GraphAttribute>();
				if (graphAttribute.Ignore)
					continue;

				var name = graphAttribute?.Name ?? field.Name;
				var description = graphAttribute?.Description;
				var deprecationReason = field.Attributes.First<Attribute, ObsoleteAttribute>()?.Message;

				this.AddValue(name, description, field.Value, deprecationReason);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected override string ChangeEnumCase(string value)
			=> this._ChangeEnumCase(value);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private string DefaultChangeEnumCase(string value)
			=> value;
	}
}
