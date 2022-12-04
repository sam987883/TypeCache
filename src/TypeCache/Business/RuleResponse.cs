// Copyright (c) 2021 Samuel Abraham

namespace TypeCache.Business;

public readonly struct RuleResponse
{
	public required Exception? Error { get; init; }

	public required string[] ValidationMessages { get; init; }

	public required RuleResponseStatus Status { get; init; }
}

public readonly struct RuleResponse<RESPONSE>
{
	public required Exception? Error { get; init; }

	public required string[] ValidationMessages { get; init; }

	public required RESPONSE? Response { get; init; }

	public required RuleResponseStatus Status { get; init; }
}
