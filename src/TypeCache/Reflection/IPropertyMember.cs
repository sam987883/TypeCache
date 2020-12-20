﻿// Copyright (c) 2021 Samuel Abraham

namespace TypeCache.Reflection
{
	public interface IPropertyMember : IMember
	{
		object? this[object instance] { get; set; }

		IMethodMember? GetMethod { get; }

		IMethodMember? SetMethod { get; }
	}
}