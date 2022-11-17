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

public class ArrayExtensions
{
	[Fact]
	public async Task AllAsync()
	{
		var arrayOfTasks = new[] { Task.CompletedTask, Task.CompletedTask, Task.CompletedTask, Task.CompletedTask, Task.CompletedTask, Task.CompletedTask };
		var arrayOfIntTasks = new[] { Task.FromResult(1), Task.FromResult(2), Task.FromResult(3), Task.FromResult(4), Task.FromResult(5), Task.FromResult(6) };

		await arrayOfTasks.AllAsync<Task>();
		await arrayOfIntTasks.AllAsync();

		await Task.CompletedTask;
	}

	[Fact]
	public async Task AnyAsync()
	{
		var arrayOfTasks = new[] { Task.CompletedTask, Task.CompletedTask, Task.CompletedTask, Task.CompletedTask, Task.CompletedTask, Task.CompletedTask };
		var arrayOfIntTasks = new[] { Task.FromResult(1), Task.FromResult(2), Task.FromResult(3), Task.FromResult(4), Task.FromResult(5), Task.FromResult(6) };

		await arrayOfTasks.AnyAsync<Task>();
		await arrayOfIntTasks.AnyAsync();

		await Task.CompletedTask;
	}

	[Fact]
	public void Clear()
	{
		var intArray = new[] { 1, 2, 3, 4, 5, 6 };

		intArray.Clear(2, 2);
		Assert.Equal(new[] { 1, 2, 0, 0, 5, 6 }, intArray);
		intArray.Clear();
		Assert.Equal(new[] { 0, 0, 0, 0, 0, 0 }, intArray);
	}

	[Fact]
	public void Copy()
	{
		var intArray = new[] { 1, 2, 3, 4, 5, 6 };

		Assert.Equal(intArray, intArray.GetCopy());
	}

	[Fact]
	public void Do()
	{
		var stringArray = new[] { "123", "abc", "def" };

		stringArray.Do(value => Assert.Contains(value, stringArray));
		Assert.Throws<ArgumentNullException>(() => stringArray.Do(null as Action<string>));

		stringArray.Do((value, i) =>
		{
			Assert.Contains(value, stringArray);
			Assert.Contains(i, new[] { 0, 1, 2 });
		});
		Assert.Throws<ArgumentNullException>(() => stringArray.Do(null as Action<string, int>));

		stringArray.Do(new ActionRef<string>((ref string value) =>
		{
			value = value.ToUpperInvariant();
		}));
		Assert.Equal("ABC", stringArray[1]);
		Assert.Equal("DEF", stringArray[2]);
		Assert.Throws<ArgumentNullException>(() => stringArray.Do(null as ActionRef<string>));

		stringArray.Do(new ActionRef<string, int>((ref string value, ref int i) =>
		{
			value = i.ToString();
			if (i == 0)
				++i;
		}));
		Assert.Equal("0", stringArray[0]);
		Assert.Equal("ABC", stringArray[1]);
		Assert.Equal("2", stringArray[2]);
		Assert.Throws<ArgumentNullException>(() => stringArray.Do(null as ActionRef<string, int>));

		stringArray = new[] { "123", "abc", "def" };
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
		var intArray = new[] { 1, 2, 3, 4, 5, 6 };

		Assert.Equal(new[] { 3, 4 }, intArray.Get(2..4));
		Assert.Equal(new[] { 3, 2 }, intArray.Get(3..1));
		Assert.Empty(intArray.Get(2..2));
		Assert.Equal(new[] { 3, 4, 5 }, intArray.Get(2..^1));
		Assert.Equal(new[] { 6, 5, 4, 3, 2, 1 }, intArray.Get(^0..0).ToArray());
		Assert.Equal(new[] { 1, 2, 3, 4, 5, 6 }, intArray.Get(0..^0));
	}

	[Fact]
	public void If()
	{
		var intArray = new[] { 1, 2, 3, 4, 5, 6 };

		Assert.Equal(new[] { 1, 3, 5 }, intArray.If(i => i % 2 == 1));
		Assert.Empty(intArray.If(i => i > 6));
		Assert.Throws<ArgumentNullException>(() => intArray.If(null).ToArray());
	}

