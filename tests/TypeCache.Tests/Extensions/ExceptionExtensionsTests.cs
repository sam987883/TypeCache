// Copyright (c) 2021 Samuel Abraham

using TypeCache.Extensions;
using Xunit;

namespace TypeCache.Tests.Extensions;

public class ExceptionExtensionsTests
{
	[Fact]
	public void InnerMostException_SingleException()
	{
		var exception = new Exception("Test");
		var innerMost = exception.InnerMostException;

		Assert.Same(exception, innerMost);
	}

	[Fact]
	public void InnerMostException_NestedExceptions()
	{
		var innerException = new Exception("Inner");
		var middleException = new Exception("Middle", innerException);
		var outerException = new Exception("Outer", middleException);

		var innerMost = outerException.InnerMostException;

		Assert.Same(innerException, innerMost);
	}

	[Fact]
	public void InnerMostException_DeepNesting()
	{
		Exception current = new Exception("Level5");
		current = new Exception("Level4", current);
		current = new Exception("Level3", current);
		current = new Exception("Level2", current);
		var outerException = new Exception("Level1", current);

		var innerMost = outerException.InnerMostException;

		Assert.Equal("Level5", innerMost.Message);
	}
}
