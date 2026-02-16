// Copyright (c) 2021 Samuel Abraham

namespace TypeCache.Mediation;

public sealed class CustomRule<REQUEST, RESPONSE>(Func<REQUEST, CancellationToken, RESPONSE> send) : IRule<REQUEST, RESPONSE>
	where REQUEST : notnull
{

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public RESPONSE Send(REQUEST request, CancellationToken token)
		=> send(request, token);
}
