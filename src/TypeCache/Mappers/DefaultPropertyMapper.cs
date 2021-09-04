// Copyright (c) 2021 Samuel Abraham

using System;
using System.Runtime.CompilerServices;
using TypeCache.Mappers.Extensions;

namespace TypeCache.Mappers
{
	internal class DefaultPropertyMapper<FROM, TO> : IPropertyMapper<FROM, TO>
		where FROM : notnull
		where TO : notnull
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Map(FROM from, TO to)
			=> (from, to).MapProperties(StringComparison.Ordinal);
	}
}
