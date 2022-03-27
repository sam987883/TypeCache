// Copyright (c) 2021 Samuel Abraham

using System;
using System.Runtime.CompilerServices;
using GraphQL.Language.AST;
using GraphQL.Types;
using Microsoft.Extensions.DependencyInjection;
using TypeCache.Extensions;
using TypeCache.Security;
using static TypeCache.Default;

namespace TypeCache.GraphQL.Types;

/// <summary>
/// Requires call to either one of:
/// <list type="bullet">
/// <item><see cref="ServiceCollectionExtensions.RegisterHashMaker(IServiceCollection, byte[], byte[])"/></item>
/// <item><see cref="ServiceCollectionExtensions.RegisterHashMaker(IServiceCollection, decimal, decimal)"/></item>
/// </list>
/// </summary>
public class GraphQLHashIdType : ScalarGraphType
{
	private readonly IHashMaker _HashMaker;

	/// <exception cref="ArgumentNullException"/>
	public GraphQLHashIdType(IHashMaker hashMaker)
	{
		hashMaker.AssertNotNull();

		this._HashMaker = hashMaker;
		this.Name = "HashID";
		this.Description = "A hashed ID.";
	}

	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public override object? ParseLiteral(IValue value)
		=> this.ParseValue(value.Value);

	public override object? ParseValue(object? value)
		=> value switch
		{
			int id => id,
			long id => id,
			string hashId => this._HashMaker.Decrypt(hashId),
			_ => value
		};

	public override object? Serialize(object? value)
		=> value switch
		{
			int id => this._HashMaker.Encrypt(id),
			long id => this._HashMaker.Encrypt(id),
			string hashId => this._HashMaker.Decrypt(hashId),
			_ => value
		};
}
