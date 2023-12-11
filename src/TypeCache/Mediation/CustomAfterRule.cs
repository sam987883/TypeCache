// Copyright (c) 2021 Samuel Abraham

using System.Runtime.CompilerServices;
using TypeCache.Extensions;

namespace TypeCache.Mediation;

internal sealed class CustomAfterRule<REQUEST>(Func<REQUEST, CancellationToken, Task> executeAsync)
	: IAfterRule<REQUEST>
	where REQUEST : IRequest
{
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public Task HandleAsync(REQUEST request, CancellationToken token = default)
		=> executeAsync(request, token);
}

internal sealed class CustomAfterRule<REQUEST, RESPONSE>(Func<REQUEST, RESPONSE, CancellationToken, Task> executeAsync)
	: IAfterRule<REQUEST, RESPONSE>
	where REQUEST : IRequest<RESPONSE>
{
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public Task HandleAsync(REQUEST request, RESPONSE response, CancellationToken token = default)
		=> executeAsync(request, response, token);
}
