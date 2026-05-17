// Copyright (c) 2021 Samuel Abraham

using System.Data;
using NSubstitute;
using TypeCache.Data;
using TypeCache.Data.Extensions;
using Xunit;

namespace TypeCache.Tests.Data.Extensions;

public class DbCommandExtensionsTests
{
	[Fact]
	public void AddInputParameter()
	{
		var command = Substitute.For<IDbCommand>();
		var parameters = Substitute.For<IDataParameterCollection>();
		command.Parameters.Returns(parameters);
		command.CreateParameter().Returns(_ => Substitute.For<IDbDataParameter>());

		command.AddInputParameter("@test", "value");

		parameters.Received(1).Add(Arg.Any<IDbDataParameter>());
	}

	[Fact]
	public void AddInputParameter_WithNullValue()
	{
		var command = Substitute.For<IDbCommand>();
		var parameters = Substitute.For<IDataParameterCollection>();
		command.Parameters.Returns(parameters);
		command.CreateParameter().Returns(_ => Substitute.For<IDbDataParameter>());

		command.AddInputParameter("@test", null);

		parameters.Received(1).Add(Arg.Any<IDbDataParameter>());
	}

	[Fact]
	public void AddOutputParameter()
	{
		var command = Substitute.For<IDbCommand>();
		var parameters = Substitute.For<IDataParameterCollection>();
		command.Parameters.Returns(parameters);
		command.CreateParameter().Returns(_ => Substitute.For<IDbDataParameter>());

		command.AddOutputParameter("@output", DbType.String);

		parameters.Received(1).Add(Arg.Any<IDbDataParameter>());
	}

	[Fact]
	public void AddInputOutputParameter()
	{
		var command = Substitute.For<IDbCommand>();
		var parameters = Substitute.For<IDataParameterCollection>();
		command.Parameters.Returns(parameters);
		command.CreateParameter().Returns(_ => Substitute.For<IDbDataParameter>());

		command.AddInputOutputParameter("@inout", "value", DbType.String);

		parameters.Received(1).Add(Arg.Any<IDbDataParameter>());
	}
}
