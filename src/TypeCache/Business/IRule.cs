// Copyright (c) 2021 Samuel Abraham

using System.Threading;
using System.Threading.Tasks;

namespace TypeCache.Business;

public interface IRule<in REQUEST, RESPONSE>
{
	ValueTask<RESPONSE> ApplyAsync(REQUEST request, CancellationToken token = default);
}
