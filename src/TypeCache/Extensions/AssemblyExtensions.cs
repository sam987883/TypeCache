using System.Reflection;

namespace TypeCache.Extensions;

public static class AssemblyExtensions
{
	extension(Assembly @this)
	{
		public string GetNuGetVersion()
		{
			var version = @this.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;
			return version?.LastIndexOf('+') switch
			{
				null => string.Empty,
				-1 => version,
				int i => version[0..i]
			};
		}
	}
}
