// Copyright (c) 2021 Samuel Abraham

using System;
using TypeCache.Reflection;
using Xunit;

namespace TypeCache.Tests.Reflection;

public class ClrTypeTests
{
	[Fact]
	public void ClrType_Enum_Values()
	{
		var values = System.Enum.GetValues(typeof(ClrType));

		Assert.NotEmpty(values);
		Assert.Equal(5, values.Length);
	}

	[Fact]
	public void ClrType_Class()
	{
		Assert.Equal(0, (int)ClrType.Class);
	}

	[Fact]
	public void ClrType_Struct()
	{
		Assert.True(System.Enum.IsDefined(typeof(ClrType), ClrType.Struct));
	}

	[Fact]
	public void ClrType_Enum()
	{
		Assert.True(System.Enum.IsDefined(typeof(ClrType), ClrType.Enum));
	}

	[Fact]
	public void ClrType_Interface()
	{
		Assert.True(System.Enum.IsDefined(typeof(ClrType), ClrType.Interface));
	}

	[Fact]
	public void ClrType_Delegate()
	{
		Assert.True(System.Enum.IsDefined(typeof(ClrType), ClrType.Delegate));
	}
}
