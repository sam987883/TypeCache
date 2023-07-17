// Copyright (c) 2021 Samuel Abraham

using System.Runtime.CompilerServices;
using TypeCache.Extensions;

namespace TypeCache.Mediation;

internal sealed class CustomAfterRule<REQUEST> : IAfterRule<REQUEST>
	where REQUEST : IRequest
{
	private readonly Func<REQUEST, CancellationToken, Task> _ExecuteAsync;

	public CustomAfterRule(Func<REQUEST, CancellationToken, Task> executeAsync)
	{
		this._ExecuteAsync = executeAsync;
	}

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public Task HandleAsync(REQUEST request, CancellationToken token = default)
		=> this._ExecuteAsync(request, token);
}

internal sealed class CustomAfterRule<REQUEST, RESPONSE> : IAfterRule<REQUEST, RESPONSE>
	where REQUEST : IRequest<RESPONSE>
{
	private readonly Func<REQUEST, RESPONSE, CancellationToken, Task> _ExecuteAsync;

	public CustomAfterRule(Func<REQUEST, RESPONSE, CancellationToken, Task> executeAsync)
	{
		this._ExecuteAsync = executeAsync;
	}

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public Task HandleAsync(REQUEST request, RESPONSE response, CancellationToken token = default)
		=> this._ExecuteAsync(request, response, token);
}
