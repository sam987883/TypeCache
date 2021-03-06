﻿// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;
using TypeCache.Collections;
using TypeCache.Collections.Extensions;
using TypeCache.Extensions;
using Xunit;

namespace TypeCache.Tests.Collections.Extensions
{
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
		public void Deconstruct()
		{
			{
				(int? item1, IEnumerable<int> rest) = new[] { 1, 2 };
				Assert.Equal(1, item1);
				Assert.Equal(new[] { 2 }, rest);
			}
			{
				(int? item1, int? item2, IEnumerable<int> rest) = new[] { 1, 2 };
				Assert.Equal(1, item1);
				Assert.Equal(2, item2);
				Assert.Equal(Array<int>.Empty, rest);
			}
			{
				(int? item1, int? item2, int? item3, IEnumerable<int> rest) = new[] { 1, 2 };
				Assert.Equal(1, item1);
				Assert.Equal(2, item2);
				Assert.Null(item3);
				Assert.Equal(Array<int>.Empty, rest);
			}
			{
				(string item1, IEnumerable<string> rest) = new[] { "1", "2" };
				Assert.Equal("1", item1);
				Assert.Equal(new[] { "2" }, rest);
			}
			{
				(string item1, string item2, IEnumerable<string> rest) = new[] { "1", "2" };
				Assert.Equal("1", item1);
				Assert.Equal("2", item2);
				Assert.Equal(Array<string>.Empty, rest);
			}
			{
				(string item1, string item2, string item3, IEnumerable<string> rest) = new[] { "1", "2" };
				Assert.Equal("1", item1);
				Assert.Equal("2", item2);
				Assert.Null(item3);
				Assert.Equal(Array<string>.Empty, rest);
			}
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
			Assert.Equal(new[] { 4, 3 }, intArray.Get(3..1));
			Assert.Equal(Array<int>.Empty, intArray.Get(2..2));
			Assert.Equal(new[] { 3, 4, 5 }, intArray.Get(2..^1));
			Assert.Throws<IndexOutOfRangeException>(() => intArray.Get(^0..0).ToArray());
			Assert.Equal(new[] { 1, 2, 3, 4, 5, 6 }, intArray.Get(0..^0));
		}

		[Fact]
		public void IsSequence()
		{
			var stringArray = new[] { "123", "abc", "def" };
			Assert.True(stringArray.IsSequence(new[] { "123", "ABC", "DEF" }));
			Assert.False(stringArray.IsSequence(new[] { "123", "ABC", "DEF" }, StringComparison.Ordinal));
			Assert.False(stringArray.IsSequence(new[] { "123", "def", "abc" }, StringComparison.OrdinalIgnoreCase));
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
		public void ToArray()
		{
			var intArray = new[] { 1, 2, 3, 4, 5, 6 };
			Assert.Equal(new[] { "1", "2", "3", "4", "5", "6" }, intArray.ToArray(i => i.ToString()));
			Assert.Equal(new[] { '1', '2', '3', '4', '5', '6' }, intArray.ToArray(i => char.Parse(i.ToString())));
			Assert.Equal(Array<char>.Empty, Array<int>.Empty.ToArray(i => char.Parse(i.ToString())));
			Assert.Equal(Array<char>.Empty, (null as int[]).ToArray(i => char.Parse(i.ToString())));
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
			Assert.Equal(intArray[^1], intArray.ToImmutableStack().FirstValue());
			Assert.Equal(stringArray[^1], stringArray.ToImmutableStack().First());
			Assert.Equal(ImmutableStack<int>.Empty, Array<int>.Empty.ToImmutableStack());
		}

		[Fact]
		public void ToIndex()
		{
			var intArray = new[] { 1, 2, 3, 4, 5, 6 };
			Assert.Equal(new[] { 1, 3, 5 }, intArray.ToIndex(i => i % 2 == 0));
			Assert.Equal(new[] { 0, 2, 4 }, intArray.ToIndex(i => i % 2 == 1));
			Assert.Empty(Array<int>.Empty.ToIndex(i => i % 2 == 1));
			Assert.Empty((null as int[]).ToIndex(i => i % 2 == 1));
			Assert.Throws<ArgumentNullException>(() => Array<int>.Empty.ToIndex(null));
			Assert.Equal(new[] { 3 }, intArray.ToIndex(4));
			Assert.Empty(intArray.ToIndex(7));
			Assert.Empty(Array<int>.Empty.ToIndex(1));
			Assert.Empty((null as int[]).ToIndex(1));
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
}
