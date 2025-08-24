// Copyright (c) 2021 Samuel Abraham

using System;
using System.Reflection;
using TypeCache.Reflection;
using Xunit;

namespace TypeCache.Tests.Utilities;

public class EnumTests
{
	[Fact]
	public void Enum_BindingFlags()
	{
		Assert.Equal(typeof(BindingFlags).GetCustomAttributes<Attribute>(), Enum<BindingFlags>.Attributes);
		Assert.True(Enum<BindingFlags>.Flags);
		Assert.Equal(typeof(BindingFlags).Name, Enum<BindingFlags>.Name);
		Assert.Equal(ScalarType.Int32, Enum<BindingFlags>.UnderlyingType);
	}

	[Fact]
	public void Enum_ScalarType()
	{
		Assert.Equal(typeof(ScalarType).GetCustomAttributes<Attribute>(), Enum<ScalarType>.Attributes);
		Assert.Empty(Enum<ScalarType>.Attributes);
		Assert.False(Enum<ScalarType>.Flags);
		Assert.Equal(typeof(ScalarType).Name, Enum<ScalarType>.Name);
		Assert.Equal(ScalarType.Int32, Enum<ScalarType>.UnderlyingType);
	}
}
