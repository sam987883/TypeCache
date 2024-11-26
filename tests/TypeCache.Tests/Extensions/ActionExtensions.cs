// Copyright (c) 2021 Samuel Abraham

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Time.Testing;
using TypeCache.Extensions;
using TypeCache.Utilities;
using Xunit;

namespace TypeCache.Tests.Extensions;

public class ActionExtensions
{
	[Fact]
	public async Task Retry()
	{
		var attempt = 0;
		var action = () =>
		{
			if (++attempt < 4)
				throw new Exception();

			Console.WriteLine($"Action.{nameof(Retry)} Attempt #: {attempt}");
		};

		var timeProvider = new FakeTimeProvider();

		var task = action.Retry(Sequence.LinearTime(1.Seconds(), 10), timeProvider);
		while (!task.IsCompleted)
			timeProvider.Advance(1.Seconds());

		await task;
	}
}
