// Copyright (c) 2021 Samuel Abraham

using System;

namespace TypeCache.Attributes;

[Flags]
public enum SqlApiAction
{
	None,
	Select = 1,
	Page = 2,
	Delete = 4,
	DeleteData = 8,
	Insert = 16,
	InsertData = 32,
	Update = 64,
	UpdateData = 128,
	All = Select | Page | Delete | DeleteData | Insert | InsertData | Update | UpdateData
}