	[Fact]
	public async Task IfAsync()
	{
		var intArray = new[] { 1, 2, 3, 4, 5, 6 };

		Assert.Equal(new[] { 1, 3, 5 }, await intArray.IfAsync(async (i, token) => await Task.FromResult(i % 2 == 1)).ToArrayAsync());
		Assert.Empty(await intArray.IfAsync(async (i, token) => await Task.FromResult(i > 6)).ToArrayAsync());
		await Assert.ThrowsAsync<ArgumentNullException>(async () => await intArray.IfAsync(null as Func<int, Task<bool>>).ToArrayAsync());
		await Assert.ThrowsAsync<ArgumentNullException>(async () => await intArray.IfAsync(null as Func<int, CancellationToken, Task<bool>>).ToArrayAsync());

		await Task.CompletedTask;
	}

	[Fact]
	public void ToArray()
	{
		var intArray = new[] { 1, 2, 3, 4, 5, 6 };

		Assert.Equal(new[] { "1", "2", "3", "4", "5", "6" }, intArray.Map(i => i.ToString()));
		Assert.Equal(new[] { '1', '2', '3', '4', '5', '6' }, intArray.Map(i => char.Parse(i.ToString())));
		Assert.Equal(Array<char>.Empty, Array<int>.Empty.Map(i => char.Parse(i.ToString())));
		Assert.Equal(Array<char>.Empty, (null as int[]).Map(i => char.Parse(i.ToString())));
	}

	[Fact]
	public void Reverse()
	{
		var intArray = new[] { 1, 2, 3, 4, 5, 6 };

		intArray.Reverse();
		Assert.Equal(new[] { 6, 5, 4, 3, 2, 1 }, intArray);
	}

	[Fact]
	public void Search()
	{
		var intArray = new[] { 1, 2, 3, 4, 5, 6 };

		Assert.Equal(2, intArray.Search(3));
		Assert.Equal(-1, intArray.Search(-3, Comparer<int>.Default));
		Assert.Equal(2, intArray.Search(3, 1, 4, Comparer<int>.Default));
		Assert.Equal(-7, intArray.Search(10));
	}

	[Fact]
	public void Sort()
	{
		var intArray = new[] { 1, 2, 3, 4, 5, 6 };

		intArray.Sort();
		Assert.Equal(new[] { 1, 2, 3, 4, 5, 6 }, intArray);
		intArray.Sort(new Comparison<int>((a, b) => a < b ? 1 : (a > b ? -1 : 0)));
		Assert.Equal(new[] { 6, 5, 4, 3, 2, 1 }, intArray);
		intArray.Sort(2, 3, Comparer<int>.Default);
		Assert.Equal(new[] { 6, 5, 2, 3, 4, 1 }, intArray);
	}

	[Fact]
	public void Subarray()
	{
		var intArray = new[] { 1, 2, 3, 4, 5, 6 };

		Assert.Equal(new[] { 4, 5, 6 }, intArray.Subarray(3));
		Assert.Equal(new[] { 4, 5 }, intArray.Subarray(3, 2));
	}

	[Fact]
	public void To()
	{
		var intArray = new[] { 1, 2, 3, 4, 5, 6 };
		var stringArray = new[] { "1", "2", "3", "4", "5", "6" };

		Assert.Empty((null as int[]).Map(i => i.ToString()));
		Assert.Equal(stringArray, intArray.Map(i => i.ToString()));
		Assert.Equal(stringArray, intArray.Map((x, i) => (i + 1).ToString()));
	}

	[Fact]
	public async Task ToAsync()
	{
		var intArray = new[] { 1, 2, 3, 4, 5, 6 };
		var stringArray = new[] { "1", "2", "3", "4", "5", "6" };

		Assert.Equal(await stringArray.ToAsync().ToArrayAsync(), await intArray.MapAsync(async i => await Task.FromResult(i.ToString())).ToArrayAsync());
	}

	[Fact]
	public void ToBoolean()
	{
		Assert.True(true.ToBytes().ToBoolean());
		Assert.False(false.ToBytes().ToBoolean());
	}

	[Fact]
	public void ToChar()
	{
		Assert.Equal(char.MinValue, char.MinValue.ToBytes().ToChar());
		Assert.Equal(char.MaxValue, char.MaxValue.ToBytes().ToChar());
	}

