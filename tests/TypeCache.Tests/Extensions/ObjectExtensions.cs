// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using TypeCache.Extensions;
using Xunit;

namespace TypeCache.Tests.Extensions;

public class ObjectExtensions
{
	public void ThrowIfEqual()
	{
		var list = new List<int>(0);
		Assert.Throws<ArgumentOutOfRangeException>(() => list.ThrowIfEqual(list));
		new List<int>(0).ThrowIfEqual(new List<int>(0));
	}

	public void ThrowIfNotEqual()
	{
		var list = new List<string>(0);
		list.ThrowIfNotEqual(list);
		Assert.Throws<ArgumentOutOfRangeException>(() => new List<string>(0).ThrowIfNotEqual(new List<string>(0)));
	}

	public void ThrowIfNot()
	{
		true.ThrowIfNot<bool>();
		999.ThrowIfNot<int>();
		"AAA".ThrowIfNot<string>();
		Assert.Throws<ArgumentOutOfRangeException>(() => true.ThrowIfNot<object>());
		Assert.Throws<ArgumentOutOfRangeException>(() => 999.ThrowIfNot<object>());
		Assert.Throws<ArgumentOutOfRangeException>(() => "AAA".ThrowIfNot<object>());
	}

	public void ThrowIfNotSame()
	{
		var item = new object();
		item.ThrowIfNotReferenceEqual(item);
		Assert.Throws<ArgumentOutOfRangeException>(() => new object().ThrowIfNotReferenceEqual(new object()));
	}

	public void ThrowIfNull()
	{
		((int?)123456).ThrowIfNull();
		"AAA".ThrowIfNull();
		Assert.Throws<ArgumentNullException>(() => (null as string).ThrowIfNull());
		Assert.Throws<ArgumentNullException>(() => (null as int?).ThrowIfNull());
	}

	public void ThrowIfSame()
	{
		var item = new object();
		Assert.Throws<ArgumentOutOfRangeException>(() => item.ThrowIfReferenceEqual(item));
		new object().ThrowIfReferenceEqual(new object());
	}
}
