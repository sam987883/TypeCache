// Copyright (c) 2021 Samuel Abraham

using System.Collections.Generic;
using TypeCache.Collections.Extensions;
using Xunit;

namespace TypeCache.Tests.Collections.Extensions
{
	public class EnumeratorExtensions
	{
		private IEnumerable<int> GetInts()
		{
			yield return 1;
			yield return 2;
			yield return 3;
			yield return 4;
			yield return 5;
			yield return 6;
			yield return 7;
		}

		private IEnumerable<string> GetStrings()
		{
			yield return "aaa";
			yield return "";
			yield return null;
			yield return "aaa";
			yield return "Aaa";
			yield return "BBB";
			yield return "CcC";
		}

		[Fact]
		public void Count()
		{
			Assert.Equal(7, this.GetInts().GetEnumerator().Count());
			Assert.Equal(7, this.GetStrings().GetEnumerator().Count());
		}

		[Fact]
		public void Deconstruct()
		{
			{
				var (value1, _) = this.GetInts().GetEnumerator();
				Assert.Equal(1, value1);
			}
			{
				var (value1, value2, _) = this.GetInts().GetEnumerator();
				Assert.Equal(1, value1);
				Assert.Equal(2, value2);
			}
			{
				var (value1, value2, value3, _) = this.GetInts().GetEnumerator();
				Assert.Equal(1, value1);
				Assert.Equal(2, value2);
				Assert.Equal(3, value3);
			}
			{
				var (value1, value2, value3, _) = ((IEnumerable<int>)new[] { 1, 2 }).GetEnumerator();
				Assert.Equal(1, value1);
				Assert.Equal(2, value2);
				Assert.Null(value3);
			}
			{
				var (item1, _) = this.GetStrings().GetEnumerator();
				Assert.Equal("aaa", item1);
			}
			{
				var (item1, item2, _) = this.GetStrings().GetEnumerator();
				Assert.Equal("aaa", item1);
				Assert.Equal(string.Empty, item2);
			}
			{
				var (item1, item2, item3, _) = this.GetStrings().GetEnumerator();
				Assert.Equal("aaa", item1);
				Assert.Equal(string.Empty, item2);
				Assert.Equal(null, item3);
			}
			{
				var (item1, item2, item3, _) = ((IEnumerable<string>)new[] { "aaa" }).GetEnumerator();
				Assert.Equal("aaa", item1);
				Assert.Null(item2);
				Assert.Null(item3);
			}
		}

		[Fact]
		public void Get()
		{
			Assert.Equal("Aaa", this.GetStrings().GetEnumerator().Get(4));
			Assert.Null(this.GetStrings().GetEnumerator().Get(10));
		}

		[Fact]
		public void GetValue()
		{
			Assert.Equal(5, this.GetInts().GetEnumerator().GetValue(4));
			Assert.Null(this.GetInts().GetEnumerator().GetValue(10));
		}

		[Fact]
		public void Next()
		{
			var enumerator = this.GetStrings().GetEnumerator();
			var list = new List<string>();

			while (enumerator.Next() is not null)
				list.Add(enumerator.Current);

			Assert.Equal(2, list.Count);
		}

		[Fact]
		public void NextValue()
		{
			var enumerator = this.GetInts().GetEnumerator();
			var list = new List<int>();

			while (enumerator.NextValue().HasValue)
				list.Add(enumerator.Current);

			Assert.Equal(7, list.Count);
		}

		[Fact]
		public void Rest()
		{
			var enumerator = this.GetInts().GetEnumerator();
			var i = 0;
			while (++i < 4)
				enumerator.MoveNext();

			Assert.Equal(new[] { 4, 5, 6, 7 }, enumerator.Rest().ToArray());
			Assert.Empty(enumerator.Rest());
		}

		[Fact]
		public void Skip()
		{
			var enumerator = this.GetInts().GetEnumerator();
			enumerator.Skip(4);

			Assert.Equal(new[] { 5, 6, 7 }, enumerator.Rest().ToArray());
			Assert.False(enumerator.Skip(1));
			Assert.True(enumerator.Skip(0));
		}
	}
}
