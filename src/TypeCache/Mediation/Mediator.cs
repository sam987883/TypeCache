// Copyright (c) 2021 Samuel Abraham

using Microsoft.Extensions.DependencyInjection;
using TypeCache.Extensions;

namespace TypeCache.Mediation;

internal sealed class Mediator(IServiceProvider serviceProvider)
	: IMediator
{
	public RESPONSE Send<REQUEST, RESPONSE>(Request<REQUEST, RESPONSE> request, CancellationToken token = default)
		where REQUEST : notnull
	{
		this.Validate(request.Value, request.ServiceKey);

		IRule<REQUEST, RESPONSE> rule;
		using (var scope = serviceProvider.CreateScope())
		{
			rule = scope.ServiceProvider.GetRequiredKeyedService<IRule<REQUEST, RESPONSE>>(request.ServiceKey);
		}

		return rule.Send(request.Value, token);
	}

	public void Validate<REQUEST>(REQUEST request, object? key = null)
		where REQUEST : notnull
	{
		request.ThrowIfNull();

		IEnumerable<IValidation<REQUEST>> validations;
		using (var scope = serviceProvider.CreateScope())
		{
			validations = scope.ServiceProvider.GetServices<IValidation<REQUEST>>();
			if (key is not null)
				validations = validations.Concat(scope.ServiceProvider.GetKeyedServices<IValidation<REQUEST>>(key));
		}

		if (!validations.Any())
			return;

		Task.WaitAll(validations.Select(_ => Task.Run(() => _.Validate(request))));
	}
}
