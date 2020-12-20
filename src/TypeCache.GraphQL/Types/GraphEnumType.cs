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

        private readonly IEnumCache<T> _EnumCache;

        public GraphEnumType(IEnumCache<T> enumCache, Func<string, string> changeEnumCase = null)
        {
            this.Name = enumCache.Name;
            this._ChangeEnumCase = changeEnumCase ?? new Func<string, string>(value => value);
            this._EnumCache = enumCache;

            var fields = this._EnumCache.Fields.If(field => !field.Attributes.Any<Attribute, GraphIgnoreAttribute>());
            foreach (var field in fields)
            {
                var description = field.Attributes.First<Attribute, GraphDescriptionAttribute>()?.Description;
                var deprecationReason = field.Attributes.First<Attribute, ObsoleteAttribute>()?.Message;

                this.AddValue(field.Name, description, field.Value, deprecationReason);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override string ChangeEnumCase(string value)
            => this._ChangeEnumCase(value);
    }
}
