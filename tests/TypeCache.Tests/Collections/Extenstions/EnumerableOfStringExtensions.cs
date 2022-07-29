// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using TypeCache.Collections;
using TypeCache.Collections.Extensions;
using TypeCache.Extensions;
using Xunit;

namespace TypeCache.Tests.Collections.Extensions;

public class EnumerableOfStringExtensions
{
	private IEnumerable<string> GetStrings()
	{
		yield return "  ";
		yield return "";
		yield return null;
		yield return "aaa";
		yield return "Aaa";
		yield return "BBB";
		yield return "CcC";
	}

	[Fact]
	public void Group()
	{
		var group = this.GetStrings().Group(_ => _.IsNotBlank() ? _[0].ToString() : "_", StringComparison.Ordinal);
		Assert.Contains("_", group.Keys);
		Assert.Contains("a", group.Keys);
		Assert.Contains("B", group.Keys);
		Assert.Contains("C", group.Keys);
		Assert.Equal(5, group.Count());
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
		Assert.Equal(new[] { "aaa", "Aaa", "BBB", "CcC" }, this.GetStrings().IfNotBlank());
		Assert.Empty(Array<string>.Empty.IfNotBlank());
		Assert.Empty((null as IEnumerable<string>).IfNotBlank());
	}

	[Fact]
	public void IsSequence()
	{
		Assert.True(this.GetStrings().IsSequence(this.GetStrings(), StringComparison.Ordinal));
		Assert.True(Array<string>.Empty.IsSequence(Array<string>.Empty, StringComparison.Ordinal));
		Assert.False(Array<string>.Empty.IsSequence(null, StringComparison.Ordinal));
		Assert.False((null as IEnumerable<string>).IsSequence(Array<string>.Empty, StringComparison.Ordinal));
		Assert.True((null as IEnumerable<string>).IsSequence(null, StringComparison.Ordinal));
	}

	[Fact]
	public void IsSet()
	{
		var array = this.GetStrings().ToArray();

		Assert.True(array.IsSet(this.GetStrings(), StringComparison.Ordinal));
		Assert.True(Array<string>.Empty.IsSet(null, StringComparison.Ordinal));
		Assert.True((null as IEnumerable<string>).IsSet(Array<string>.Empty, StringComparison.Ordinal));
		Assert.True((null as IEnumerable<string>).IsSet(null, StringComparison.Ordinal));
	}

	[Fact]
	public void Match()
	{
		var array = this.GetStrings().ToArray();

		Assert.Equal(array, this.GetStrings().Match(array, StringComparison.Ordinal));
		Assert.NotEqual(array, this.GetStrings().Match(array));
		Assert.True(new[] { "aaa", "bbb" }.IsSet(this.GetStrings().Match(new[] { "aaa", "bbb" }, StringComparison.OrdinalIgnoreCase)));
		Assert.Equal(Array<string>.Empty, this.GetStrings().Match(null));
	}

	[Fact]
	public void NotMatch()
	{
		var array = this.GetStrings().ToArray();

		Assert.Equal(Array<string>.Empty, this.GetStrings().NotMatch(array));
		Assert.True(this.GetStrings().NotMatch(new[] { "aaa", "bbb" }).SetEquals(new[] { "  ", "", null, "CcC" }));
		Assert.Equal(array, this.GetStrings().NotMatch(Array<string>.Empty, StringComparison.Ordinal));
		Assert.NotEqual(array, this.GetStrings().NotMatch(Array<string>.Empty));
		Assert.Equal(array, this.GetStrings().NotMatch(null, StringComparison.Ordinal));
		Assert.NotEqual(array, this.GetStrings().NotMatch(null));
	}

	[Fact]
	public void Sort()
	{
		Assert.Null(this.GetStrings().Sort(StringComparison.OrdinalIgnoreCase)[0]);
		Assert.Equal("BBB", this.GetStrings().Sort(StringComparison.OrdinalIgnoreCase)[5]);
	}

