// Copyright (c) 2021 Samuel Abraham

using System;
using System.Linq;
using System.Reflection;
using TypeCache.Extensions;
using TypeCache.Utilities;
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
		Assert.Equal(typeof(BindingFlags).Name(), EnumOf<BindingFlags>.Name);
		Assert.Equal(Enum.GetValues(typeof(BindingFlags)), EnumOf<BindingFlags>.Tokens.Keys.ToArray());
		Assert.Equal(Enum.GetNames(typeof(BindingFlags)), EnumOf<BindingFlags>.Tokens.Values.Select(_ => _.Name).ToArray(), StringComparer.Ordinal);
		Assert.Equal(SystemType.Int32, EnumOf<BindingFlags>.UnderlyingType.GetSystemType());
		Assert.Equal(typeof(int).TypeHandle, EnumOf<BindingFlags>.UnderlyingType.TypeHandle);
	}

	[Fact]
	public void EnumOfKindType()
	{
		Assert.Empty(EnumOf<Kind>.Attributes);
		Assert.NotNull(EnumOf<Kind>.Comparer);
		Assert.False(EnumOf<Kind>.Flags);
		Assert.Equal(typeof(Kind).Name(), EnumOf<Kind>.Name);
		Assert.Equal(Enum.GetValues(typeof(Kind)), EnumOf<Kind>.Tokens.Keys.ToArray());
		Assert.Equal(Enum.GetNames(typeof(Kind)), EnumOf<Kind>.Tokens.Values.Select(_ => _.Name).ToArray(), StringComparer.Ordinal);
		Assert.Equal(SystemType.Int32, EnumOf<Kind>.UnderlyingType.GetSystemType());
		Assert.Equal(typeof(int).TypeHandle, EnumOf<Kind>.UnderlyingType.TypeHandle);
	}

	[Fact]
	public void EnumOfSystemType()
	{
		Assert.Empty(EnumOf<SystemType>.Attributes);
		Assert.NotNull(EnumOf<SystemType>.Comparer);
		Assert.False(EnumOf<SystemType>.Flags);
		Assert.Equal(typeof(SystemType).Name(), EnumOf<SystemType>.Name);
		Assert.Equal(Enum.GetValues(typeof(SystemType)), EnumOf<SystemType>.Tokens.Keys.ToArray());
		Assert.Equal(Enum.GetNames(typeof(SystemType)), EnumOf<SystemType>.Tokens.Values.Select(_ => _.Name).ToArray(), StringComparer.Ordinal);
		Assert.Equal(SystemType.Int32, EnumOf<SystemType>.UnderlyingType.GetSystemType());
		Assert.Equal(typeof(int).TypeHandle, EnumOf<SystemType>.UnderlyingType.TypeHandle);
	}
}
