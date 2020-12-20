// Copyright (c) 2021 Samuel Abraham

using System.Collections.Immutable;

namespace TypeCache.Reflection
{
	public interface IConstructorCache<out T>
		where T : class
	{
		IImmutableList<IConstructorMember> Constructors { get; }

		T Create(params object[] parameters);
	}
}
