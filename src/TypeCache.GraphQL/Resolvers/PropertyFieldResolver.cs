// Copyright (c) 2021 Samuel Abraham

using System;
using GraphQL;
using GraphQL.Resolvers;
using TypeCache.Collections.Extensions;
using TypeCache.Extensions;
using TypeCache.Reflection;
using static System.FormattableString;
using static System.Globalization.CultureInfo;

namespace TypeCache.GraphQL.Resolvers;

public class PropertyFieldResolver : IFieldResolver<object>
{
	private readonly PropertyMember _PropertyMember;

	public PropertyFieldResolver(PropertyMember propertyMember)
	{
		this._PropertyMember = propertyMember;
	}

	public object? Resolve(IResolveFieldContext context)
	{
		var value = this._PropertyMember.GetValue(context.Source);
		if (value is null)
			return value ?? context.GetArgument<object>("null");

		var format = context.GetArgument<string>("format");

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

			value = format.IsNotBlank() ? dateTime.ToString(format, InvariantCulture) : dateTime;
		}
		else if (value is DateTimeOffset dateTimeOffset)
		{
			var timeZone = context.GetArgument<string>("timeZone");
			if (timeZone.IsNotBlank())
				dateTimeOffset = dateTimeOffset.ToTimeZone(timeZone);

			value = format.IsNotBlank() ? dateTimeOffset.ToString(format, InvariantCulture) : dateTimeOffset;
		}
		else if (value is string text)
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
				var match = text.RegexMatch(pattern);
				if (match.Success)
					text = match.Value;
				else
					return null;
			}

			var length = context.GetArgument<int?>("length");
			if (text.Length > length)
				text = text.Left(length.Value);

			text = context.GetArgument<StringCase?>("case") switch
			{
				StringCase.Lower => text.ToLower(),
				StringCase.LowerInvariant => text.ToLowerInvariant(),
				StringCase.Upper => text.ToUpper(),
				StringCase.UpperInvariant => text.ToUpperInvariant(),
				_ => text
			};

			value = text;
		}
		else if (format.IsNotBlank())
			value = string.Format(InvariantCulture, Invariant($"{{0:{format}}}"), value);

		return value;
	}
}
