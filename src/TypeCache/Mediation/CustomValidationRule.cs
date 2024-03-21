// Copyright (c) 2021 Samuel Abraham

namespace TypeCache.Mediation;

internal sealed class CustomValidationRule<REQUEST> : IValidationRule<REQUEST>
	where REQUEST : IRequest
{
	private readonly Func<REQUEST, CancellationToken, Task> _ValidateAsync;

	public CustomValidationRule(Func<REQUEST, CancellationToken, Task> validateAsync)
	{
		this._ValidateAsync = validateAsync;
	}

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public Task Validate(REQUEST request, CancellationToken token = default)
		=> this._ValidateAsync(request, token);
}
