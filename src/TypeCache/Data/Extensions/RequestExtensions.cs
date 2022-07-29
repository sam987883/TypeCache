// Copyright (c) 2021 Samuel Abraham

using TypeCache.Data.Domain;

namespace TypeCache.Data.Extensions
{
	public static class RequestExtensions
	{
		public static CountCommand ToCountCommand(this DeleteCommand @this)
			=> new()
			{
				DataSource = @this.DataSource,
				Table = @this.Table,
				InputParameters = @this.InputParameters,
				Where = @this.Where
			};

		public static CountCommand ToCountCommand(this InsertCommand @this)
			=> new()
			{
				DataSource = @this.DataSource,
				Table = @this.From,
				InputParameters = @this.InputParameters,
				Where = @this.Where
			};

		public static CountCommand ToCountCommand(this SelectCommand @this)
			=> new()
			{
				DataSource = @this.DataSource,
				Table = @this.From,
				InputParameters = @this.InputParameters,
				Where = @this.Where
			};

		public static CountCommand ToCountCommand(this UpdateCommand @this)
			=> new()
			{
				DataSource = @this.DataSource,
				Table = @this.Table,
				InputParameters = @this.InputParameters,
				Where = @this.Where
			};
	}
}
