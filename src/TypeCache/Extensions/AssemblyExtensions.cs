using System.Reflection;

namespace TypeCache.Extensions;

public static class AssemblyExtensions
{
	public static string GetNuGetVersion(this Assembly assembly)
	{
		var version = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;
		return version?.LastIndexOf('+') switch
		{
			null => string.Empty,
			-1 => version,
			int i => version[0..i]
		};
	}
}
