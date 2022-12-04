// Copyright (c) 2021 Samuel Abraham

namespace TypeCache.Business;

public interface IRuleIntermediary<in REQUEST>
{
	ValueTask<RuleResponse> GetAsync(REQUEST request, CancellationToken token = default);
}

public interface IRuleIntermediary<in REQUEST, RESPONSE>
{
	ValueTask<RuleResponse<RESPONSE>> GetAsync(REQUEST request, CancellationToken token = default);
}
