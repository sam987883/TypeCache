// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace TypeCache.Web.Extensions;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddSqlApiControllers(this IServiceCollection @this)
		=> @this;
}
