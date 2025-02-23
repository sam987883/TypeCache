// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TypeCache.Extensions;
using TypeCache.Utilities;
using Xunit;
using static System.FormattableString;

namespace TypeCache.Tests.Extensions;

public class ArrayExtensions
{
	[Fact]
	public void Clear()
	{
		var expected = Enumerable.Range(1, 10).ToArray();
		var actual = Enumerable.Range(1, 10).ToArray();

		Array.Clear(expected, 4, 3);
		actual.Clear(4, 3);

		Assert.Equal(expected, actual);

		Array.Clear(expected);
		actual.Clear();
	}

	[Fact]
	public void Copy()
	{
		int[] expected = [1, 2, 3, 4, 5, 6];
		var actual = expected.Copy();

		Assert.Equal(expected, actual);
	}

	[Fact]
	public void ForEach()
	{
		var expected = Enumerable.Range(0, 10).ToArray();
		var actual = Enumerable.Range(0, 10).ToArray();

		Array.ForEach(expected, i => expected[i] += 1);
		actual.ForEach(i => actual[i] += 1);

		Assert.Equal(expected, actual);
	}

	[Fact]
	public void ForEach_Between()
	{
		var expected = Enumerable.Range(1, 10).ToArray();
		var expectedCount = expected.Length - 1;
		var actualCount = 0;
		expected.ForEach(i => expected[i] = i, () => ++actualCount);

		Assert.Equal(expectedCount, actualCount);
	}

	[Fact]
	public void ForEach_ActionRef()
	{
		var expected = Enumerable.Range(1, 10).Select(i => i + 1).ToArray();
		var actual = Enumerable.Range(1, 10).ToArray();

		actual.ForEach(new ActionRef<int>((ref int i) => ++i));

		Assert.Equal(expected, actual);
	}

	[Fact]
	public void ForEach_Index()
	{
		var values = Enumerable.Range(0, 10).Index();

		values.ForEach(_ => Assert.Equal(_.Index, _.Item));
	}

	[Fact]
	public void ForEach_IndexBetween()
	{
		var expected = Enumerable.Range(0, 10).Index();
		var expectedCount = expected.Count() - 1;
		var actualCount = 0;

		expected.ForEach(_ => Assert.Equal(_.Index, _.Item), () => ++actualCount);
		Assert.Equal(expectedCount, actualCount);
	}

	[Fact]
	public void FromBase64()
	{
		var values = "AAECAwQFBgcICQoLDA0ODxAREhMUFRYXGBkaGxwdHh8gISIjJCUmJygpKissLS4vMDEyMzQ1Njc4\r\nOTo7PD0+P0BBQkNERUZHSElKS0xNTk9QUVJTVFVWV1hZWltcXV5fYGFiY2RlZmdoaWprbG1ub3Bx\r\ncnN0dXZ3eHl6e3x9fn+AgYKDhIWGh4iJiouMjY6PkJGSk5SVlpeYmZqbnJ2en6ChoqOkpaanqKmq\r\nq6ytrq+wsbKztLW2t7i5uru8vb6/wMHCw8TFxsfIycrLzM3Oz9DR0tPU1dbX2Nna29zd3t/g4eLj\r\n5OXm5+jp6uvs7e7v8PHy8/T19vf4+fr7/P3+/w==".ToCharArray();
		var expected = Convert.FromBase64CharArray(values, 0, values.Length);
		var actual = values.FromBase64();

		Assert.Equal(expected, actual);

		expected = Convert.FromBase64CharArray(values, 12, 20);
		actual = values.FromBase64(12, 20);

		Assert.Equal(expected, actual);
	}

	[Fact]
	public void InParallel()
	{
		var expected = 0;
		var actual = 0;

		Parallel.Invoke(() => ++expected, () => expected += 2, () => expected += 3);
		var actions = new Action[] { () => ++actual, () => actual += 2, () => actual += 3 };
		actions.InParallel();

		Assert.Equal(expected, actual);
	}


	[Fact]
	public void Length()
	{
		int[] expected = [3, 0, 1, 2];
		var array = new int[expected[0], expected[1], expected[2], expected[3]];
		var actual = Enumerable.Range(0, array.Rank).Select(i => array.Length(i)).ToArray();

		Assert.Equal(expected, actual);
	}

	[Fact]
	public void Reverse()
	{
		var values = Enumerable.Range(0, 10);
		var expected = values.Reverse().ToArray();
		var actual = values.ToArray();

		Array.Reverse(actual);

		Assert.Equal(expected, actual);
	}

	[Fact]
	public void Search()
	{
		var array = Enumerable.Range(0, 10).ToArray();

		Assert.Equal(Array.BinarySearch(array, 3), array.Search(3));
		Assert.Equal(Array.BinarySearch(array, 3, Comparer<int>.Default), array.Search(3, Comparer<int>.Default));
		Assert.Equal(Array.BinarySearch(array, -3), array.Search(-3));
		Assert.Equal(Array.BinarySearch(array, -3, Comparer<int>.Default), array.Search(-3, Comparer<int>.Default));
		Assert.Equal(Array.BinarySearch(array, 1, 4, 3), array.Search(3, 1, 4));
		Assert.Equal(Array.BinarySearch(array, 1, 4, 3, Comparer<int>.Default), array.Search(3, 1, 4, Comparer<int>.Default));
	}

