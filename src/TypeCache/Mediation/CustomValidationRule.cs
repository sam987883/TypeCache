// Copyright (c) 2021 Samuel Abraham

namespace TypeCache.Mediation;

internal sealed class CustomValidationRule<REQUEST>(Func<REQUEST, CancellationToken, Task> validateAsync)
	: IValidationRule<REQUEST>
	where REQUEST : IRequest
{
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public Task ValidateAsync(REQUEST request, CancellationToken token = default)
		=> validateAsync(request, token);
}
