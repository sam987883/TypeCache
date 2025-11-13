// Copyright (c) 2021 Samuel Abraham

namespace TypeCache.Mediation;

internal sealed class CustomAfterRule<REQUEST>(Func<REQUEST, CancellationToken, Task> execute)
	: IAfterRule<REQUEST>
	where REQUEST : notnull
{
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public Task Handle(REQUEST request, CancellationToken token = default)
		=> execute(request, token);
}
