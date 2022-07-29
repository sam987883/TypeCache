// Copyright (c) 2021 Samuel Abraham

using System;
using System.Threading;
using System.Threading.Tasks;

namespace TypeCache.Business;

public interface IRuleIntermediary<in REQUEST, RESPONSE>
{
	ValueTask<RuleResponse<RESPONSE>> GetAsync(REQUEST request, CancellationToken token = default);
}
