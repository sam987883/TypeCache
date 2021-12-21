// Copyright (c) 2021 Samuel Abraham

using System.Runtime.CompilerServices;
using static TypeCache.Default;

namespace TypeCache.Mappers;

public interface IFieldMapper
{
	string[] Map(object from, object to);
}

public interface IFieldMapper<in FROM, in TO> : IFieldMapper
	where FROM : notnull
	where TO : notnull
{
	string[] Map(FROM from, TO to);

	/// <summary>
	/// <c><see langword="this"/>.Map((<typeparamref name="FROM"/>)<paramref name="from"/>, (<typeparamref name="TO"/>)<paramref name="to"/>)</c>
	/// </summary>
	[MethodImpl(METHOD_IMPL_OPTIONS)]
	string[] IFieldMapper.Map(object from, object to)
		=> this.Map((FROM)from, (TO)to);
}
