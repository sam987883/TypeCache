// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using TypeCache.Collections;
using TypeCache.Collections.Extensions;
using TypeCache.Extensions;
using Xunit;

namespace TypeCache.Tests.Collections.Extensions;

public class ImmutableArrayExtensions
{
	[Fact]
	public void Deconstruct()
	{
		{
			(int? item1, IEnumerable<int> rest) = new[] { 1, 2 }.ToImmutableArray();
			Assert.Equal(1, item1);
			Assert.Equal(new[] { 2 }, rest);
		}
		{
			(int? item1, int? item2, IEnumerable<int> rest) = new[] { 1, 2 }.ToImmutableArray();
			Assert.Equal(1, item1);
			Assert.Equal(2, item2);
			Assert.Equal(Array<int>.Empty, rest);
		}
		{
			(int? item1, int? item2, int? item3, IEnumerable<int> rest) = new[] { 1, 2 }.ToImmutableArray();
			Assert.Equal(1, item1);
			Assert.Equal(2, item2);
			Assert.Null(item3);
			Assert.Equal(Array<int>.Empty, rest);
		}
		{
			(string item1, IEnumerable<string> rest) = new[] { "1", "2" }.ToImmutableArray();
			Assert.Equal("1", item1);
			Assert.Equal(new[] { "2" }, rest);
		}
		{
			(string item1, string item2, IEnumerable<string> rest) = new[] { "1", "2" }.ToImmutableArray();
			Assert.Equal("1", item1);
			Assert.Equal("2", item2);
			Assert.Equal(Array<string>.Empty, rest);
		}
		{
			(string item1, string item2, string item3, IEnumerable<string> rest) = new[] { "1", "2" }.ToImmutableArray();
			Assert.Equal("1", item1);
			Assert.Equal("2", item2);
			Assert.Null(item3);
			Assert.Equal(Array<string>.Empty, rest);
		}
	}

	[Fact]
	public void Do()
	{
		var stringArray = new[] { "123", "abc", "def" }.ToImmutableArray();

		stringArray.Do(value => Assert.Contains(value, stringArray));
		Assert.Throws<ArgumentNullException>(() => stringArray.Do(null as Action<string>));
		stringArray.Do((value, i) =>
		{
			Assert.Contains(value, stringArray);
			Assert.Contains(i, new[] { 0, 1, 2 });
		});
		Assert.Throws<ArgumentNullException>(() => stringArray.Do(null as Action<string, int>));

		var i = 0;
		stringArray.Do(value => Assert.Contains(value, stringArray), () => ++i);
		Assert.Throws<ArgumentNullException>(() => stringArray.Do(null as Action<string>, () => ++i));
		Assert.Throws<ArgumentNullException>(() => stringArray.Do(value => Assert.Contains(value, stringArray), null));
		Assert.Equal(2, i);

		stringArray.Do((value, i) =>
		{
			Assert.Contains(value, stringArray);
			Assert.Contains(stringArray[i], stringArray);
			Assert.Contains(i, new[] { 0, 1, 2 });
		}, () => ++i);
		Assert.Equal(4, i);
		Assert.Throws<ArgumentNullException>(() => stringArray.Do(null as Action<string, int>, () => ++i));
		Assert.Throws<ArgumentNullException>(() => stringArray.Do((value, i) =>
		{
			Assert.Contains(value, stringArray);
			Assert.Contains(stringArray[i], stringArray);
			Assert.Contains(i, new[] { 0, 1, 2 });
		}, null));
	}

	[Fact]
	public void Get()
	{
		var intArray = new[] { 1, 2, 3, 4, 5, 6 }.ToImmutableArray();

		Assert.Equal(new[] { 3, 4 }, intArray.Get(2..4));
		Assert.Equal(new[] { 4, 3 }, intArray.Get(3..1));
		Assert.Empty(intArray.Get(2..2));
		Assert.Equal(new[] { 3, 4, 5 }, intArray.Get(2..^1));
		Assert.Throws<ArgumentOutOfRangeException>(() => intArray.Get(^0..0).ToArray());
		Assert.Equal(new[] { 1, 2, 3, 4, 5, 6 }, intArray.Get(0..^0));
	}

	[Fact]
	public void If()
	{
		var intArray = new[] { 1, 2, 3, 4, 5, 6 }.ToImmutableArray();

		Assert.Equal(new[] { 1, 3, 5 }, intArray.If(i => i % 2 == 1));
		Assert.Empty(intArray.If(i => i > 6));
		Assert.Throws<ArgumentNullException>(() => intArray.If(null).ToArray());
	}

	[Fact]
	public async Task IfAsync()
	{
		var intArray = new[] { 1, 2, 3, 4, 5, 6 };

		Assert.Equal(new[] { 1, 3, 5 }, await intArray.ToImmutableArray().IfAsync(async (i, token) => await Task.FromResult(i % 2 == 1)).ToListAsync());
		Assert.Empty(await intArray.ToImmutableArray().IfAsync(async (i, token) => await Task.FromResult(i > 6)).ToListAsync());
		await Assert.ThrowsAsync<ArgumentNullException>(async () => await intArray.IfAsync(null as Func<int, Task<bool>>).ToListAsync());
		await Assert.ThrowsAsync<ArgumentNullException>(async () => await intArray.IfAsync(null as Func<int, CancellationToken, Task<bool>>).ToListAsync());

		await Task.CompletedTask;
	}

	[Fact]
	public void To()
	{
		var intArray = new[] { 1, 2, 3, 4, 5, 6 }.ToImmutableArray();
		var stringArray = new[] { "1", "2", "3", "4", "5", "6" }.ToImmutableArray();

		Assert.Equal(stringArray, intArray.Map(i => i.ToString()));
		Assert.Equal(stringArray, intArray.Map((x, i) => (i + 1).ToString()));
	}

	[Fact]
	public async Task ToAsync()
	{
		var intArray = new[] { 1, 2, 3, 4, 5, 6 }.ToImmutableArray();
		var stringArray = new[] { "1", "2", "3", "4", "5", "6" }.ToImmutableArray();

		Assert.Equal(await stringArray.ToAsync().ToListAsync(), await intArray.MapAsync(async i => await Task.FromResult(i.ToString())).ToListAsync());
	}

	[Fact]
	public void ToIndex()
	{
		var intArray = new[] { 1, 2, 3, 4, 5, 6 }.ToImmutableArray();

		Assert.Equal(new[] { 1, 3, 5 }, intArray.ToIndex(i => i % 2 == 0));
		Assert.Equal(new[] { 0, 2, 4 }, intArray.ToIndex(i => i % 2 == 1));
		Assert.Empty(Array<int>.Empty.ToImmutableArray().ToIndex(i => i % 2 == 1));
		Assert.Throws<ArgumentNullException>(() => Array<int>.Empty.ToImmutableArray().ToIndex(null));
		Assert.Equal(new[] { 3 }, intArray.ToIndex(4));
		Assert.Empty(intArray.ToIndex(7));
		Assert.Empty(Array<int>.Empty.ToImmutableArray().ToIndex(1));
	}
}
