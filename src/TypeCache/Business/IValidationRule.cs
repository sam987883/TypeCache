// Copyright (c) 2021 Samuel Abraham

using System.Threading;
using System.Threading.Tasks;

namespace TypeCache.Business
{
	public interface IValidationRule<in I>
	{
		ValueTask ValidateAsync(I request, CancellationToken cancellationToken = default);
	}
}
