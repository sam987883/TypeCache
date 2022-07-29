// Copyright (c) 2021 Samuel Abraham

using System;
using TypeCache.Collections;

namespace TypeCache.Business;

public readonly struct RuleResponse<RESPONSE>
{
	public RuleResponse(RESPONSE response)
	{
		this.Error = null;
		this.FailedValidation = Array<string>.Empty;
		this.Response = response;
		this.Status = RuleResponseStatus.Success;
	}

	public RuleResponse(params string[] failedValidation)
	{
		this.Error = null;
		this.FailedValidation = failedValidation;
		this.Response = default;
		this.Status = RuleResponseStatus.FailValidation;
	}

	public RuleResponse(Exception error)
	{
		this.Error = error;
		this.FailedValidation = Array<string>.Empty;
		this.Response = default;
		this.Status = RuleResponseStatus.Error;
	}

	public Exception? Error { get; init; }

	public string[] FailedValidation { get; init; }

	public RESPONSE? Response { get; init; }

	public RuleResponseStatus Status { get; init; }
}
