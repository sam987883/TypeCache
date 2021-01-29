// Copyright (c) 2021 Samuel Abraham

using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using TypeCache.Collections.Extensions;
using TypeCache.Extensions;

namespace TypeCache.Business.Extensions
{
	public static class HandlerExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async ValueTask ApplyValidationRules<T>(this IProcessHandler<T> _, IServiceProvider provider, T request, CancellationToken cancellationToken)
			=> await ApplyValidationRulesForProcess(provider, request, cancellationToken);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async ValueTask ApplyValidationRules<M, T>(this IProcessHandler<M, T> _, IServiceProvider provider, T request, CancellationToken cancellationToken)
			=> await ApplyValidationRulesForProcess(provider, request, cancellationToken);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async ValueTask ApplyValidationRules<M, T>(this IProcessHandler<M, T> _, IServiceProvider provider, M metadata, T request, CancellationToken cancellationToken)
			=> await ApplyValidationRulesForProcess(provider, metadata, request, cancellationToken);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async ValueTask<ValidationResponse[]> ApplyValidationRules<T, R>(this IRuleHandler<T, R> _, IServiceProvider provider, T request, CancellationToken cancellationToken)
			=> await ApplyValidationRulesForRule(provider, request, cancellationToken);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async ValueTask<ValidationResponse[]> ApplyValidationRules<M, T, R>(this IRuleHandler<M, T, R> _, IServiceProvider provider, T request, CancellationToken cancellationToken)
			=> await ApplyValidationRulesForRule(provider, request, cancellationToken);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async ValueTask<ValidationResponse[]> ApplyValidationRules<M, T, R>(this IRuleHandler<M, T, R> _, IServiceProvider provider, M metadata, T request, CancellationToken cancellationToken)
			=> await ApplyValidationRulesForRule(provider, metadata, request, cancellationToken);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async ValueTask<ValidationResponse[]> ApplyValidationRules<T, R>(this IRulesHandler<T, R> _, IServiceProvider provider, T request, CancellationToken cancellationToken)
			=> await ApplyValidationRulesForRule(provider, request, cancellationToken);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async ValueTask<ValidationResponse[]> ApplyValidationRules<M, T, R>(this IRulesHandler<M, T, R> _, IServiceProvider provider, T request, CancellationToken cancellationToken)
			=> await ApplyValidationRulesForRule(provider, request, cancellationToken);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static async ValueTask<ValidationResponse[]> ApplyValidationRules<M, T, R>(this IRulesHandler<M, T, R> _, IServiceProvider provider, M metadata, T request, CancellationToken cancellationToken)
			=> await ApplyValidationRulesForRule(provider, metadata, request, cancellationToken);

		private static async ValueTask ApplyValidationRulesForProcess<T>(IServiceProvider provider, T request, CancellationToken cancellationToken)
		{
			var validationRules = provider.GetServices<IValidationRule<T>>();
			if (validationRules.Any())
			{
				var results = await validationRules.To(async validationRule => await validationRule.ApplyAsync(request, cancellationToken)).AllAsync();
				var errors = results.If(_ => _.IsError);
				if (errors.Any())
					throw new ValidationRuleException(errors.ToMany(_ => _.Messages).ToArray());
			}
		}

		private static async ValueTask ApplyValidationRulesForProcess<M, T>(IServiceProvider provider, M metadata, T request, CancellationToken cancellationToken)
		{
			var validationRules = provider.GetServices<IValidationRule<M, T>>();
			if (validationRules.Any())
			{
				var results = await validationRules.To(async validationRule => await validationRule.ApplyAsync(metadata, request, cancellationToken)).AllAsync();
				var errors = results.If(_ => _.IsError);
				if (errors.Any())
					throw new ValidationRuleException(errors.ToMany(_ => _.Messages).ToArray());
			}
		}

		private static async ValueTask<ValidationResponse[]> ApplyValidationRulesForRule<T>(IServiceProvider provider, T request, CancellationToken cancellationToken)
		{
			var validationRules = provider.GetServices<IValidationRule<T>>();
			return validationRules.Any()
				? await validationRules.To(async validationRule => await validationRule.ApplyAsync(request, cancellationToken)).AllAsync()
				: Array.Empty<ValidationResponse>();
		}

		private static async ValueTask<ValidationResponse[]> ApplyValidationRulesForRule<M, T>(IServiceProvider provider, M metadata, T request, CancellationToken cancellationToken)
		{
			var validationRules = provider.GetServices<IValidationRule<M, T>>();
			return validationRules.Any()
				? await validationRules.To(async validationRule => await validationRule.ApplyAsync(metadata, request, cancellationToken)).AllAsync()
				: Array.Empty<ValidationResponse>();
		}
	}
}
