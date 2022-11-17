// Copyright (c) 2021 Samuel Abraham

using System.Collections.Generic;
using TypeCache.Collections.Extensions;
using TypeCache.Extensions;
using Xunit;

namespace TypeCache.Tests.Collections.Extensions;

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
	public void Get()
	{
		Assert.Equal("Aaa", this.GetStrings().GetEnumerator().Get(4));
		Assert.Null(this.GetStrings().GetEnumerator().Get(10));
	}

	[Fact]
	public void GetValue()
	{
		Assert.Equal(5, this.GetInts().GetEnumerator().Get(4));
		Assert.Equal(default, this.GetInts().GetEnumerator().Get(10));
	}

	[Fact]
	public void Move()
	{
		var enumerator = this.GetInts().GetEnumerator();

		Assert.True(enumerator.Move(4));
		Assert.Equal(new[] { 5, 6, 7 }, enumerator.Rest().ToArray());
		Assert.False(enumerator.Move(1));
		Assert.True(enumerator.Move(0));
	}

	[Fact]
	public void Next()
	{
		var enumerator = this.GetStrings().GetEnumerator();
		var list = new List<string>(2);

		while (enumerator.Next() is not null)
			list.Add(enumerator.Current);

		Assert.Equal(2, list.Count);
	}

	[Fact]
	public void NextValue()
	{
		var enumerator = this.GetInts().GetEnumerator();
		var list = new List<int>(7);

		while (enumerator.IfNext(out var item))
			list.Add(item);

		Assert.Equal(7, list.Count);
	}

	[Fact]
	public void Rest()
	{
		var enumerator = (1..7).Values().GetEnumerator();

		Assert.True(enumerator.Move(4));
		Assert.Equal(new[] { 5, 6, 7 }, enumerator.Rest().ToArray());
		Assert.Empty(enumerator.Rest());
	}
}
