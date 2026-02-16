// Copyright (c) 2021 Samuel Abraham

namespace TypeCache.Mediation;

internal sealed class CustomValidation<REQUEST>(Action<REQUEST> validate)
	: IValidation<REQUEST>
	where REQUEST : notnull
{
	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public void Validate(REQUEST request)
		=> validate(request);
}
