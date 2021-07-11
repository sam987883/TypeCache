// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;
using TypeCache.Collections;
using TypeCache.Collections.Extensions;
using TypeCache.Extensions;
using Xunit;

namespace TypeCache.Tests.Collections.Extensions
{
	public class IEnumerableExtensions
	{
		private IEnumerable<int> GetItems()
		{
			yield return 1;
			yield return 2;
			yield return 3;
			yield return 4;
			yield return 5;
			yield return 6;
		}

		[Fact]
		public void Aggregate()
		{
			Assert.Equal(22, this.GetItems().Aggregate(1, (a, b) => a + b));
			Assert.Throws<ArgumentNullException>(() => this.GetItems().Aggregate(1, null));
		}

		[Fact]
		public async Task AggregateAsync()
		{
			Assert.Equal(22, await this.GetItems().AggregateAsync(1, async (a, b) => await Task.FromResult(a + b)));
			await Assert.ThrowsAsync<ArgumentNullException>(async () => await this.GetItems().AggregateAsync(1, null));

			await Task.CompletedTask;
		}

		[Fact]
		public void All()
		{
			Assert.True(this.GetItems().All(i => i > 0));
			Assert.Throws<ArgumentNullException>(() => this.GetItems().All(null));
			Assert.False(this.GetItems().All(i => i % 2 is 0));
		}

		[Fact]
		public async Task AllAsync()
		{
			await new[] { Task.CompletedTask, Task.CompletedTask, Task.CompletedTask }.AllAsync<Task>();
			Assert.Equal(new[] { 1, 2, 3 }, await new[] { Task.FromResult(1), Task.FromResult(2), Task.FromResult(3) }.AllAsync());

			await Task.CompletedTask;
		}

		[Fact]
		public void And()
		{
			var array = new[] { 1, 2, 3, 4, 5, 6 };

			Assert.Equal(array, new[] { 1, 2, 3 }.And(4, 5, 6));
			Assert.Equal(array, new[] { 1, 2, 3 }.And((IEnumerable<int>)new[] { 4, 5, 6 }));
			Assert.Equal(array, new[] { 1, 2 }.And((IEnumerable<IEnumerable<int>>)new[] { (IEnumerable<int>)new[] { 3, 4 }, (IEnumerable<int>)new[] { 5, 6 } }));
		}

		[Fact]
		public void Any()
		{
			var array = new[] { 1, 2, 3, 4, 5, 6 };

			Assert.False((null as IEnumerable).Any<int>());
			Assert.False(((IEnumerable)Array<int>.Empty).Any<int>());
			Assert.True(((IEnumerable)new[] { 1, 2, 3 }).Any<int>());
			Assert.True(this.GetItems().Any<int>());

			Assert.True(array.Any());
			Assert.True(new List<int>(array).Any());
			Assert.True(this.GetItems().Any());
			Assert.False(Array<string>.Empty.Any());
			Assert.False(new List<string>(1).Any());
			Assert.False(Enumerable<string>.Empty.Any());
			Assert.True(array.Any(i => i % 2 == 0));
			Assert.False(array.Any(i => i > 6 || i < 1));
		}

		[Fact]
		public async Task AnyAsync()
		{
			await new[] { Task.CompletedTask, Task.CompletedTask, Task.CompletedTask }.AnyAsync<Task>();
			Assert.InRange(await await new[] { Task.FromResult(1), Task.FromResult(2), Task.FromResult(3) }.AnyAsync(), 1, 3);

			await Task.CompletedTask;
		}

		[Fact]
		public void As()
		{
			Assert.NotNull((null as IEnumerable).As<int>());
			Assert.IsType<int[]>(((IEnumerable)new[] { 1, 2, 3 }).As<int>());
			Assert.NotNull(this.GetItems().As<int>());
			Assert.Equal(new string[] { null, null, null, null, null, null }, this.GetItems().As<string>());
		}

		[Fact]
		public void Count()
		{
			var array = new[] { 1, 2, 3, 4, 5, 6 };

			Assert.Equal(array.Length, array.Count());
			Assert.Equal(array.Length, new List<int>(array).Count());
			Assert.Equal(6, this.GetItems().Count());
			Assert.Equal(0, Array<string>.Empty.Count());
			Assert.Equal(0, new List<string>(1).Count());
			Assert.Equal(0, Enumerable<string>.Empty.Count());
		}

