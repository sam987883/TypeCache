// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TypeCache.Collections;
using TypeCache.Collections.Extensions;
using TypeCache.Extensions;
using Xunit;

namespace TypeCache.Tests.Collections.Extensions;

public class ListExtensions
{
	[Fact]
	public void Deconstruct()
	{
		{
			(int? item1, IEnumerable<int> rest) = new List<int>(3) { 1, 2 };
			Assert.Equal(1, item1);
			Assert.Equal(new[] { 2 }, rest);
		}
		{
			(int? item1, int? item2, IEnumerable<int> rest) = new List<int>(3) { 1, 2 };
			Assert.Equal(1, item1);
			Assert.Equal(2, item2);
			Assert.Equal(Array<int>.Empty, rest);
		}
		{
			(int? item1, int? item2, int? item3, IEnumerable<int> rest) = new List<int>(3) { 1, 2 };
			Assert.Equal(1, item1);
			Assert.Equal(2, item2);
			Assert.Null(item3);
			Assert.Equal(Array<int>.Empty, rest);
		}
		{
			(string item1, IEnumerable<string> rest) = new List<string>(3) { "1", "2" };
			Assert.Equal("1", item1);
			Assert.Equal(new[] { "2" }, rest);
		}
		{
			(string item1, string item2, IEnumerable<string> rest) = new List<string>(3) { "1", "2" };
			Assert.Equal("1", item1);
			Assert.Equal("2", item2);
			Assert.Equal(Array<string>.Empty, rest);
		}
		{
			(string item1, string item2, string item3, IEnumerable<string> rest) = new List<string>(3) { "1", "2" };
			Assert.Equal("1", item1);
			Assert.Equal("2", item2);
			Assert.Null(item3);
			Assert.Equal(Array<string>.Empty, rest);
		}
	}

	[Fact]
	public void Do()
	{
		var stringList = new List<string>(3) { "123", "abc", "def" };

		stringList.Do(value => Assert.Contains(value, stringList));
		Assert.Throws<ArgumentNullException>(() => stringList.Do(null as Action<string>));

		stringList.Do((value, i) =>
		{
			Assert.Contains(value, stringList);
			Assert.Contains(i, new[] { 0, 1, 2 });
		});
		Assert.Throws<ArgumentNullException>(() => stringList.Do(null as Action<string, int>));

		var i = 0;
		stringList.Do(value => Assert.Contains(value, stringList), () => ++i);
		Assert.Throws<ArgumentNullException>(() => stringList.Do(null as Action<string>, () => ++i));
		Assert.Throws<ArgumentNullException>(() => stringList.Do(value => Assert.Contains(value, stringList), null));
		Assert.Equal(2, i);

		stringList.Do((value, i) =>
		{
			Assert.Contains(value, stringList);
			Assert.Contains(stringList[i], stringList);
			Assert.Contains(i, new[] { 0, 1, 2 });
		}, () => ++i);
		Assert.Equal(4, i);
		Assert.Throws<ArgumentNullException>(() => stringList.Do(null as Action<string, int>, () => ++i));
		Assert.Throws<ArgumentNullException>(() => stringList.Do((value, i) =>
		{
			Assert.Contains(value, stringList);
			Assert.Contains(stringList[i], stringList);
			Assert.Contains(i, new[] { 0, 1, 2 });
		}, null));
	}

	[Fact]
	public void Get()
	{
		var list = new List<int>() { 1, 2, 3, 4, 5, 6 };

		Assert.Equal(new[] { 3, 4 }, list.Get(2..4));
		Assert.Equal(new[] { 4, 3 }, list.Get(3..1));
		Assert.Empty(list.Get(2..2));
		Assert.Equal(new[] { 3, 4, 5 }, list.Get(2..^1));
		Assert.Throws<ArgumentOutOfRangeException>(() => list.Get(^0..0).ToArray());
		Assert.Equal(new[] { 1, 2, 3, 4, 5, 6 }, list.Get(0..^0));
	}

	[Fact]
	public void If()
	{
		var list = new List<int>() { 1, 2, 3, 4, 5, 6 };

		Assert.Equal(new[] { 1, 3, 5 }, list.If(i => i % 2 == 1));
		Assert.Empty(list.If(i => i > 6));
		Assert.Throws<ArgumentNullException>(() => list.If(null).ToArray());
	}

	[Fact]
	public async Task IfAsync()
	{
		var list = new List<int>() { 1, 2, 3, 4, 5, 6 };

		Assert.Equal(new[] { 1, 3, 5 }, await list.IfAsync(async (i, token) => await Task.FromResult(i % 2 == 1)).ToListAsync());
		Assert.Empty(await list.IfAsync(async (i, token) => await Task.FromResult(i > 6)).ToListAsync());
		await Assert.ThrowsAsync<ArgumentNullException>(async () => await list.IfAsync(null as Func<int, Task<bool>>).ToListAsync());
		await Assert.ThrowsAsync<ArgumentNullException>(async () => await list.IfAsync(null as Func<int, CancellationToken, Task<bool>>).ToListAsync());

		await Task.CompletedTask;
	}

	[Fact]
	public void To()
	{
		var intList = new List<int>() { 1, 2, 3, 4, 5, 6 };
		var stringList = new List<string>() { "1", "2", "3", "4", "5", "6" };

		Assert.Empty((null as int[]).Map(i => i.ToString()));
		Assert.Equal(stringList, intList.Map(i => i.ToString()));
		Assert.Equal(stringList, intList.Map((x, i) => (i + 1).ToString()));
	}

	[Fact]
	public async Task ToAsync()
	{
		var intList = new List<int>() { 1, 2, 3, 4, 5, 6 };
		var stringList = new List<string>() { "1", "2", "3", "4", "5", "6" };

		Assert.Equal(await stringList.ToAsync().ToListAsync(), await intList.MapAsync(async i => await Task.FromResult(i.ToString())).ToListAsync());
	}

	[Fact]
	public void ToIndex()
	{
		var intList = new List<int>() { 1, 2, 3, 4, 5, 6 };

		Assert.Equal(new[] { 1, 3, 5 }, intList.ToIndex(i => i % 2 == 0));
		Assert.Equal(new[] { 0, 2, 4 }, intList.ToIndex(i => i % 2 == 1));
		Assert.Empty(Array<int>.Empty.ToIndex(i => i % 2 == 1));
		Assert.Empty((null as int[]).ToIndex(i => i % 2 == 1));
		Assert.Throws<ArgumentNullException>(() => Array<int>.Empty.ToIndex(null));
		Assert.Equal(new[] { 3 }, intList.ToIndex(4));
		Assert.Empty(intList.ToIndex(7));
		Assert.Empty(Array<int>.Empty.ToIndex(1));
		Assert.Empty((null as int[]).ToIndex(1));
	}
}
