// Copyright (c) 2021 Samuel Abraham

namespace TypeCache.Mediation;

internal sealed class CustomRule<REQUEST>(Func<REQUEST, CancellationToken, Task> execute)
	: IRule<REQUEST>
	where REQUEST : IRequest
{
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public Task Execute(REQUEST request, CancellationToken token = default)
		=> execute(request, token);
}

internal sealed class CustomRule<REQUEST, RESPONSE>(Func<REQUEST, CancellationToken, Task<RESPONSE>> map)
	: IRule<REQUEST, RESPONSE>
	where REQUEST : IRequest<RESPONSE>
{
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public Task<RESPONSE> Map(REQUEST request, CancellationToken token = default)
		=> map(request, token);
}
