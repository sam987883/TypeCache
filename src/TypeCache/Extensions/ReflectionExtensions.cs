// Copyright (c) 2021 Samuel Abraham

using System.Reflection;
using static System.Reflection.BindingFlags;

namespace TypeCache.Extensions;

public static partial class ReflectionExtensions
{
	private const char GENERIC_TICKMARK = '`';

	private const BindingFlags ALL_BINDING_FLAGS = FlattenHierarchy | IgnoreCase | Instance | NonPublic | Public | Static;
	private const BindingFlags INSTANCE_BINDING_FLAGS = FlattenHierarchy | IgnoreCase | Instance | NonPublic | Public;
	private const BindingFlags PUBLIC_INSTANCE_BINDING_FLAGS = FlattenHierarchy | IgnoreCase | Instance | Public;
	private const BindingFlags PUBLIC_STATIC_BINDING_FLAGS = FlattenHierarchy | IgnoreCase | Public | Static;
	private const BindingFlags STATIC_BINDING_FLAGS = FlattenHierarchy | IgnoreCase | NonPublic | Public | Static;

	private const string Item = nameof(Item);
	private const string Rest = nameof(Rest);

	private static T? GetMemberInfo<T>(this T[] @this, string name)
		where T : MemberInfo
	{
		var memberInfos = @this.Where(_ => _.Name.EqualsIgnoreCase(name)).ToArray();
		return memberInfos switch
		{
			{ Length: 0 } => null,
			{ Length: 1 } => memberInfos[0],
			_ => memberInfos.FirstOrDefault(_ => _.Name.EqualsOrdinal(name))
		};
	}

	private static MethodInfo[] GetMethodInfos(this MethodInfo[] @this, string name)
	{
		var methodInfos = @this.Where(_ => _.Name.EqualsIgnoreCase(name)).ToArray();
		var lookup = methodInfos.ToLookup(_ => _.Name, StringComparer.Ordinal);
		return lookup.Count switch
		{
			0 => [],
			1 => lookup.First().ToArray(),
			_ => lookup[name].ToArray()
		};
	}
}
