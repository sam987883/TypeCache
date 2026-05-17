using System.Reflection;

namespace TypeCache.Extensions;

public static class AssemblyExtensions
{
	extension(Assembly @this)
	{
		public string? InformationalVersion
			=> @this.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;

		public string NuGetVersion
		{
			get
			{
				var informationalVersion = @this.InformationalVersion;
				return informationalVersion?.LastIndexOf('+') switch
				{
					null => string.Empty,
					-1 => informationalVersion,
					int i => informationalVersion[0..i]
				};
			}
		}
	}
}