	[Fact]
	public void ToCSV()
	{
		Assert.Equal("  , , , aaa, Aaa, BBB, CcC", this.GetStrings().ToCSV());
		Assert.Equal(", , , AAA, AAA, BBB, CCC", this.GetStrings().ToCSV(_ => _?.Trim().ToUpperInvariant()));
		Assert.Throws<ArgumentNullException>(() => this.GetStrings().ToCSV(null));
		Assert.Equal(@"""""""abc"""",""""def"""""",ghi", ((IEnumerable<string>)new[] { @"""abc"",""def""", "ghi" }).ToCSV(true));
	}

	[Fact]
	public void ToDictionary()
	{
		var array = this.GetStrings().ToArray();

		Assert.Equal(array.Length - 1, array.IfNotNull().Map(_ => KeyValuePair.Create(_, _)).ToDictionary(StringComparison.Ordinal).Count);
		Assert.Throws<ArgumentException>(() => array.IfNotNull().Map(_ => KeyValuePair.Create(_, _)).ToDictionary());
		Assert.Empty((null as IEnumerable<KeyValuePair<string, int>>).ToDictionary());

		Assert.Equal(array.Length - 1, array.IfNotNull().Map(_ => Tuple.Create(_, _)).ToDictionary(StringComparison.Ordinal).Count);
		Assert.Throws<ArgumentException>(() => array.IfNotNull().Map(_ => Tuple.Create(_, _)).ToDictionary());
		Assert.Empty((null as IEnumerable<(string, int)>).ToDictionary());

		Assert.Equal(array.Length - 1, array.IfNotNull().Map(_ => (_, _)).ToDictionary(StringComparison.Ordinal).Count);
		Assert.Throws<ArgumentException>(() => array.IfNotNull().Map(_ => (_, _)).ToDictionary());
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
		Assert.Empty(Array<string>.Empty.ToHashSet(StringComparison.Ordinal));
		Assert.Empty((null as IEnumerable<string>).ToHashSet(StringComparison.Ordinal));
	}

	[Fact]
	public void ToImmutableDictionary()
	{
		var pairs = this.GetStrings().IfNotNull().Map(value => KeyValuePair.Create(value, 1));
		var immutableDictionary1 = pairs.ToImmutableDictionary(StringComparison.Ordinal);
		var immutableDictionary2 = pairs.ToImmutableDictionary(pair => pair.Key, StringComparison.Ordinal);
		var immutableDictionary3 = pairs.ToImmutableDictionary(pair => pair.Key, pair => pair.Value, StringComparison.Ordinal);
	}

	[Fact]
	public void ToImmutableHashSet()
	{
		Assert.True(this.GetStrings().IsSet(this.GetStrings().ToImmutableHashSet(StringComparison.Ordinal)));
		Assert.False(this.GetStrings().IsSet(this.GetStrings().ToImmutableHashSet(StringComparison.OrdinalIgnoreCase), StringComparison.Ordinal));
	}

	[Fact]
	public void ToImmutableSortedDictionary()
	{
		Assert.NotEmpty(this.GetStrings().IfNotNull().Map(_ => KeyValuePair.Create(_, 1)).ToImmutableSortedDictionary(StringComparison.Ordinal));
	}

	[Fact]
	public void ToImmutableSortedSet()
	{
		Assert.NotEmpty(this.GetStrings().ToImmutableSortedSet(StringComparison.Ordinal));
		Assert.NotEqual(this.GetStrings().ToImmutableSortedSet(StringComparison.Ordinal), this.GetStrings().ToImmutableSortedSet(StringComparison.OrdinalIgnoreCase));
	}

	[Fact]
	public void ToIndex()
	{
		Assert.Equal(1, this.GetStrings().ToIndex(string.Empty).First());
		Assert.Equal(2, this.GetStrings().ToIndex(null as string).First());
		Assert.Equal(3, this.GetStrings().ToIndex("aaa", StringComparison.Ordinal).First());
		Assert.Equal(4, this.GetStrings().ToIndex("Aaa", StringComparison.Ordinal).First());
		Assert.Equal(5, this.GetStrings().ToIndex("BBB").First());
	}

	[Fact]
	public void Union()
	{
		var array = this.GetStrings().ToArray();

		Assert.Equal(array, this.GetStrings().Union(array, StringComparison.Ordinal));
		Assert.NotEqual(array, this.GetStrings().Union(array));
		Assert.Equal(array, this.GetStrings().Union(Array<string>.Empty, StringComparison.Ordinal));
		Assert.NotEqual(array, this.GetStrings().Union(Array<string>.Empty));
		Assert.Equal(array, this.GetStrings().Union(null, StringComparison.Ordinal));
		Assert.NotEqual(array, this.GetStrings().Union(null));
		Assert.NotEqual(array, this.GetStrings().Union(new[] { "1111" }, StringComparison.Ordinal));
		Assert.NotEqual(array, this.GetStrings().Union(new[] { "1111" }, StringComparison.OrdinalIgnoreCase));
	}

	[Fact]
	public void Without()
	{
		var array = this.GetStrings().ToArray();

		Assert.Equal(Array<string>.Empty, this.GetStrings().Without(array));
		Assert.True(this.GetStrings().Without(new[] { "aaa", "bbb" }).SetEquals(new[] { "  ", "", null, "CcC" }));
		Assert.Equal(array, this.GetStrings().Without(Array<string>.Empty, StringComparison.Ordinal));
		Assert.NotEqual(array, this.GetStrings().Without(Array<string>.Empty));
		Assert.Equal(array, this.GetStrings().Without(null, StringComparison.Ordinal));
		Assert.NotEqual(array, this.GetStrings().Without(null));
	}
}
