// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using TypeCache.Collections;
using TypeCache.Collections.Extensions;
using TypeCache.Extensions;
using Xunit;

namespace TypeCache.Tests.Collections.Extensions
{
	public class IEnumerableStringExtensions
	{
		private IEnumerable<string> GetStrings()
		{
			yield return "  ";
			yield return "";
			yield return null;
			yield return "aaa";
			yield return "BBB";
			yield return "CcC";
		}

		[Fact]
		public void Group()
		{
			var group = this.GetStrings().Group(_ => !_.IsBlank() ? _[0].ToString() : "_", StringComparison.Ordinal);
			Assert.Contains("_", group.Keys);
			Assert.Contains("a", group.Keys);
			Assert.Contains("B", group.Keys);
			Assert.Contains("C", group.Keys);
			Assert.Equal(4, group.Count());
			Assert.Throws<ArgumentNullException>(() => this.GetStrings().Group(null));
		}

		[Fact]
		public void Has()
		{
			var array = this.GetStrings().ToArray();

			Assert.True(array.Has("aaa"));
			Assert.False(array.Has("AAA", StringComparison.Ordinal));
			Assert.True(array.Has(null as string));
			Assert.False((null as IEnumerable<string>).Has("CcC"));

			Assert.True(array.Has(new[] { "aaa", "BBB" }));
			Assert.False(array.Has(new[] { "AAA", "BBB" }, StringComparison.Ordinal));
			Assert.True(array.Has(null as IEnumerable<string>));
			Assert.False((null as IEnumerable<string>).Has(new[] { "aaa", "BBB" }));
		}

		[Fact]
		public void IfNotBlank()
		{
			Assert.Equal(new[] { "aaa", "BBB", "CcC" }, this.GetStrings().IfNotBlank());
			Assert.Empty(Enumerable<string>.Empty.IfNotBlank());
			Assert.Empty((null as IEnumerable<string>).IfNotBlank());
		}

		[Fact]
		public void IsSequence()
		{
			Assert.True(this.GetStrings().IsSequence(this.GetStrings(), StringComparison.Ordinal));
			Assert.True(Enumerable<string>.Empty.IsSequence(null, StringComparison.Ordinal));
			Assert.True((null as IEnumerable<string>).IsSequence(Enumerable<string>.Empty, StringComparison.Ordinal));
			Assert.True((null as IEnumerable<string>).IsSequence(null, StringComparison.Ordinal));
		}

		[Fact]
		public void IsSet()
		{
			var array = this.GetStrings().ToArray();

			Assert.True(array.IsSet(this.GetStrings(), StringComparison.Ordinal));
			Assert.True(Enumerable<string>.Empty.IsSet(null, StringComparison.Ordinal));
			Assert.True((null as IEnumerable<string>).IsSet(Enumerable<string>.Empty, StringComparison.Ordinal));
			Assert.True((null as IEnumerable<string>).IsSet(null, StringComparison.Ordinal));
		}

		[Fact]
		public void Match()
		{
			var array = this.GetStrings().ToArray();

			Assert.Equal(array, this.GetStrings().Match(array));
			Assert.True(new[] { "aaa", "bbb" }.IsSet(this.GetStrings().Match(new[] { "aaa", "bbb" }, StringComparison.OrdinalIgnoreCase)));
			Assert.Equal(Enumerable<string>.Empty, this.GetStrings().Match(null));
		}

		[Fact]
		public void Neither()
		{
			var array = this.GetStrings().ToArray();

			Assert.Equal(Array<string>.Empty, this.GetStrings().Neither(array));
			Assert.True(this.GetStrings().Neither(new[] { "aaa", "bbb" }).SetEquals(new[] { "  ", "", null, "CcC" }));
			Assert.Equal(array, this.GetStrings().Neither(null));
		}

		[Fact]
		public void Sort()
		{
			Assert.Null(this.GetStrings().Sort(StringComparison.OrdinalIgnoreCase)[0]);
			Assert.Equal("CcC", this.GetStrings().Sort(StringComparison.OrdinalIgnoreCase)[5]);
		}

		[Fact]
		public void ToCSV()
		{
			Assert.Equal(",,,aaa,BBB,CcC", this.GetStrings().ToCSV());
			Assert.Equal(",,,AAA,BBB,CCC", this.GetStrings().ToCSV(_ => _?.Trim().ToUpperInvariant()));
			Assert.Throws<ArgumentNullException>(() => this.GetStrings().ToCSV(null));
			Assert.Equal(@"""""""abc"""",""""def"""""",ghi", ((IEnumerable<string>)new[] { @"""abc"",""def""", "ghi" }).ToCSV());
		}

		[Fact]
		public void ToDictionary()
		{
			var array = this.GetStrings().ToArray();

			Assert.Equal(array.Length - 1, array.IfNotNull().To(_ => KeyValuePair.Create(_, _)).ToDictionary().Count);
			Assert.Empty((null as IEnumerable<KeyValuePair<string, int>>).ToDictionary());

			Assert.Equal(array.Length - 1, array.IfNotNull().To(_ => (_, _)).ToDictionary().Count);
			Assert.Empty((null as IEnumerable<(string, int)>).ToDictionary());

			Assert.Equal(array.Length - 1, this.GetStrings().IfNotNull().ToDictionary(_ => _, StringComparison.Ordinal).Count);
			Assert.Empty((null as IEnumerable<KeyValuePair<string, int>>).ToDictionary(StringComparison.Ordinal));

			Assert.Equal(array.Length - 1, this.GetStrings().IfNotNull().ToDictionary(_ => _, _ => _, StringComparison.Ordinal).Count);
			Assert.Empty((null as IEnumerable<KeyValuePair<string, int>>).ToDictionary(StringComparison.Ordinal));
			Assert.Throws<ArgumentNullException>(() => this.GetStrings().ToDictionary<string, string>(null, _ => _, StringComparison.Ordinal));
			Assert.Throws<ArgumentNullException>(() => this.GetStrings().ToDictionary(i => i, null as Func<string, object>, StringComparison.Ordinal));
		}

		[Fact]
		public void ToHashSet()
		{
			var array = this.GetStrings().ToArray();

			Assert.True(array.IsSet(this.GetStrings().ToHashSet(StringComparison.Ordinal), StringComparison.Ordinal));
			Assert.Empty(Enumerable<string>.Empty.ToHashSet(StringComparison.Ordinal));
			Assert.Empty((null as IEnumerable<string>).ToHashSet(StringComparison.Ordinal));
		}
	}
}
