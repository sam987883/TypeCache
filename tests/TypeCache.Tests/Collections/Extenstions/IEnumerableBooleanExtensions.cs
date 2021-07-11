// Copyright (c) 2021 Samuel Abraham

using System.Collections.Generic;
using TypeCache.Collections.Extensions;
using Xunit;

namespace TypeCache.Tests.Collections.Extensions
{
	public class IEnumerableBooleanExtensions
	{
		[Fact]
		public void All()
		{
			IEnumerable<bool> allTrue = new[] { true, true, true };
			IEnumerable<bool> someTrue = new[] { true, false, true };

			Assert.True(allTrue.All(true));
			Assert.False(allTrue.All(false));
			Assert.False(someTrue.All(true));
			Assert.False(someTrue.All(false));
		}

		[Fact]
		public void Any()
		{
			IEnumerable<bool> allTrue = new[] { true, true, true };
			IEnumerable<bool> someTrue = new[] { true, false, true };

			Assert.True(allTrue.Any(true));
			Assert.False(allTrue.Any(false));
			Assert.True(someTrue.Any(true));
			Assert.True(someTrue.Any(false));
		}
	}
}
