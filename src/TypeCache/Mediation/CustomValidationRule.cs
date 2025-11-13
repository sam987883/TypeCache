// Copyright (c) 2021 Samuel Abraham

namespace TypeCache.Mediation;

internal sealed class CustomValidationRule<REQUEST>(Action<REQUEST> validate)
	: IValidationRule<REQUEST>
	where REQUEST : notnull
{
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public void Validate(REQUEST request)
		=> validate(request);
}
