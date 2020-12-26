// Copyright (c) 2021 Samuel Abraham

using GraphQL.Language.AST;
using GraphQL.Types;
using Microsoft.Extensions.DependencyInjection;
using System.Runtime.CompilerServices;
using TypeCache.Extensions;
using TypeCache.Security;

namespace TypeCache.GraphQL.Types
{
	public class GraphHashIdType : ScalarGraphType
	{
		private readonly IHashMaker _HashMaker;

		public GraphHashIdType(IHashMaker hashMaker)
		{
			this.Name = "HashID";
			hashMaker.AssertNotNull($"{nameof(GraphHashIdType)}: Must first call: [{nameof(IServiceCollection)}.{nameof(IServiceCollectionExtensions.RegisterSecurity)}].");

			this._HashMaker = hashMaker;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override object ParseLiteral(IValue value)
			=> this.ParseValue(value.Value);

		public override object ParseValue(object value)
			=> value switch
			{
				int id => id,
				long id => id,
				string hashId => this._HashMaker.Decrypt(hashId),
				_ => value
			};

		public override object Serialize(object value)
			=> value switch
			{
				int id => this._HashMaker.Encrypt(id),
				long id => this._HashMaker.Encrypt(id),
				string hashId => this._HashMaker.Decrypt(hashId),
				_ => value
			};
	}
}
