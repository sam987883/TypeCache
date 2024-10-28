// Copyright (c) 2021 Samuel Abraham

using System.Reflection;
using static System.Reflection.BindingFlags;

namespace TypeCache.Extensions;

public static partial class ReflectionExtensions
{
	private const char GENERIC_TICKMARK = '`';

	private const BindingFlags BINDING_FLAGS = FlattenHierarchy | IgnoreCase | Instance | NonPublic | Public | Static;
	private const BindingFlags INSTANCE_BINDING_FLAGS = Instance | NonPublic | Public;
	private const BindingFlags STATIC_BINDING_FLAGS = FlattenHierarchy | IgnoreCase | NonPublic | Public | Static;

	private const string Item = nameof(Item);
	private const string Rest = nameof(Rest);
}
