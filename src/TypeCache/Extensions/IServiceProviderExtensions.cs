// Copyright (c) 2021 Samuel Abraham

using Microsoft.Extensions.DependencyInjection;
using System;
using System.Runtime.CompilerServices;
using TypeCache.Reflection;

namespace TypeCache.Extensions
{
	public static class IServiceProviderExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ITypeCache GetTypeCache(this IServiceProvider @this)
            => @this.GetRequiredService<ITypeCache>();
    }
}
