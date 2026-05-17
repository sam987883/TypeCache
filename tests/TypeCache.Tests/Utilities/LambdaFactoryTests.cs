// Copyright (c) 2021 Samuel Abraham

using System.Linq.Expressions;
using TypeCache.Utilities;
using Xunit;

namespace TypeCache.Tests.Utilities;

public class LambdaFactoryTests
{
	[Fact]
	public void CreateAction_SingleParameter()
	{
		var expression = LambdaFactory.CreateAction<int>(p => Expression.Empty());

		Assert.NotNull(expression);
		var compiled = expression.Compile();
		Assert.NotNull(compiled);
	}

	[Fact]
	public void CreateAction_TwoParameters()
	{
		var expression = LambdaFactory.CreateAction<int, string>((p1, p2) => Expression.Empty());

		Assert.NotNull(expression);
		var compiled = expression.Compile();
		Assert.NotNull(compiled);
	}

	[Fact]
	public void CreateAction_ThreeParameters()
	{
		var expression = LambdaFactory.CreateAction<int, string, bool>((p1, p2, p3) => Expression.Empty());

		Assert.NotNull(expression);
		var compiled = expression.Compile();
		Assert.NotNull(compiled);
	}

	[Fact]
	public void CreateComparison()
	{
		var expression = LambdaFactory.CreateComparison<int>((left, right) =>
			Expression.Call(left, typeof(int).GetMethod("CompareTo", [typeof(int)])!, right));

		Assert.NotNull(expression);
		var compiled = expression.Compile();
		Assert.NotNull(compiled);
	}
}
