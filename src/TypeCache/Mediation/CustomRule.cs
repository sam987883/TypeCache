// Copyright (c) 2021 Samuel Abraham

using System.Runtime.CompilerServices;
using TypeCache.Extensions;

namespace TypeCache.Mediation;

internal sealed class CustomRule<REQUEST>(Func<REQUEST, CancellationToken, Task> executeAsync)
	: IRule<REQUEST>
	where REQUEST : IRequest
{
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public Task ExecuteAsync(REQUEST request, CancellationToken token = default)
		=> executeAsync(request, token);
}

internal sealed class CustomRule<REQUEST, RESPONSE>(Func<REQUEST, CancellationToken, Task<RESPONSE>> mapAsync)
	: IRule<REQUEST, RESPONSE>
	where REQUEST : IRequest<RESPONSE>
{
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public Task<RESPONSE> MapAsync(REQUEST request, CancellationToken token = default)
		=> mapAsync(request, token);
}
