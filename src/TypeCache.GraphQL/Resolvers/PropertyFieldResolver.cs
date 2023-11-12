// Copyright (c) 2021 Samuel Abraham

using System;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using GraphQL;
using TypeCache.Extensions;
using static System.FormattableString;
using static System.Globalization.CultureInfo;
using static System.Text.RegularExpressions.RegexOptions;

namespace TypeCache.GraphQL.Resolvers;

public sealed class PropertyFieldResolver<T> : FieldResolver
{
	private readonly PropertyInfo _PropertyInfo;

	public PropertyFieldResolver(PropertyInfo propertyInfo)
	{
		this._PropertyInfo = propertyInfo;
	}

	protected override async ValueTask<object?> ResolveAsync(IResolveFieldContext context)
	{
		var source = context.Source switch
		{
			null => null,
			Task<T> task => await task,
			ValueTask<T> task => await task,
			_ => context.Source
		};

		var value = this._PropertyInfo.GetPropertyValue(source!);
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
				var match = Regex.Match(text, pattern, Compiled | Singleline);
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

		if (format.IsNotBlank())
			return string.Format(InvariantCulture, Invariant($"{{0:{format}}}"), value);

		return value;
	}
}
