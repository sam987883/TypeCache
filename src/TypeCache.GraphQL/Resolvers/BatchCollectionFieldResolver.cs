// Copyright (c) 2021 Samuel Abraham

using GraphQL;
using TypeCache.Extensions;
using TypeCache.GraphQL.Extensions;
using TypeCache.Reflection;
using IResolveFieldContext = global::GraphQL.IResolveFieldContext;

namespace TypeCache.GraphQL.Resolvers;

public sealed class BatchCollectionFieldResolver<SOURCE, ITEM>(StaticMethodEntity method, Func<SOURCE, ITEM[], ITEM[]> getResults) : FieldResolver
	where SOURCE : notnull
	where ITEM : notnull
{
	private readonly Lock _Lock = new();
	private bool _HasResults = false;
	private ValueTask<ITEM[]>? _Results = null;

	/// <exception cref="ArgumentNullException"/>
	/// <exception cref="ArgumentOutOfRangeException"/>
	protected override async ValueTask<object?> ResolveAsync(IResolveFieldContext context)
	{
		context.Source.ThrowIfNull();
		getResults.ThrowIfNull();
		method.ThrowIfNull();
		method.Return.ParameterType.IsAny<IEnumerable<ITEM>, Task<IEnumerable<ITEM>>, ValueTask<IEnumerable<ITEM>>, ITEM[], Task<ITEM[]>, ValueTask<ITEM[]>>().ThrowIfFalse();

		if (!this._HasResults)
		{
			using (this._Lock.EnterScope())
			{
				if (!this._HasResults)
				{
					this._Results = this.GetData(context);
					this._HasResults = true;
				}
			}
		}

		return getResults((SOURCE)context.Source, this._Results.HasValue ? await this._Results.Value : []);
	}

	private ValueTask<ITEM[]> GetData(IResolveFieldContext context)
		=> method.Invoke(context.GetArguments(method.Parameters).ToArray()) switch
		{
			ITEM[] batch => ValueTask.FromResult(batch),
			IEnumerable<ITEM> batch => ValueTask.FromResult(batch.AsArray()),
			Task<ITEM[]> task => new ValueTask<ITEM[]>(task),
			Task<IEnumerable<ITEM>> task => ValueTask.FromResult(task.Result.AsArray()),
			ValueTask<ITEM[]> valueTask => valueTask,
			ValueTask<IEnumerable<ITEM>> valueTask => ValueTask.FromResult(valueTask.Result.AsArray()),
			_ => ValueTask.FromResult<ITEM[]>([])
		};
}