		[Fact]
		public void Deconstruct()
		{
			{
				(int? item1, IEnumerable<int> rest) = new List<int>(2) { 1, 2 };
				Assert.Equal(1, item1);
				Assert.Equal(new[] { 2 }, rest);
			}
			{
				(int? item1, int? item2, IEnumerable<int> rest) = new List<int>(2) { 1, 2 };
				Assert.Equal(1, item1);
				Assert.Equal(2, item2);
				Assert.Equal(Array<int>.Empty, rest);
			}
			{
				(int? item1, int? item2, int? item3, IEnumerable<int> rest) = new List<int>(2) { 1, 2 };
				Assert.Equal(1, item1);
				Assert.Equal(2, item2);
				Assert.Null(item3);
				Assert.Equal(Array<int>.Empty, rest);
			}
			{
				(string item1, IEnumerable<string> rest) = new List<string>(2) { "1", "2" };
				Assert.Equal("1", item1);
				Assert.Equal(new[] { "2" }, rest);
			}
			{
				(string item1, string item2, IEnumerable<string> rest) = new List<string>(2) { "1", "2" };
				Assert.Equal("1", item1);
				Assert.Equal("2", item2);
				Assert.Equal(Array<string>.Empty, rest);
			}
			{
				(string item1, string item2, string item3, IEnumerable<string> rest) = new List<string>(2) { "1", "2" };
				Assert.Equal("1", item1);
				Assert.Equal("2", item2);
				Assert.Null(item3);
				Assert.Equal(Array<string>.Empty, rest);
			}
		}

