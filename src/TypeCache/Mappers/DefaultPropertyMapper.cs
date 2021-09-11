// Copyright (c) 2021 Samuel Abraham

using System.Runtime.CompilerServices;
using TypeCache.Mappers.Extensions;
using static TypeCache.Default;

namespace TypeCache.Mappers
{
	internal class DefaultPropertyMapper<FROM, TO> : IPropertyMapper<FROM, TO>
		where FROM : notnull
		where TO : notnull
	{
		[MethodImpl(METHOD_IMPL_OPTIONS)]
		public void Map(FROM from, TO to)
			=> (from, to).MapProperties();
	}
}
