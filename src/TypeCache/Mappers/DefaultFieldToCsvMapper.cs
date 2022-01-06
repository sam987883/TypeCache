// Copyright (c) 2021 Samuel Abraham

using System.Runtime.CompilerServices;
using TypeCache.Collections.Extensions;
using TypeCache.Extensions;
using TypeCache.Mappers.Extensions;
using static TypeCache.Default;

namespace TypeCache.Mappers;

internal class DefaultFieldToCsvMapper<T> : IFieldToCsvMapper<T>
	where T : notnull
{
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public string[] Map(params T[] rows)
		=> rows.Map(row => TypeOf<T>.Fields.Values.Map(field => field.GetValue(row)).ToArray()).ToArray().ToCSV();
}
