// Copyright (c) 2021 Samuel Abraham

using System;
using System.Diagnostics;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TypeCache.Business;
using static TypeCache.Default;

namespace TypeCache.Web.Endpoints;

[ApiController]
public abstract class WebApiEndpoint : ControllerBase
{
	public WebApiEndpoint(IMediator mediator)
	{
		this.Mediator = mediator;
	}

	protected IMediator Mediator { get; }

	public async ValueTask<ObjectResult> ApplyRuleAsync<REQUEST, RESPONSE>(REQUEST request)
		=> await this.Mediator.ApplyRuleAsync<REQUEST, RESPONSE, ObjectResult>(request
			, response => this.Ok(response)
			, error => error is ValidationException validationError
				? this.BadRequest(validationError.ValidationMessages)
				: this.StatusCode((int)HttpStatusCode.InternalServerError, error.Message)
			, this.HttpContext.RequestAborted);
}
