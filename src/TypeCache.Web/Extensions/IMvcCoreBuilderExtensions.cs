// Copyright (c) 2021 Samuel Abraham

using Microsoft.Extensions.DependencyInjection;
using TypeCache.Web.Controllers;

namespace TypeCache.Web.Extensions;

public static class IMvcCoreBuilderExtensions
{
	/// <summary>
	/// Adds the TypeCache.Web assembly as an Application Part to MVC.
	/// </summary>
	public static IMvcCoreBuilder AddSqlApiControllers(this IMvcCoreBuilder @this)
		=> @this.AddApplicationPart(typeof(CommandController).Assembly);
}
