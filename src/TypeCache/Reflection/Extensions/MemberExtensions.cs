// Copyright (c) 2021 Samuel Abraham

using System.Collections.Immutable;
using TypeCache.Collections.Extensions;

namespace TypeCache.Reflection.Extensions
{
	internal static class MemberExtensions
	{
		public static bool IsCallableWith(this IImmutableList<IParameter> @this, params object?[]? arguments)
		{
			if (arguments?.Length > 0)
			{
				var argumentEnumerator = arguments.ToCustomEnumerable().GetEnumerator();
				for (var i = 0; i < @this.Count; ++i)
				{
					var parameter = @this[i];
					if (argumentEnumerator.MoveNext())
					{
						if (argumentEnumerator.Current != null)
						{
							if (!parameter.Supports(argumentEnumerator.Current.GetType()))
								return false;
						}
						else if (!parameter.IsNullable)
							return false;
					}
					else if (!parameter.HasDefaultValue && !parameter.IsOptional)
						return false;
				}
				return !argumentEnumerator.MoveNext();
			}
			return @this.Count == 0 || @this.All(parameter => parameter!.HasDefaultValue || parameter.IsOptional);
		}
	}
}
