// Copyright (c) 2021 Samuel Abraham

using TypeCache.GraphQL.Extensions;

namespace TypeCache.GraphQL.Types;

/// <summary>
/// <c>sampleEnumValue</c> ---> <c>SampleEnumValue</c>
/// </summary>
[AttributeUsage(AttributeTargets.Enum, AllowMultiple = false)]
public sealed class PascalCaseAttribute : Attribute
{
}
