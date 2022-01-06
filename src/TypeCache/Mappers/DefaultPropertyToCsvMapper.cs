// Copyright (c) 2021 Samuel Abraham

using System.Runtime.CompilerServices;
using TypeCache.Collections.Extensions;
using TypeCache.Extensions;
using TypeCache.Mappers.Extensions;
using static TypeCache.Default;

namespace TypeCache.Mappers;

internal class DefaultPropertyToCsvMapper<T> : IFieldToCsvMapper<T>
	where T : notnull
{
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public string[] Map(params T[] rows)
		=> rows.Map(row => TypeOf<T>.Properties.Values.Map(property => property.GetValue(row)).ToArray()).ToArray().ToCSV();
}
