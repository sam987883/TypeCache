﻿// Copyright (c) 2020 Samuel Abraham

using Microsoft.Extensions.DependencyInjection;
using sam987883.Extensions;
using System;

namespace sam987883.Dependencies
{
    public static class IServiceProviderExtensions
    {
        public static (T Service, bool Exists) GetService<T>(this IServiceProvider @this, string name) =>
            @this.GetServices<INamedService<T>>().If(namedService => namedService.Name.Is(name)).To(namedService => namedService.Service).First();
    }
}