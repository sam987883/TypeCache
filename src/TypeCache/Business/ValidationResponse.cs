// Copyright (c) 2021 Samuel Abraham

using System;

namespace TypeCache.Business
{
	public record ValidationResponse(Exception? Exception)
	{
		public static ValidationResponse Success
			=> new ValidationResponse(null as Exception);

		public bool HasError => this.Exception != null;
	};
}
