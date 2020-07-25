// Copyright (c) 2020 Samuel Abraham

using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace sam987883.Web
{
	public interface IRequestValidator<T>
	{
		Task<bool> Validate(T request, HttpContext httpContext);
	}
}
