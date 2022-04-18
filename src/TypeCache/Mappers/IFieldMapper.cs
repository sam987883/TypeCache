// Copyright (c) 2021 Samuel Abraham

using System.Collections.Generic;
using System.Runtime.CompilerServices;
using static TypeCache.Default;

namespace TypeCache.Mappers;

public interface IFieldMapper
{
	IEnumerable<string> Map(object from, object to);
}

public interface IFieldMapper<in FROM, in TO> : IFieldMapper
	where FROM : notnull
	where TO : notnull
{
	IEnumerable<string> Map(FROM from, TO to);

	/// <summary>
	/// <c><see langword="this"/>.Map((<typeparamref name="FROM"/>)<paramref name="from"/>, (<typeparamref name="TO"/>)<paramref name="to"/>)</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	IEnumerable<string> IFieldMapper.Map(object from, object to)
		=> this.Map((FROM)from, (TO)to);
}
