// Copyright (c) 2021 Samuel Abraham

using System;
using System.Linq;
using System.Reflection;
using TypeCache.Extensions;
using TypeCache.Reflection;
using Xunit;

namespace TypeCache.Tests;

public class EnumOfTests
{
	[Fact]
	public void EnumOfBindingFlags()
	{
		Assert.Equal(1, EnumOf<BindingFlags>.Attributes.Count);
		Assert.NotNull(EnumOf<BindingFlags>.Comparer);
		Assert.True(EnumOf<BindingFlags>.Flags);
		Assert.Equal(typeof(BindingFlags).TypeHandle, EnumOf<BindingFlags>.Handle);
		Assert.False(EnumOf<BindingFlags>.Internal);
		Assert.Equal(typeof(BindingFlags).Name, EnumOf<BindingFlags>.Name);
		Assert.True(EnumOf<BindingFlags>.Public);
		Assert.Equal(Enum.GetValues(typeof(BindingFlags)), EnumOf<BindingFlags>.Tokens.Select(_ => _.Value).ToArray());
		Assert.Equal(Enum.GetNames(typeof(BindingFlags)), EnumOf<BindingFlags>.Tokens.Select(_ => _.Name).ToArray(), StringComparer.Ordinal);
		Assert.Equal(SystemType.Int32, EnumOf<BindingFlags>.UnderlyingType.SystemType);
		Assert.Equal(typeof(int).TypeHandle, EnumOf<BindingFlags>.UnderlyingType.TypeHandle);
	}

	[Fact]
	public void EnumOfKindType()
	{
		Assert.Empty(EnumOf<Kind>.Attributes);
		Assert.NotNull(EnumOf<Kind>.Comparer);
		Assert.False(EnumOf<Kind>.Flags);
		Assert.Equal(typeof(Kind).TypeHandle, EnumOf<Kind>.Handle);
		Assert.False(EnumOf<Kind>.Internal);
		Assert.Equal(typeof(Kind).Name, EnumOf<Kind>.Name);
		Assert.True(EnumOf<Kind>.Public);
		Assert.Equal(Enum.GetValues(typeof(Kind)), EnumOf<Kind>.Tokens.Select(_ => _.Value).ToArray());
		Assert.Equal(Enum.GetNames(typeof(Kind)), EnumOf<Kind>.Tokens.Select(_ => _.Name).ToArray(), StringComparer.Ordinal);
		Assert.Equal(SystemType.Int32, EnumOf<Kind>.UnderlyingType.SystemType);
		Assert.Equal(typeof(int).TypeHandle, EnumOf<Kind>.UnderlyingType.TypeHandle);
	}

	[Fact]
	public void EnumOfSystemType()
	{
		Assert.Empty(EnumOf<SystemType>.Attributes);
		Assert.NotNull(EnumOf<SystemType>.Comparer);
		Assert.False(EnumOf<SystemType>.Flags);
		Assert.Equal(typeof(SystemType).TypeHandle, EnumOf<SystemType>.Handle);
		Assert.False(EnumOf<SystemType>.Internal);
		Assert.Equal(typeof(SystemType).Name, EnumOf<SystemType>.Name);
		Assert.True(EnumOf<SystemType>.Public);
		Assert.Equal(Enum.GetValues(typeof(SystemType)), EnumOf<SystemType>.Tokens.Select(_ => _.Value).ToArray());
		Assert.Equal(Enum.GetNames(typeof(SystemType)), EnumOf<SystemType>.Tokens.Select(_ => _.Name).ToArray(), StringComparer.Ordinal);
		Assert.Equal(SystemType.Int32, EnumOf<SystemType>.UnderlyingType.SystemType);
		Assert.Equal(typeof(int).TypeHandle, EnumOf<SystemType>.UnderlyingType.TypeHandle);
	}
}
