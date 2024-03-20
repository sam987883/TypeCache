// Copyright (c) 2021 Samuel Abraham

namespace TypeCache.Mediation;

internal sealed class CustomAfterRule<REQUEST>(Func<REQUEST, CancellationToken, Task> execute)
	: IAfterRule<REQUEST>
	where REQUEST : IRequest
{
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public Task Handle(REQUEST request, CancellationToken token = default)
		=> execute(request, token);
}

internal sealed class CustomAfterRule<REQUEST, RESPONSE>(Func<REQUEST, RESPONSE, CancellationToken, Task> execute)
	: IAfterRule<REQUEST, RESPONSE>
	where REQUEST : IRequest<RESPONSE>
{
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public Task Handle(REQUEST request, RESPONSE response, CancellationToken token = default)
		=> execute(request, response, token);
}
