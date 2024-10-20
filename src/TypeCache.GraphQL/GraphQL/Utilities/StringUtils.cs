using System.Collections.Generic;
using System.Linq;
using System.Text;
using TypeCache.Extensions;
using TypeCache.Utilities;

namespace GraphQL.Utilities;

/// <summary>
/// Provides utility methods for strings.
/// </summary>
public static class StringUtils
{
	/// <summary>
	/// Given array of strings [ "A", "B", "C" ] return one string "'A', 'B' or 'C'".
	/// </summary>
	internal static string QuotedOrList(IEnumerable<string> items, int maxLength = 5)
	{
		var builder = new StringBuilder();
		var itemsArray = items.Take(maxLength).ToArray();
		for (var i = 0; i < itemsArray.Length; i++)
		{
			var index = i + 1;
			builder.Append(i switch
			{
				_ when index < itemsArray.Length => $"'{itemsArray[i]}', ",
				_ when index == itemsArray.Length => $"'{itemsArray[i]}' or ",
				_ => $"'{itemsArray[i]}'"
			});
		}

		return builder.ToString();
	}

	/// <summary>
	/// Given an invalid input string and a list of valid options, returns a filtered
	/// list of valid options sorted based on their similarity with the input.
	/// </summary>
	public static string[] SuggestionList(string input, IEnumerable<string>? options)
	{
		if (options is null)
			return [];

		var optionsByDistance = new Dictionary<string, int>();
		var inputThreshold = input.Length / 2;
		foreach (string option in options)
		{
			var distance = DamerauLevenshteinDistance(input, option, inputThreshold);
			var threshold = Math.Max(inputThreshold, Math.Max(option.Length / 2, 1));
			if (distance <= threshold)
				optionsByDistance[option] = distance;
		}

		return optionsByDistance.OrderBy(_ => _.Value).Select(_ => _.Key).ToArray();
	}

	/// <summary>
	/// Computes the Damerau-Levenshtein Distance between two strings, represented as arrays of
	/// integers, where each integer represents the code point of a character in the source string.
	/// Includes an optional threshold which can be used to indicate the maximum allowable distance.
	/// http://stackoverflow.com/a/9454016/279764
	/// </summary>
	/// <param name="source">An array of the code points of the first string</param>
	/// <param name="target">An array of the code points of the second string</param>
	/// <param name="threshold">Maximum allowable distance</param>
	/// <returns>Int.MaxValue if threshold exceeded; otherwise the Damerau-Levenshtein distance between the strings</returns>
	public static int DamerauLevenshteinDistance(string source, string target, int threshold)
	{
		var length1 = source.Length;
		var length2 = target.Length;

		// Return trivial case - difference in string lengths exceeds threshold
		if (Math.Abs(length1 - length2) > threshold)
			return int.MaxValue;

		// Ensure arrays [i] / length1 use shorter length
		if (length1 > length2)
		{
			(source, target) = (target, source);
			(length1, length2) = (length2, length1);
		}

		var maxi = length1;
		var maxj = length2;

		var dCurrent = new int[maxi + 1];
		var dMinus1 = new int[maxi + 1];
		var dMinus2 = new int[maxi + 1];
		int[] dSwap;

		for (var i = 0; i <= maxi; i++)
			dCurrent[i] = i;

		int jm1 = 0, im1, im2;

		for (var j = 1; j <= maxj; j++)
		{
			// Rotate
			dSwap = dMinus2;
			dMinus2 = dMinus1;
			dMinus1 = dCurrent;
			dCurrent = dSwap;

			// Initialize
			var minDistance = int.MaxValue;
			dCurrent[0] = j;
			im1 = 0;
			im2 = -1;

			for (var i = 1; i <= maxi; i++)
			{
				var cost = source[im1] == target[jm1] ? 0 : 1;

				var del = dCurrent[im1] + 1;
				var ins = dMinus1[i] + 1;
				var sub = dMinus1[im1] + cost;

				//Fastest execution for min value of 3 integers
				var min = (del > ins) ? (ins > sub ? sub : ins) : (del > sub ? sub : del);

				if (i > 1 && j > 1 && source[im2] == target[jm1] && source[im1] == target[j - 2])
					min = Math.Min(min, dMinus2[im2] + cost);

				dCurrent[i] = min;
				if (min < minDistance)
					minDistance = min;

				im1++;
				im2++;
			}

			jm1++;
			if (minDistance > threshold)
				return int.MaxValue;
		}

		var result = dCurrent[maxi];
		return result > threshold ? int.MaxValue : result;
	}
}
