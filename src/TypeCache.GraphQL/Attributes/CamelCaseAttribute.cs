// Copyright (c) 2021 Samuel Abraham

namespace TypeCache.GraphQL.Types;

/// <summary>
/// <c>SampleEnumValue</c> ---> <c>sampleEnumValue</c>
/// </summary>
[AttributeUsage(AttributeTargets.Enum, AllowMultiple = false)]
public sealed class CamelCaseAttribute : Attribute
{
}
