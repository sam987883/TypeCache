// Copyright (c) 2020 Samuel Abraham

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Sam987883.Common.Extensions
{
    public static class IServiceProviderExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: MaybeNull]
        public static T GetService<T>(this IServiceProvider @this)
            => (T)@this.GetService(typeof(T));

        public static object GetRequiredService(this IServiceProvider @this, Type serviceType)
		{
            var service = @this.GetService(serviceType);
            service.AssertNotNull(nameof(service));
            return service;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T GetRequiredService<T>(this IServiceProvider @this)
            => (T)@this.GetRequiredService(typeof(T));
    }
}
