// Copyright (c) 2021 Samuel Abraham

using System.Runtime.CompilerServices;
using TypeCache.Extensions;

namespace TypeCache.Mediation;

internal sealed class CustomRule<REQUEST> : IRule<REQUEST>
	where REQUEST : IRequest
{
	private readonly Func<REQUEST, CancellationToken, Task> _ExecuteAsync;

	public CustomRule(Func<REQUEST, CancellationToken, Task> executeAsync)
	{
		this._ExecuteAsync = executeAsync;
	}

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public Task ExecuteAsync(REQUEST request, CancellationToken token = default)
		=> this._ExecuteAsync(request, token);
}

internal sealed class CustomRule<REQUEST, RESPONSE> : IRule<REQUEST, RESPONSE>
	where REQUEST : IRequest<RESPONSE>
{
	private readonly Func<REQUEST, CancellationToken, Task<RESPONSE>> _MapAsync;

	public CustomRule(Func<REQUEST, CancellationToken, Task<RESPONSE>> mapAsync)
	{
		this._MapAsync = mapAsync;
	}

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public Task<RESPONSE> MapAsync(REQUEST request, CancellationToken token = default)
		=> this._MapAsync(request, token);
}