	[Fact]
	public void ToDouble()
	{
		Assert.Equal(double.MinValue, double.MinValue.ToBytes().ToDouble());
		Assert.Equal(double.MaxValue, double.MaxValue.ToBytes().ToDouble());
	}

	[Fact]
	public void ToImmutableQueue()
	{
		var intArray = new[] { 1, 2, 3, 4, 5, 6 };
		var stringArray = new[] { "123", "abc", "def" };

		Assert.False(intArray.ToImmutableQueue().IsEmpty);
		Assert.False(stringArray.ToImmutableQueue().IsEmpty);
		Assert.Equal(ImmutableQueue<int>.Empty, Array<int>.Empty.ToImmutableQueue());
	}

	[Fact]
	public void ToImmutableStack()
	{
		var intArray = new[] { 1, 2, 3, 4, 5, 6 };
		var stringArray = new[] { "123", "abc", "def" };

		Assert.Equal(intArray[^1], intArray.ToImmutableStack().First());
		Assert.Equal(stringArray[^1], stringArray.ToImmutableStack().First());
		Assert.Equal(ImmutableStack<int>.Empty, Array<int>.Empty.ToImmutableStack());
	}

	[Fact]
	public void ToInt16()
	{
		Assert.Equal(short.MinValue, short.MinValue.ToBytes().ToInt16());
		Assert.Equal(short.MaxValue, short.MaxValue.ToBytes().ToInt16());
	}

	[Fact]
	public void ToInt32()
	{
		Assert.Equal(int.MinValue, int.MinValue.ToBytes().ToInt32());
		Assert.Equal(int.MaxValue, int.MaxValue.ToBytes().ToInt32());
	}

	[Fact]
	public void ToInt64()
	{
		Assert.Equal(long.MinValue, long.MinValue.ToBytes().ToInt64());
		Assert.Equal(long.MaxValue, long.MaxValue.ToBytes().ToInt64());
	}

	[Fact]
	public void ToSingle()
	{
		Assert.Equal(float.MinValue, float.MinValue.ToBytes().ToSingle());
		Assert.Equal(float.MaxValue, float.MaxValue.ToBytes().ToSingle());
	}

	[Fact]
	public void ToUInt16()
	{
		Assert.Equal(ushort.MinValue, ushort.MinValue.ToBytes().ToUInt16());
		Assert.Equal(ushort.MaxValue, ushort.MaxValue.ToBytes().ToUInt16());
	}

	[Fact]
	public void ToUInt32()
	{
		Assert.Equal(uint.MinValue, uint.MinValue.ToBytes().ToUInt32());
		Assert.Equal(uint.MaxValue, uint.MaxValue.ToBytes().ToUInt32());
	}

	[Fact]
	public void ToUInt64()
	{
		Assert.Equal(ulong.MinValue, ulong.MinValue.ToBytes().ToUInt64());
		Assert.Equal(ulong.MaxValue, ulong.MaxValue.ToBytes().ToUInt64());
	}

	[Fact]
	public void WaitForAll()
	{
		var arrayOfTasks = new[] { Task.CompletedTask, Task.CompletedTask, Task.CompletedTask, Task.CompletedTask, Task.CompletedTask, Task.CompletedTask };
		var arrayOfIntTasks = new[] { Task.FromResult(1), Task.FromResult(2), Task.FromResult(3), Task.FromResult(4), Task.FromResult(5), Task.FromResult(6) };

		arrayOfTasks.WaitForAll<Task>();
		arrayOfIntTasks.WaitForAll<int>();
	}

	[Fact]
	public void WaitForAny()
	{
		var arrayOfTasks = new[] { Task.CompletedTask, Task.CompletedTask, Task.CompletedTask, Task.CompletedTask, Task.CompletedTask, Task.CompletedTask };
		var arrayOfIntTasks = new[] { Task.FromResult(1), Task.FromResult(2), Task.FromResult(3), Task.FromResult(4), Task.FromResult(5), Task.FromResult(6) };

		arrayOfTasks.WaitForAny<Task>();
		arrayOfIntTasks.WaitForAny<int>();
	}
}
