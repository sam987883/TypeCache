// Copyright (c) 2021 Samuel Abraham

using System.Runtime.CompilerServices;
using TypeCache.Mappers.Extensions;

namespace TypeCache.Mappers
{
	internal class DefaultFieldMapper<FROM, TO> : IFieldMapper<FROM, TO>
		where FROM : notnull
		where TO : notnull
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Map(FROM from, TO to)
			=> (from, to).MapFields();
	}
}
