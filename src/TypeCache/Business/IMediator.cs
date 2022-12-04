// Copyright (c) 2021 Samuel Abraham

namespace TypeCache.Business;

public interface IMediator
{
	ValueTask ApplyRuleAsync<REQUEST>(REQUEST request, CancellationToken token = default);

	ValueTask<RESPONSE> ApplyRuleAsync<REQUEST, RESPONSE>(REQUEST request, CancellationToken token = default);

	ValueTask ApplyRuleAsync<REQUEST, RESPONSE>(REQUEST request, Action<RESPONSE> onSuccess, CancellationToken token = default);

	ValueTask<RuleResponseStatus> ApplyRuleAsync<REQUEST, RESPONSE>(REQUEST request, Action<RESPONSE> onSuccess, Action<Exception> onError, CancellationToken token = default);

	ValueTask<RESULT> ApplyRuleAsync<REQUEST, RESPONSE, RESULT>(REQUEST request, Func<RESPONSE, RESULT> onSuccess, CancellationToken token = default);

	ValueTask<RESULT> ApplyRuleAsync<REQUEST, RESPONSE, RESULT>(REQUEST request, Func<RESPONSE, RESULT> onSuccess, Func<Exception, RESULT> onError, CancellationToken token = default);

	ValueTask RunProcessAsync<REQUEST>(REQUEST request, Action onComplete, CancellationToken token = default);
}
