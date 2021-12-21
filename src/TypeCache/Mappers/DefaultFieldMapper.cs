// Copyright (c) 2021 Samuel Abraham

using System.Runtime.CompilerServices;
using TypeCache.Mappers.Extensions;
using static TypeCache.Default;

namespace TypeCache.Mappers;

internal class DefaultFieldMapper<FROM, TO> : IFieldMapper<FROM, TO>
	where FROM : notnull
	where TO : notnull
{
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public string[] Map(FROM from, TO to)
		=> (from, to).MapFields();
}
