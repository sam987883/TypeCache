// Copyright (c) 2020 Samuel Abraham

using System.Collections.Immutable;

namespace Sam987883.Reflection
{
	public interface IConstructorCache<T>
		where T : class
	{
		IImmutableList<IConstructorMember<T>> Constructors { get; }

		T Create(params object[] parameters);
	}
}
