// Copyright (c) 2021 Samuel Abraham

using System;

namespace TypeCache.Attributes;

[Flags]
public enum SqlApiAction
{
	None,
	Select = 1,
	Delete = 2,
	DeleteData = 4,
	Insert = 8,
	InsertData = 16,
	Update = 32,
	UpdateData = 64,
	CRUD = Select | Delete | DeleteData | Insert | InsertData | Update | UpdateData,
	Truncate = 128,
	All = Select | Delete | DeleteData | Insert | InsertData | Update | UpdateData | Truncate
}
