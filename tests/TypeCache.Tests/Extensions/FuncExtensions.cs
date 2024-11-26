// Copyright (c) 2021 Samuel Abraham

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Time.Testing;
using NSubstitute;
using TypeCache.Extensions;
using TypeCache.Utilities;
using Xunit;
using static System.Collections.Specialized.BitVector32;

namespace TypeCache.Tests.Extensions;

public class FuncExtensions
{
	[Fact]
	public async Task Retry()
	{
		var expectedValue = 22;
		var attempt = 0;
		var func = () =>
		{
			if (++attempt < 4)
				throw new Exception();

			Console.WriteLine($"Func {nameof(Retry)} Attempt #: {attempt}");
			return expectedValue;
		};

		var timeProvider = new FakeTimeProvider();

		var task = func.Retry(Sequence.LinearTime(1.Seconds(), 10), timeProvider);
		while (!task.IsCompleted)
			timeProvider.Advance(1.Seconds());

		var actualValue = await task;
		Assert.Equal(expectedValue, actualValue);
	}
}
