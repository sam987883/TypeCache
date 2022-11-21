// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using TypeCache.Collections;
using TypeCache.Extensions;
using Xunit;

namespace TypeCache.Tests.Extensions;

public class ArrayExtensions
{
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
	public void ToArray()
	{
		var intArray = new[] { 1, 2, 3, 4, 5, 6 };

		Assert.Equal(new[] { "1", "2", "3", "4", "5", "6" }, intArray.Select(i => i.ToString()));
		Assert.Equal(new[] { '1', '2', '3', '4', '5', '6' }, intArray.Select(i => char.Parse(i.ToString())));
		Assert.Equal(Array<char>.Empty, Array<int>.Empty.Select(i => char.Parse(i.ToString())));
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
		intArray.Sort(new Comparison<int>((a, b) => a < b ? 1 : a > b ? -1 : 0));
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

		Assert.Equal(stringArray, intArray.Select(i => i.ToString()));
		Assert.Equal(stringArray, intArray.Select((x, i) => (i + 1).ToString()));
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

		arrayOfTasks.WaitAll();
	}

	[Fact]
	public void WaitForAny()
	{
		var arrayOfTasks = new[] { Task.CompletedTask, Task.CompletedTask, Task.CompletedTask, Task.CompletedTask, Task.CompletedTask, Task.CompletedTask };
		var arrayOfIntTasks = new[] { Task.FromResult(1), Task.FromResult(2), Task.FromResult(3), Task.FromResult(4), Task.FromResult(5), Task.FromResult(6) };

		arrayOfTasks.WaitAny<Task>();
		arrayOfIntTasks.WaitAny<int>();
	}

	[Fact]
	public async Task WhenAllAsync()
	{
		var arrayOfTasks = new[] { Task.CompletedTask, Task.CompletedTask, Task.CompletedTask, Task.CompletedTask, Task.CompletedTask, Task.CompletedTask };
		var arrayOfIntTasks = new[] { Task.FromResult(1), Task.FromResult(2), Task.FromResult(3), Task.FromResult(4), Task.FromResult(5), Task.FromResult(6) };

		await arrayOfTasks.WhenAllAsync<Task>();
		await arrayOfIntTasks.WhenAllAsync();

		await Task.CompletedTask;
	}

	[Fact]
	public async Task WhenAnyAsync()
	{
		var arrayOfTasks = new[] { Task.CompletedTask, Task.CompletedTask, Task.CompletedTask, Task.CompletedTask, Task.CompletedTask, Task.CompletedTask };
		var arrayOfIntTasks = new[] { Task.FromResult(1), Task.FromResult(2), Task.FromResult(3), Task.FromResult(4), Task.FromResult(5), Task.FromResult(6) };

		await arrayOfTasks.WhenAnyAsync<Task>();
		await arrayOfIntTasks.WhenAnyAsync();

		await Task.CompletedTask;
	}
}
