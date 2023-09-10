// Copyright (c) 2021 Samuel Abraham

using System;
using System.Linq;
using System.Reflection;
using TypeCache.Extensions;
using TypeCache.Utilities;
using Xunit;

namespace TypeCache.Tests.Utilities;

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
		Assert.Equal(ScalarType.Int32, EnumOf<BindingFlags>.UnderlyingType.GetDataType());
		Assert.Equal(typeof(int).TypeHandle, EnumOf<BindingFlags>.UnderlyingType.TypeHandle);
	}

	[Fact]
	public void EnumOfDataType()
	{
		Assert.Empty(EnumOf<ScalarType>.Attributes);
		Assert.NotNull(EnumOf<ScalarType>.Comparer);
		Assert.False(EnumOf<ScalarType>.Flags);
		Assert.Equal(typeof(ScalarType).Name(), EnumOf<ScalarType>.Name);
		Assert.Equal(Enum.GetValues(typeof(ScalarType)), EnumOf<ScalarType>.Tokens.Keys.ToArray());
		Assert.Equal(Enum.GetNames(typeof(ScalarType)), EnumOf<ScalarType>.Tokens.Values.Select(_ => _.Name).ToArray(), StringComparer.Ordinal);
		Assert.Equal(ScalarType.Int32, EnumOf<ScalarType>.UnderlyingType.GetDataType());
		Assert.Equal(typeof(int).TypeHandle, EnumOf<ScalarType>.UnderlyingType.TypeHandle);
	}
}
