﻿// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TypeCache.Extensions;
using Xunit;

namespace TypeCache.Tests.Extensions;

public class ListExtensions
{
	[Fact]
	public void AddIfNotBlank()
	{
		var list = new List<string>(1);
		list.AddIfNotBlank(" \n \r ");

		Assert.Empty(list);

		list.AddIfNotBlank("AAA");
		Assert.Equal(1, list.Count);
	}

	[Fact]
	public void AddIfNotNull()
	{
		var list = new List<string>(1);
		list.AddIfNotNull(null);

		Assert.Empty(list);

		list.AddIfNotNull(string.Empty);
		Assert.Equal(1, list.Count);
	}

	[Fact]
	public void InsertIfNotBlank()
	{
		var list = new List<string> { "1", "2", "4" };
		list.InsertIfNotBlank(2, " \n \r ");

		Assert.Equal(3, list.Count);

		list.InsertIfNotBlank(2, "AAA");
		Assert.Equal(4, list.Count);
	}

	[Fact]
	public void InsertIfNotNull()
	{
		var list = new List<string> { "1", "2", "4" };
		list.InsertIfNotNull(2, null);

		Assert.Equal(3, list.Count);

		list.InsertIfNotNull(2, string.Empty);
		Assert.Equal(4, list.Count);
	}
}
