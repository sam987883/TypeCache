// Copyright (c) 2021 Samuel Abraham

using System.Reflection;
using System.Text.RegularExpressions;
using GraphQL;
using TypeCache.Extensions;
using static System.Globalization.CultureInfo;

namespace TypeCache.GraphQL.Resolvers;

public sealed class PropertyFieldResolver(PropertyInfo propertyInfo) : FieldResolver
{
	protected override async Task<object?> ResolveAsync(IResolveFieldContext context)
	{
		propertyInfo.ThrowIfNull();
		context.Source.ThrowIfNull();

		var source = context.Source switch
		{
			Task<object> task => await task,
			ValueTask<object> task => await task,
			Task task => await task.Cast<object>(),
			_ when context.Source!.GetType().Is(typeof(ValueTask<>).MakeGenericType(propertyInfo.DeclaringType!))
				=> await ((Task)typeof(ValueTask<>).MakeGenericType(propertyInfo.DeclaringType!).InvokeMethodFunc(nameof(ValueTask.AsTask), [], context.Source, [])!).Cast<object>(),
			_ => context.Source
		};

		var value = propertyInfo.GetValueEx(source!);
		if (value is null)
			return value ?? context.GetArgument<object>("null");

		await Task.CompletedTask;

		var format = context.GetArgument<string>("format");

		//value = value switch
		//{
		//	true => context.GetArgument<string>("true"),
		//	false => context.GetArgument<string>("false"),
		//	_ => value
		//};

		if (value is DateTime dateTime)
		{
			var timeZone = context.GetArgument<string>("timeZone");
			if (timeZone.IsNotBlank())
			{
				var currentTimeZone = TimeZoneInfo.Utc.Id;
				if (timeZone.Contains(','))
					(currentTimeZone!, timeZone, _) = timeZone.Split(',');

				dateTime = dateTime.ChangeTimeZone(currentTimeZone, timeZone!);
			}

			return format.IsNotBlank() ? dateTime.ToString(format, InvariantCulture) : dateTime;
		}

		if (value is DateTimeOffset dateTimeOffset)
		{
			var timeZone = context.GetArgument<string>("timeZone");
			if (timeZone.IsNotBlank())
				dateTimeOffset = dateTimeOffset.ToTimeZone(timeZone);

			return format.IsNotBlank() ? dateTimeOffset.ToString(format, InvariantCulture) : dateTimeOffset;
		}

		if (value is string text)
		{
			var trim = context.GetArgument<string>("trim");
			if (trim is not null)
				text = text.Trim(trim.ToCharArray());

			var trimStart = context.GetArgument<string>("trimStart");
			if (trimStart is not null)
				text = text.TrimStart(trimStart.ToCharArray());

			var trimEnd = context.GetArgument<string>("trimEnd");
			if (trimEnd is not null)
				text = text.TrimEnd(trimEnd.ToCharArray());

			var pattern = context.GetArgument<string>("match");
			if (pattern.IsNotBlank())
			{
				var match = text.ToRegex(RegexOptions.Compiled | RegexOptions.Singleline).Match(text);
				if (match.Success)
					text = match.Value;
				else
					return null;
			}

			var length = context.GetArgument<int?>("length");
			if (text.Length > length)
				text = text.Left(length.Value);

			return context.GetArgument<StringCase?>("case") switch
			{
				StringCase.Lower => text.ToLower(),
				StringCase.LowerInvariant => text.ToLowerInvariant(),
				StringCase.Upper => text.ToUpper(),
				StringCase.UpperInvariant => text.ToUpperInvariant(),
				_ => text
			};
		}

		return value switch
		{
			IFormattable formattable when format.IsNotBlank() => formattable.ToString(format, InvariantCulture),
			_ => value
		};
	}
}
