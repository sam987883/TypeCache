// Copyright (c) 2021 Samuel Abraham

namespace TypeCache.GraphQL.Types;

/// <summary>
/// <c>SampleEnumValue</c> ---> <c>SAMPLE_ENUM_VALUE</c>
/// </summary>
[AttributeUsage(AttributeTargets.Enum, AllowMultiple = false)]
public sealed class ConstantCaseAttribute : Attribute
{
}
