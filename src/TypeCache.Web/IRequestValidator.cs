// Copyright (c) 2021 Samuel Abraham

using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace TypeCache.Web
{
	public interface IRequestValidator<T>
	{
		Task<bool> Validate(T request, HttpContext httpContext);
	}
}
