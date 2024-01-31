// Copyright (c) 2021 Samuel Abraham

namespace TypeCache.Mediation;

internal sealed class CustomValidationRule<REQUEST>(Func<REQUEST, CancellationToken, Task> validate)
	: IValidationRule<REQUEST>
	where REQUEST : IRequest
{
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public Task Validate(REQUEST request, CancellationToken token = default)
		=> validate(request, token);
}
