// Copyright (c) 2021 Samuel Abraham

using System.Runtime.CompilerServices;
using static TypeCache.Default;

namespace TypeCache.Mappers
{
	public interface IPropertyMapper
	{
		string[] Map(object from, object to);
	}

	public interface IPropertyMapper<in FROM, in TO> : IPropertyMapper
		where FROM : notnull
		where TO : notnull
	{
		string[] Map(FROM from, TO to);

		/// <summary>
		/// <see cref="IPropertyMapper{FROM, TO}"/>.Map((<typeparamref name="FROM"/>)<paramref name="from"/>, (<typeparamref name="TO"/>)<paramref name="to"/>)
		/// </summary>
		[MethodImpl(METHOD_IMPL_OPTIONS)]
		string[] IPropertyMapper.Map(object from, object to)
			=> Map((FROM)from, (TO)to);
	}
}
