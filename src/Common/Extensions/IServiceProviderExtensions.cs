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
        public static T GetService<T>(this IServiceProvider @this) =>
            (T)@this.GetService(typeof(T));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object GetRequiredService(this IServiceProvider @this, Type serviceType) =>
            @this.GetService(serviceType) ?? throw new InvalidOperationException($"There is no service of type: {serviceType.FullName}.");

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T GetRequiredService<T>(this IServiceProvider @this) =>
            (T)@this.GetRequiredService(typeof(T));
    }
}
