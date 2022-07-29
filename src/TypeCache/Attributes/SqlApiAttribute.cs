// Copyright (c) 2021 Samuel Abraham

using System;

namespace TypeCache.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class SqlApiAttribute : Attribute
{
}
