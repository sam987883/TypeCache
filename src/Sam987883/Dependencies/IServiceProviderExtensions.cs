// Copyright (c) 2020 Samuel Abraham

using Microsoft.Extensions.DependencyInjection;
using Sam987883.Common.Extensions;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Sam987883.Dependencies
{
	public static class IServiceProviderExtensions
    {
        public static (T Service, bool Exists) GetService<T>(this IServiceProvider @this, string name) =>
            @this.GetServices<INamedService<T>>().If(namedService => namedService.Name.Is(name)).To(namedService => namedService.Service).First();

        [return: MaybeNull]
        public static T GetService<T>(this IServiceProvider @this) =>
            (T)@this.GetService(typeof(T));
    }
}
