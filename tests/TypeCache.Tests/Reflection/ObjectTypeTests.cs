// Copyright (c) 2021 Samuel Abraham

using TypeCache.Reflection;
using Xunit;

namespace TypeCache.Tests.Reflection;

public class ObjectTypeTests
{
	[Fact]
	public void ObjectType_Enum_Values()
	{
		var values = Enum.GetValues(typeof(ObjectType));

		Assert.NotEmpty(values);
	}

	[Fact]
	public void ObjectType_Unknown()
	{
		Assert.Equal(0, (int)ObjectType.Unknown);
	}

	[Fact]
	public void ObjectType_Enumerable()
	{
		Assert.True(Enum.IsDefined(typeof(ObjectType), ObjectType.Enumerable));
	}

	[Fact]
	public void ObjectType_Exception()
	{
		Assert.True(Enum.IsDefined(typeof(ObjectType), ObjectType.Exception));
	}

	[Fact]
	public void ObjectType_Delegate()
	{
		Assert.True(Enum.IsDefined(typeof(ObjectType), ObjectType.Delegate));
	}
}