		[Fact]
		public void Do()
		{
			var stringList = new List<string> { "123", "abc", "def" };
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
			Assert.Equal(2, i);
			Assert.Throws<ArgumentNullException>(() => stringList.Do(null as Action<string>, () => ++i));
			Assert.Throws<ArgumentNullException>(() => stringList.Do(value => Assert.Contains(value, stringList), null));

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
		public async Task DoAsync()
		{
			var listOfTasks = new List<Task> { Task.CompletedTask, Task.CompletedTask, Task.CompletedTask, Task.CompletedTask, Task.CompletedTask, Task.CompletedTask };

			await new List<Task>(0).DoAsync(async task => await task);
			await listOfTasks.DoAsync(async task => await task);
			await Assert.ThrowsAsync<ArgumentNullException>(async () => await listOfTasks.DoAsync(null));

			var listOfIntTasks = new List<Task<int>> { Task.FromResult(1), Task.FromResult(2), Task.FromResult(3), Task.FromResult(4), Task.FromResult(5), Task.FromResult(6) };
			var array = new[] { 1, 2, 3, 4, 5, 6 };

			await new List<Task<int>>(0).DoAsync(async task => await task);
			await listOfIntTasks.DoAsync(async task => Assert.Contains(await task, array));
			await Assert.ThrowsAsync<ArgumentNullException>(async () => await listOfIntTasks.DoAsync(null));

			await Task.CompletedTask;
		}

		[Fact]
		public void First()
		{
			Assert.Null((null as IEnumerable).First<string>());
			Assert.Null(((IEnumerable)Array<string>.Empty).First<string>());
			Assert.Null(this.GetItems().First<string>());

			Assert.Null(new List<string>().First());
			Assert.Equal("1", new List<string> { "1", "2", "3" }.First());
		}

		[Fact]
		public void FirstValue()
		{
			Assert.Null((null as IEnumerable).FirstValue<int>());
			Assert.Null(((IEnumerable)Array<int>.Empty).FirstValue<int>());
			Assert.Equal(1, this.GetItems().FirstValue<int>());

			Assert.Null(new List<int>().FirstValue());
			Assert.Equal(1, new List<int> { 1, 2, 3 }.FirstValue());
		}

		[Fact]
		public void Gather()
		{
			var array = new[] { 1, 2, 3, 4, 5, 6 };

			Assert.Equal(Enumerable<int>.Empty, new[] { Array<int>.Empty }.Gather());
			Assert.Equal(Enumerable<int>.Empty, ((IEnumerable<IEnumerable<int>>)new IEnumerable<int>[] { ImmutableArray<int>.Empty }).Gather());
			Assert.Equal(Enumerable<int>.Empty, new List<IEnumerable<int>> { new List<int>(0) }.Gather());

			Assert.Equal(array, new[] { new[] { 1, 2 }, new[] { 3, 4 }, new[] { 5, 6 } }.Gather());
			Assert.Equal(array, ((IEnumerable<IEnumerable<int>>)new IEnumerable<int>[] { new[] { 1, 2 }.ToImmutableArray(), new[] { 3, 4 }.ToImmutableArray(), new[] { 5, 6 }.ToImmutableArray() }).Gather());
			Assert.Equal(array, new List<IEnumerable<int>> { new List<int>() { 1, 2 }, new List<int>() { 3, 4 }, new List<int>() { 5, 6 } }.Gather());
		}

		[Fact]
		public void Get()
		{
			var array = new[] { 1, 2, 3, 4, 5, 6 };
			var immutableArray = array.ToImmutableArray();
			var list = new List<int>(array);
			var enumerable = this.GetItems();

			Assert.Equal("2", new[] { "1", "2", "3" }.Get(1));
			Assert.Equal("2", new List<string> { "1", "2", "3" }.Get(1));
			Assert.Equal(null, this.GetItems().As<string>().Get(1));

			Assert.Equal(new[] { 3, 4 }, array.Get(2..4));
			Assert.Equal(new[] { 3, 4 }, immutableArray.Get(2..4));
			Assert.Equal(new[] { 3, 4 }, list.Get(2..4));
			Assert.Equal(new[] { 3, 4 }, enumerable.Get(2..4));
			Assert.Equal(new[] { 4, 3 }, array.Get(3..1));
			Assert.Equal(new[] { 4, 3 }, immutableArray.Get(3..1));
			Assert.Equal(new[] { 4, 3 }, list.Get(3..1));
			Assert.Equal(new[] { 4, 3 }, enumerable.Get(3..1));
			Assert.Equal(Array<int>.Empty, array.Get(2..2));
			Assert.Equal(Array<int>.Empty, immutableArray.Get(2..2));
			Assert.Equal(Array<int>.Empty, list.Get(2..2));
			Assert.Equal(Array<int>.Empty, enumerable.Get(2..2));
			Assert.Equal(new[] { 3, 4, 5 }, array.Get(2..^1));
			Assert.Equal(new[] { 3, 4, 5 }, immutableArray.Get(2..^1));
			Assert.Equal(new[] { 3, 4, 5 }, list.Get(2..^1));
			Assert.Equal(new[] { 3, 4, 5 }, enumerable.Get(2..^1));
			Assert.Throws<IndexOutOfRangeException>(() => array.Get(^0..0).ToArray());
			Assert.Throws<IndexOutOfRangeException>(() => immutableArray.Get(^0..0).ToArray());
			Assert.Throws<ArgumentOutOfRangeException>(() => list.Get(^0..0).ToArray());
			Assert.Throws<IndexOutOfRangeException>(() => enumerable.Get(^0..0).ToArray());
			Assert.Equal(array, array.Get(0..^0));
			Assert.Equal(array, immutableArray.Get(0..^0));
			Assert.Equal(array, list.Get(0..^0));
			Assert.Equal(array, enumerable.Get(0..^0));
		}

		[Fact]
		public void GetValue()
		{
			Assert.Equal(2, new List<int> { 1, 2, 3 }.GetValue(1));
			Assert.Equal(2, new[] { 1, 2, 3 }.GetValue(1));
			Assert.Equal(2, this.GetItems().GetValue(1));
		}

		[Fact]
		public void Group()
		{
			var array = new[] { 1, 2, 3, 4, 5, 6 };

			var group = array.Group(i => i - 1);
			Assert.Contains(0, group.Keys);
			Assert.Contains(1, group.Keys);
			Assert.Contains(2, group.Keys);
			Assert.Throws<ArgumentNullException>(() => array.Group(null));
		}

		[Fact]
		public void Has()
		{
			var array = new[] { 1, 2, 3, 4, 5, 6 };

			Assert.True(array.Has(Index.FromStart(2)));
			Assert.True(array.Has(Index.FromEnd(2)));
			Assert.False(array.Has(Index.FromStart(999)));
			Assert.False(array.Has(Index.FromEnd(999)));

			Assert.True(array.Has(1));
			Assert.True(array.Has(6));
			Assert.False(array.Has(0));
			Assert.False(array.Has(7));

			Assert.True(array.Has(new[] { 1, 2, 3, 4, 5, 6 }));
			Assert.False(array.Has(new[] { 1, 2, 3, 4, 5, 6, 0, 7 }));
		}

		[Fact]
		public void If()
		{
			var array = new[] { 1, 2, 3, 4, 5, 6 };

			Assert.Empty((null as IEnumerable).If<int>());
			Assert.Empty((null as IEnumerable).If<string>());
			Assert.Empty(((IEnumerable)Array<int>.Empty).If<int>());
			Assert.Empty(((IEnumerable)Array<string>.Empty).If<string>());
			Assert.NotEmpty(this.GetItems().If<int>());
			Assert.Empty(this.GetItems().If<string>());

			Assert.Empty((null as IEnumerable).If<string>());
			Assert.Empty(((IEnumerable)Enumerable<object>.Empty).If<int>());
			Assert.Equal(new[] { 1, 3, 5 }, new object[] { 1, "2", 3, "4", 5, "6" }.If<int>());
			Assert.Equal(new[] { "2", "4", "6" }, new object[] { 1, "2", 3, "4", 5, "6" }.If<string>());
			Assert.Equal(new[] { 1, 3, 5 }, ((IEnumerable<int>)array).If(i => i % 2 == 1));
			Assert.Equal(new[] { 1, 3, 5 }, new List<int>(array).If(i => i % 2 == 1));
			Assert.Equal(new[] { 1, 3, 5 }, array.ToImmutableArray().If(i => i % 2 == 1));
			Assert.Equal(new[] { 1, 3, 5 }, this.GetItems().If(i => i % 2 == 1));
			Assert.Empty(((IEnumerable<int>)array).If(i => i > 6));
			Assert.Throws<ArgumentNullException>(() => ((IEnumerable<int>)array).If(null).ToArray());
		}

		[Fact]
		public async Task IfAsync()
		{
			Assert.Equal(new[] { 1, 3, 5 }, await this.GetItems().IfAsync(async i => await Task.FromResult(i % 2 == 1)).ToListAsync());
			Assert.Empty(await this.GetItems().IfAsync(async i => await Task.FromResult(i > 6)).ToListAsync());
			await Assert.ThrowsAsync<ArgumentNullException>(async () => await this.GetItems().IfAsync(null).ToListAsync());

			await Task.CompletedTask;
		}

		[Fact]
		public void IsSequence()
		{
			var array = new[] { 1, 2, 3, 4, 5, 6 };

			Assert.True(array.IsSequence(new[] { 1, 2, 3, 4, 5, 6 }));
			Assert.False(array.IsSequence(new[] { 1, 2, 3, 4, 5, 6, 0, 7 }));
			Assert.False(array.IsSequence(null));
			Assert.False((null as IEnumerable<int>).IsSequence(new[] { 1, 2, 3, 4, 5, 6 }));
		}

		[Fact]
		public void IsSet()
		{
			var array = new[] { 1, 2, 3, 4, 5, 6 };

			Assert.True(array.IsSet(new[] { 6, 5, 4, 3, 2, 1 }));
			Assert.False(array.IsSet(new[] { 1, 2, 3, 4, 5, 6, 0, 7 }));
			Assert.False(array.IsSet(null));
			Assert.False((null as IEnumerable<int>).IsSet(new[] { 1, 2, 3, 4, 5, 6 }));
		}

		[Fact]
		public void Join()
		{
			var array = new[] { 1, 2, 3, 4, 5, 6 };

			Assert.Equal("1,2,3,4,5,6", array.Join(','));
			Assert.Equal("1.2.3.4.5.6", array.Join("."));
			Assert.Equal(string.Empty, (null as IEnumerable<int>).Join('!'));
			Assert.Equal(string.Empty, Enumerable<int>.Empty.Join("aaa"));
		}

		[Fact]
		public void Match()
		{
			var array = new[] { 1, 2, 3, 4, 5, 6 };

			Assert.Equal(array, array.Match(array));
			Assert.True(new[] { 2, 4, 6 }.IsSet(array.Match(new[] { 0, 2, 4, 6, 8 })));
			Assert.Equal(Enumerable<int>.Empty, array.Match(null));
		}

		[Fact]
		public void Maximum()
		{
			var array = new[] { 1, 2, 3, 4, 5, 6 };

			Assert.Equal(6, array.Maximum());
			Assert.Equal(0, Enumerable<int>.Empty.Maximum());
			Assert.Equal(0, (null as IEnumerable<int>).Maximum());
		}

		[Fact]
		public void Minimum()
		{
			var array = new[] { 1, 2, 3, 4, 5, 6 };

			Assert.Equal(1, array.Minimum());
			Assert.Equal(0, Enumerable<int>.Empty.Minimum());
			Assert.Equal(0, (null as IEnumerable<int>).Minimum());
		}

		[Fact]
		public void Neither()
		{
			var array = new[] { 1, 2, 3, 4, 5, 6 };

			Assert.Equal(Array<int>.Empty, array.Neither(array));
			Assert.True(new[] { 0, 1, 3, 5, 8 }.IsSet(array.Neither(new[] { 0, 2, 4, 6, 8 })));
			Assert.Equal(array, array.Neither(null));
		}

		[Fact]
		public void Skip()
		{
			var array = new[] { 1, 2, 3, 4, 5, 6 };

			Assert.Equal(new[] { 3, 4, 5, 6 }, array.Skip(2));
			Assert.Equal(Array<int>.Empty, array.Skip(6));
			Assert.Throws<IndexOutOfRangeException>(() => Enumerable<int>.Empty.Skip(3).ToArray());
		}

		[Fact]
		public void Sort()
		{
			var array = new[] { 1, 2, 3, 4, 5, 6 };

			Assert.Equal(array, this.GetItems().Sort());
			Assert.Equal(Enumerable<int>.Empty, Enumerable<int>.Empty.Sort());
		}

		[Fact]
		public void Take()
		{
			var array = new[] { 1, 2, 3, 4, 5, 6 };

			Assert.Equal(new[] { 1, 2 }, array.Take(2));
			Assert.Equal(array, array.Take(6));
			Assert.Throws<IndexOutOfRangeException>(() => Enumerable<int>.Empty.Take(3).ToArray());
		}

		[Fact]
		public void To()
		{
			var array = new[] { 1, 2, 3, 4, 5, 6 };
			var stringArray = new[] { "1", "2", "3", "4", "5", "6" };

			Assert.Empty((null as IEnumerable<int>).To(i => i.ToString()));
			Assert.Equal(stringArray, array.To(i => i.ToString()));
			Assert.Equal(stringArray, array.ToImmutableArray().To(i => i.ToString()));
			Assert.Equal(stringArray, new List<int>(array).To(i => i.ToString()));
			Assert.Equal(stringArray, this.GetItems().To(i => i.ToString()));
		}

		[Fact]
		public void ToArray()
		{
			var array = new[] { 1, 2, 3, 4, 5, 6 };

			Assert.Empty((null as IEnumerable<int>).ToArray());
			Assert.Equal(array, array.ToArray());
			Assert.NotStrictEqual(array, array.ToArray());
			Assert.Equal(array, array.ToImmutableArray().ToArray());
			Assert.Equal(array, new List<int>(array).ToArray());
			Assert.Equal(array, this.GetItems().ToArray());
		}

		[Fact]
		public async Task ToAsync()
		{
			var stringArray = new[] { "1", "2", "3", "4", "5", "6" };

			Assert.True(stringArray.IsSet(await this.GetItems().ToAsync(async i => await Task.FromResult(i.ToString())).ToListAsync()));
			Assert.Empty(await Enumerable<string>.Empty.ToAsync(async i => await Task.FromResult(i.ToString())).ToListAsync());
			await Assert.ThrowsAsync<ArgumentNullException>(async () => await this.GetItems().ToAsync<int, string>(null).ToListAsync());

			await Task.CompletedTask;
		}

		[Fact]
		public void ToCSV()
		{
			var array = new[] { 1, 2, 3, 4, 5, 6 };

			Assert.Equal("1,2,3,4,5,6", array.ToCSV());
			Assert.Equal("1,2,3,4,5,6", array.ToCSV(i => i.ToString()));
			Assert.Throws<ArgumentNullException>(() => array.ToCSV(null));
			Assert.Equal(@"""""""abc"""",""""def"""""",ghi", new[] { @"""abc"",""def""", "ghi" }.ToCSV());
		}

		[Fact]
		public void ToDictionary()
		{
			var array = new[] { 1, 2, 3, 4, 5, 6 };

			Assert.Equal(array.Length, array.To(i => KeyValuePair.Create(i, i.ToString())).ToDictionary().Count);
			Assert.Empty((null as IEnumerable<KeyValuePair<string, int>>).ToDictionary());

			Assert.Equal(array.Length, array.To(i => (i, i.ToString())).ToDictionary().Count);
			Assert.Empty((null as IEnumerable<(string, int)>).ToDictionary());

			Assert.Equal(array.Length, this.GetItems().ToDictionary(i => i.ToString()).Count);
			Assert.Empty((null as IEnumerable<KeyValuePair<string, int>>).ToDictionary(i => i.ToString()));
			Assert.Throws<ArgumentNullException>(() => this.GetItems().ToDictionary<int, string>(null));

			Assert.Equal(array.Length, this.GetItems().ToDictionary(i => i, i => i.ToString()).Count);
			Assert.Empty((null as IEnumerable<KeyValuePair<string, int>>).ToDictionary(i => i, i => i.ToString()));
			Assert.Throws<ArgumentNullException>(() => this.GetItems().ToDictionary<int, string>(null, i => i.ToString()));
			Assert.Throws<ArgumentNullException>(() => this.GetItems().ToDictionary(i => i, null as Func<int, string>));
		}

		[Fact]
		public void ToHashCode()
		{
			Assert.NotEqual(0, this.GetItems().ToHashCode());
			Assert.Equal(new HashCode().ToHashCode(), Enumerable<int>.Empty.ToHashCode());
		}

		[Fact]
		public void ToHashSet()
		{
			var array = new[] { 1, 2, 3, 4, 5, 6 };

			Assert.Equal(array, this.GetItems().ToHashSet(EqualityComparer<int>.Default));
			Assert.Empty(Enumerable<int>.Empty.ToHashSet());
			Assert.Empty((null as IEnumerable<int>).ToHashSet());
		}

		[Fact]
		public void ToImmutableQueue()
		{
			var array = new[] { 1, 2, 3, 4, 5, 6 };

			Assert.True(array.IsSequence(this.GetItems().ToImmutableQueue()));
			Assert.True(array.IsSequence(((IEnumerable<int>)array).ToImmutableQueue()));
			Assert.Empty(Enumerable<int>.Empty.ToImmutableQueue());
			Assert.Empty((null as IEnumerable<int>).ToImmutableQueue());
		}

		[Fact]
		public void ToImmutableStack()
		{
			var array = new[] { 1, 2, 3, 4, 5, 6 };

			Assert.True(array.ToStack().IsSequence(this.GetItems().ToImmutableStack()));
			Assert.True(array.ToStack().IsSequence(((IEnumerable<int>)array).ToImmutableStack()));
			Assert.Empty(Enumerable<int>.Empty.ToImmutableStack());
			Assert.Empty((null as IEnumerable<int>).ToImmutableStack());
		}

		[Fact]
		public void ToIndex()
		{
			Assert.Equal(new[] { 1, 3, 5 }, this.GetItems().ToIndex(i => i % 2 == 0));
			Assert.Equal(new[] { 0, 2, 4 }, this.GetItems().ToIndex(i => i % 2 == 1));
			Assert.Empty(Enumerable<int>.Empty.ToIndex(i => i % 2 == 1));
			Assert.Empty((null as int[]).ToIndex(i => i % 2 == 1));
			Assert.Throws<ArgumentNullException>(() => Enumerable<int>.Empty.ToIndex(null));
			Assert.Equal(new[] { 3 }, this.GetItems().ToIndex(4));
			Assert.Empty(this.GetItems().ToIndex(7));
			Assert.Empty(Enumerable<int>.Empty.ToIndex(1));
			Assert.Empty((null as int[]).ToIndex(1));
		}

		[Fact]
		public void ToList()
		{
			var array = new[] { 1, 2, 3, 4, 5, 6 };

			Assert.Equal(array, this.GetItems().ToList());
			Assert.True(array.IsSequence(this.GetItems().ToList()));
			Assert.Empty(Enumerable<int>.Empty.ToList());
			Assert.Empty((null as IEnumerable<int>).ToList());
		}

		[Fact]
		public void ToMany()
		{
			var array = new[] { 1, 2, 3, 4, 5, 6 };

			Assert.Equal(array, new[] { 1, 4 }.ToMany(i => new[] { i, i + 1, i + 2 }));
			Assert.Throws<ArgumentNullException>(() => new[] { 1, 4 }.ToMany(null as Func<int, IEnumerable<string>>).ToArray());
			Assert.Empty(Array<int>.Empty.ToMany(i => new[] { i, i + 1, i + 2 }));
			Assert.Empty((null as IEnumerable<int>).ToMany(i => new[] { i, i + 1, i + 2 }));
		}

		[Fact]
		public void ToQueue()
		{
			var array = new[] { 1, 2, 3, 4, 5, 6 };

			Assert.Equal(array, this.GetItems().ToQueue());
			Assert.True(array.IsSequence(this.GetItems().ToQueue()));
			Assert.Empty(Enumerable<int>.Empty.ToQueue());
			Assert.Empty((null as IEnumerable<int>).ToQueue());
		}

		[Fact]
		public void ToReadOnlySpan()
		{
			var array = new[] { 1, 2, 3, 4, 5, 6 };

			Assert.True((null as IEnumerable<int>).ToReadOnlySpan().IsEmpty);
			Assert.Equal(array.Length, array.ToImmutableArray().ToReadOnlySpan().Length);
			Assert.Equal(array.Length, array.ToReadOnlySpan().Length);
			Assert.Equal(array.Length, array.ToList().ToReadOnlySpan().Length);
			Assert.Equal(array.Length, this.GetItems().ToReadOnlySpan().Length);
		}

		[Fact]
		public void ToSpan()
		{
			var array = new[] { 1, 2, 3, 4, 5, 6 };

			Assert.True((null as IEnumerable<int>).ToSpan().IsEmpty);
			Assert.Equal(array.Length, array.ToSpan().Length);
			Assert.Equal(array.Length, array.ToList().ToSpan().Length);
			Assert.Equal(array.Length, this.GetItems().ToSpan().Length);
		}

		[Fact]
		public void ToStack()
		{
			var array = new[] { 1, 2, 3, 4, 5, 6 };

			array.Reverse();
			Assert.Equal(array, this.GetItems().ToStack());
			Assert.True(array.IsSequence(this.GetItems().ToStack()));
			Assert.Empty(Enumerable<int>.Empty.ToStack());
			Assert.Empty((null as IEnumerable<int>).ToStack());
		}

		[Fact]
		public void Union()
		{
			var array = new[] { 1, 2, 3, 4, 5, 6 };

			Assert.True(array.IsSet(new[] { 1, 2 }.Union(new[] { 3, 4 }).Union(new[] { 5, 6 })));
			Assert.True(array.IsSet(array.Union(Array<int>.Empty)));
			Assert.True(array.IsSet(array.Union(null)));
			Assert.Empty(Enumerable<string>.Empty.Union(Enumerable<string>.Empty));
			Assert.Empty((null as IEnumerable<string>).Union(Enumerable<string>.Empty));
		}

		[Fact]
		public void Without()
		{
			var array = new[] { 1, 2, 3, 4, 5, 6 };

			Assert.True(array.IsSet(array.Without(new[] { 7, 8, 9, 0 })));
			Assert.True(array.IsSet(new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8 }.Without(new[] { 7, 8, 9, 0 })));
			Assert.True(array.IsSet(array.Without(Array<int>.Empty)));
			Assert.True(array.IsSet(array.Without(null)));
			Assert.Empty(Enumerable<string>.Empty.Without(Enumerable<string>.Empty));
			Assert.Empty((null as IEnumerable<string>).Without(Enumerable<string>.Empty));
		}
	}
}
