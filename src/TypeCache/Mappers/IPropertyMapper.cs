// Copyright (c) 2021 Samuel Abraham

using System.Collections.Generic;
using System.Runtime.CompilerServices;
using static TypeCache.Default;

namespace TypeCache.Mappers;

public interface IPropertyMapper
{
	IEnumerable<string> Map(object from, object to);
}

public interface IPropertyMapper<in FROM, in TO> : IPropertyMapper
	where FROM : notnull
	where TO : notnull
{
	IEnumerable<string> Map(FROM from, TO to);

	/// <summary>
	/// <c><see langword="this"/>.Map((<typeparamref name="FROM"/>)<paramref name="from"/>, (<typeparamref name="TO"/>)<paramref name="to"/>)</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	IEnumerable<string> IPropertyMapper.Map(object from, object to)
		=> this.Map((FROM)from, (TO)to);
}
