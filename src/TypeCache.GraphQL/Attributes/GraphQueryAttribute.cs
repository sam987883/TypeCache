﻿// Copyright (c) 2021 Samuel Abraham

using System;

namespace TypeCache.GraphQL.Attributes
{
	/// <summary>
	/// <b>GraphQL</b><br />
	/// Marks a method to be used as a root level Query endpoint.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class GraphQueryAttribute : Attribute
    {
    }
}
