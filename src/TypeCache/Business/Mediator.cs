// Copyright (c) 2021 Samuel Abraham

using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;
using TypeCache.Extensions;

namespace TypeCache.Business
{
	public class Mediator
    {
        private readonly IServiceProvider _ServiceProvider;

        public Mediator(IServiceProvider serviceProvider)
        {
            this._ServiceProvider = serviceProvider;
        }

        public async Task RunAsync<T>(T request, CancellationToken cancellationToken = default)
        {
            var processHandler = this._ServiceProvider.GetService<IProcessHandler<T>>();
            if (processHandler != null)
                await processHandler.HandleAsync(request, cancellationToken);
            else
            {
                var validationRules = this._ServiceProvider.GetServices<IValidationRule<T>>();
                if (validationRules.Any())
                {
                    var results = await Task.WhenAll(validationRules.To(async validationRule => await validationRule.ApplyAsync(request, cancellationToken)));
                    if (results.All(true))
                    {
                        var processes = this._ServiceProvider.GetServices<IProcess<T>>();
                        await Task.WhenAll(processes.To(async process => await process.RunAsync(request, cancellationToken)));
                    }
                }
            }
        }

        public async Task RunAsync<M, T>(M metadata, T request, CancellationToken cancellationToken = default)
        {
            var processHandler = this._ServiceProvider.GetService<IProcessHandler<M, T>>();
            if (processHandler != null)
                await processHandler.HandleAsync(metadata, request, cancellationToken);
            else
            {
                var validationRules = this._ServiceProvider.GetServices<IValidationRule<T>>();
                if (validationRules.Any())
                {
                    var results = await Task.WhenAll(validationRules.To(async validationRule => await validationRule.ApplyAsync(request, cancellationToken)));
                    if (results.All(true))
                    {
                        var processes = this._ServiceProvider.GetServices<IProcess<M, T>>();
                        await Task.WhenAll(processes.To(async process => await process.RunAsync(metadata, request, cancellationToken)));
                    }
                }
            }
        }

        public async Task<R[]> SendAsync<T, R>(T request, CancellationToken cancellationToken = default)
        {
            var ruleHandler = this._ServiceProvider.GetService<IRuleHandler<T, R>>();
            if (ruleHandler != null)
                return await ruleHandler.HandleAsync(request, cancellationToken);
            else
            {
                var validationRules = this._ServiceProvider.GetServices<IValidationRule<T>>();
                if (validationRules.Any())
                {
                    var results = await Task.WhenAll(validationRules.To(async validationRule => await validationRule.ApplyAsync(request, cancellationToken)));
                    if (results.All(true))
                    {
                        var rules = this._ServiceProvider.GetServices<IRule<T, R>>();
                        return await Task.WhenAll(rules.To(async rule => await rule.ApplyAsync(request, cancellationToken)));
                    }
                }
            }
            return Array.Empty<R>();
        }

        public async Task<R[]> SendAsync<M, T, R>(M metadata, T request, CancellationToken cancellationToken = default)
        {
            var ruleHandler = this._ServiceProvider.GetService<IRuleHandler<M, T, R>>();
            if (ruleHandler != null)
                return await ruleHandler.HandleAsync(metadata, request, cancellationToken);
            else
            {
                var validationRules = this._ServiceProvider.GetServices<IValidationRule<T>>();
                if (validationRules.Any())
                {
                    var results = await Task.WhenAll(validationRules.To(async validationRule => await validationRule.ApplyAsync(request, cancellationToken)));
                    if (results.All(true))
                    {
                        var rules = this._ServiceProvider.GetServices<IRule<M, T, R>>();
                        return await Task.WhenAll(rules.To(async rule => await rule.ApplyAsync(metadata, request, cancellationToken)));
                    }
                }
            }
            return Array.Empty<R>();
        }
    }
}