	[Fact]
	public void Sort()
	{
		var expected = Enumerable.Range(1, 10).ToArray();
		var actual = Enumerable.Range(1, 10).Reverse().ToArray();

		Array.Sort(expected);
		actual.Sort();
		Assert.Equal(expected, actual);

		var comparison = new Comparison<int>((a, b) => a < b ? 1 : a > b ? -1 : 0);
		Array.Sort(expected, comparison);
		actual.Sort(comparison);
		Assert.Equal(expected, actual);

		Array.Sort(expected, Comparer<int>.Default);
		actual.Sort(Comparer<int>.Default);
		Assert.Equal(expected, actual);

		Array.Sort(expected, 3, 5, Comparer<int>.Default);
		actual.Sort(3, 5, Comparer<int>.Default);
		Assert.Equal(expected, actual);
	}

	[Fact]
	public void Subarray()
	{
		var array = Enumerable.Range(1, 10).ToArray();

		Assert.Equal(new[] { 4, 5, 6, 7, 8, 9, 10 }, array.Subarray(3));
		Assert.Equal(new[] { 4, 5 }, array.Subarray(3, 2));
		Assert.Throws<IndexOutOfRangeException>(() => array.Subarray(11, 2));
		Assert.Throws<IndexOutOfRangeException>(() => array.Subarray(9, 2));
		Assert.Throws<IndexOutOfRangeException>(() => array.Subarray(0, 22));
	}

	[Fact]
	public void ToHexString()
	{
		var bytes = Encoding.ASCII.GetBytes("ABC abc 123");
		var expected = Convert.ToHexString(bytes);
		var actual = bytes.ToHexString();

		Assert.Equal(expected, actual);
	}

	[Fact]
	public void ToImmutableQueue()
	{
		int[] intArray = [1, 2, 3, 4, 5, 6];
		var intQueue = intArray.ToImmutableQueue();
		string[] stringArray = ["123", "abc", "def"];
		var stringQueue = stringArray.ToImmutableQueue();

		Assert.False(intQueue.IsEmpty);
		Assert.False(stringQueue.IsEmpty);
		Assert.Equal(intArray, intQueue);
		Assert.Equal(stringArray, stringQueue);
		Assert.Equal(ImmutableQueue<int>.Empty, Array<int>.Empty.ToImmutableQueue());
	}

	[Fact]
	public void ToImmutableStack()
	{
		int[] intArray = [1, 2, 3, 4, 5, 6];
		var intStack = intArray.ToImmutableStack();
		string[] stringArray = ["123", "abc", "def"];
		var stringStack = stringArray.ToImmutableStack();
		intArray.Reverse();
		stringArray.Reverse();

		Assert.False(intStack.IsEmpty);
		Assert.False(stringStack.IsEmpty);
		Assert.Equal(intArray, intStack);
		Assert.Equal(stringArray, stringStack);
		Assert.Equal(ImmutableStack<int>.Empty, Array<int>.Empty.ToImmutableStack());
	}

	[Fact]
	public void ToJSON()
	{
		var array = Enumerable.Range(1, 10).ToArray();
		var expected = Invariant($"[{string.Join(',', array)}]");
		var actual = array.ToJSON().ToJsonString();

		Assert.Equal(expected, actual);
	}

	[Fact]
	public void ToSegment()
	{
		var array = Enumerable.Range(0, 10).ToArray();

		Assert.Equal(new ArraySegment<int>(array), array.ToSegment());
		Assert.Equal(new ArraySegment<int>(array, 2, 0), array.ToSegment(2, 0));
		Assert.Equal(new ArraySegment<int>(array, 2, 3), array.ToSegment(2, 3));
		Assert.Equal(new ArraySegment<int>(array, 9, 1), array.ToSegment(9, 1));
	}

	[Fact]
	public void WaitAll()
	{
		var arrayOfTasks = new[] { Task.CompletedTask, Task.CompletedTask, Task.CompletedTask, Task.CompletedTask, Task.CompletedTask, Task.CompletedTask };

		arrayOfTasks.WaitAll();
		arrayOfTasks.WaitAll(CancellationToken.None);
		arrayOfTasks.WaitAll(TimeSpan.FromMilliseconds(1), CancellationToken.None);
	}

	[Fact]
	public async Task WhenAllAsync()
	{
		var arrayOfTasks = new[] { Task.CompletedTask, Task.CompletedTask, Task.CompletedTask, Task.CompletedTask, Task.CompletedTask, Task.CompletedTask };
		var arrayOfIntTasks = new[] { Task.FromResult(1), Task.FromResult(2), Task.FromResult(3), Task.FromResult(4), Task.FromResult(5), Task.FromResult(6) };

		await arrayOfTasks.WhenAllAsync<Task>();
		await arrayOfIntTasks.WhenAllAsync();
	}

	[Fact]
	public void WaitAny()
	{
		var arrayOfTasks = new[] { Task.CompletedTask, Task.CompletedTask, Task.CompletedTask, Task.CompletedTask, Task.CompletedTask, Task.CompletedTask };
		var arrayOfIntTasks = new[] { Task.FromResult(1), Task.FromResult(2), Task.FromResult(3), Task.FromResult(4), Task.FromResult(5), Task.FromResult(6) };

		arrayOfTasks.WaitAny<Task>();
		arrayOfIntTasks.WaitAny<Task>(CancellationToken.None);
		arrayOfIntTasks.WaitAny<int>(TimeSpan.FromMilliseconds(1), CancellationToken.None);
	}

	[Fact]
	public async Task WhenAnyAsync()
	{
		var arrayOfTasks = new[] { Task.CompletedTask, Task.CompletedTask, Task.CompletedTask, Task.CompletedTask, Task.CompletedTask, Task.CompletedTask };
		var arrayOfIntTasks = new[] { Task.FromResult(1), Task.FromResult(2), Task.FromResult(3), Task.FromResult(4), Task.FromResult(5), Task.FromResult(6) };

		await arrayOfTasks.WhenAnyAsync<Task>();
		await arrayOfIntTasks.WhenAnyAsync();
	}
}
