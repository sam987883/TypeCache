// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using TypeCache.Extensions;

namespace TypeCache.Data.Schema
{
	public class SchemaValidator
	{
		private readonly ObjectSchema _ObjectSchema;

		public SchemaValidator(ObjectSchema objectSchema)
			=> this._ObjectSchema = objectSchema;

		public void Validate(BatchRequest batch)
		{
			this._ObjectSchema.Type.Assert($"{nameof(batch)}.{nameof(batch.Table)}", ObjectType.Table);
			if (!batch.Delete && !batch.Update.Any() && !batch.Insert.Any())
				throw new ArgumentException($"[{nameof(batch)}] must have either {nameof(batch.Delete)} selected, {nameof(batch.Insert)} columns or {nameof(batch.Update)} columns.", nameof(batch));
			if (batch.Delete || batch.Update.Any())
				this.ValidateInputColumnsHavePrimaryKeys($"{nameof(batch)}.{nameof(batch.Input)}.{nameof(batch.Input.Columns)}", batch.Input.Columns);
			if (batch.Insert.Any())
			{
				this.ValidateColumns($"{nameof(batch)}.{nameof(batch.Insert)}", batch.Insert);
				this.ValidateInputDataColumns($"{nameof(batch)}.{nameof(batch.Insert)}", batch.Input.Columns, batch.Insert);
			}
			if (batch.Update.Any())
			{
				this.ValidateColumns($"{nameof(batch)}.{nameof(batch.Update)}", batch.Update);
				this.ValidateInputDataColumns($"{nameof(batch)}.{nameof(batch.Update)}", batch.Input.Columns, batch.Update);
			}
			this.ValidateInputRows($"{nameof(batch)}.{nameof(batch.Input)}.{nameof(batch.Input.Rows)}", batch.Input);
			this.ValidateAliases($"{nameof(batch)}.{nameof(batch.Output)}", batch.Output);
		}

		public void Validate(DeleteRequest delete)
		{
			this._ObjectSchema.Type.Assert($"{nameof(delete)}.{nameof(delete.From)}", ObjectType.Table);
			this.ValidateAliases($"{nameof(delete)}.{nameof(delete.Output)}", delete.Output);
		}

		public void Validate(InsertRequest insert)
		{
			this._ObjectSchema.Type.Assert($"{nameof(insert)}.{nameof(insert.Into)}", ObjectType.Table);
			this.ValidateColumns($"{nameof(insert)}.{nameof(insert.Insert)}", insert.Insert);
			this.ValidateAliases($"{nameof(insert)}.{nameof(insert.Output)}", insert.Output);
			this.ValidateAliases($"{nameof(insert)}.{nameof(insert.Select)}", insert.Select);
		}

		public void Validate(SelectRequest select)
		{
			this.ValidateAliases($"{nameof(select)}.{nameof(select.Select)}", select.Select);
		}

		public void Validate(UpdateRequest update)
		{
			this._ObjectSchema.Type.Assert($"{nameof(update)}.{nameof(update.Table)}", ObjectType.Table);
			this.ValidateAliases($"{nameof(update)}.{nameof(update.Output)}", update.Output);
			var columns = update.Set.To(set => set.Column).ToList();
			this.ValidateColumns($"{nameof(update)}.{nameof(update.Set)}", columns);
			this.ValidateColumnsWritable($"{nameof(update)}.{nameof(update.Set)}", columns);
		}

		public void Validate(StoredProcedureRequest storedProcedure)
		{
			this._ObjectSchema.Type.Assert(nameof(storedProcedure), ObjectType.StoredProcedure);
			if (storedProcedure.Parameters.Any())
				this.ValidateParameters($"{nameof(storedProcedure)}.{nameof(storedProcedure.Parameters)}", storedProcedure.Parameters);
		}

		private void ValidateInputColumnsHavePrimaryKeys(string parameter, string[] columns)
		{
			if (columns.Any())
			{
				var primaryKeys = this._ObjectSchema.Columns.If(column => column.PrimaryKey).To(column => column.Name).ToList();
				if (primaryKeys.Any())
				{
					if (!columns.Has(primaryKeys, StringComparer.OrdinalIgnoreCase))
						throw new ArgumentException("Input columns must contain all primary keys to use batch DELETE/UPDATE.", parameter);
				}
				else
					throw new ArgumentException($"Table {this._ObjectSchema.Name} must have primary key(s) defined to use batch DELETE/UPDATE.", parameter);
			}
			else
				throw new ArgumentException("Batch requires input columns.", parameter);
		}

		private void ValidateInputDataColumns(string parameter, IEnumerable<string> inputColumns, IEnumerable<string> dataColumns)
		{
			if (dataColumns.Any())
			{
				var invalidColumnCsv = dataColumns.Without(inputColumns, StringComparer.OrdinalIgnoreCase).ToCsv(_ => _.EscapeIdentifier());
				if (!invalidColumnCsv.IsBlank())
					throw new ArgumentException($"Column selections are not available from the input: {invalidColumnCsv}", parameter);

				var writableColumns = this._ObjectSchema.Columns.If(column => !column.Readonly).To(column => column.Name);
				invalidColumnCsv = dataColumns.Without(writableColumns).ToCsv(column => $"[{column}]");
				if (!invalidColumnCsv.IsBlank())
					throw new ArgumentException($"Column selections for table {this._ObjectSchema.Name} contain non-writable columns: {invalidColumnCsv}", parameter);
			}
			else
				throw new ArgumentException("Columns are required.", parameter);
		}

		private void ValidateAliases(string parameter, IEnumerable<OutputExpression>? outputExpressions)
		{
			if (outputExpressions.Any())
			{
				var aliases = outputExpressions.To(_ => _.As).IfNotBlank();
				var uniqueAliases = aliases.ToHashSet(StringComparer.OrdinalIgnoreCase);
				if (aliases.Count() != uniqueAliases.Count)
					throw new ArgumentException($"Duplicate aliases in SELECT statement.", parameter);
			}
		}

		private void ValidateColumns(string parameter, IEnumerable<string> columns)
		{
			if (columns.Any())
			{
				var invalidColumnCsv = columns.Without(this._ObjectSchema.Columns.To(column => column.Name)).ToCsv(column => $"[{column}]");
				if (!invalidColumnCsv.IsBlank())
					throw new ArgumentException($"Columns do not exist: {invalidColumnCsv}", parameter);
			}
		}

		private void ValidateColumnsWritable(string parameter, IEnumerable<string> columns)
		{
			if (columns.Any())
			{
				var writableColumns = this._ObjectSchema.Columns
					.If(column => !column.Readonly)
					.To(column => column.Name);
				var invalidColumnCsv = columns
					.Without(writableColumns)
					.ToCsv(column => $"[{column}]");
				if (!invalidColumnCsv.IsBlank())
					throw new ArgumentException($"Must not include non-writable columns: {invalidColumnCsv}", parameter);
			}
			else
				throw new ArgumentException("Writable columns are required.", parameter);
		}

		private void ValidateInputRows(string parameter, RowSet input)
		{
			if (input.Rows.Any())
			{
				var invalidRowCount = input.Rows
					.If(row => row.Length != input.Columns.Length)
					.Count();
				if (invalidRowCount > 0)
					throw new ArgumentException($"{invalidRowCount} input rows have a different number of values than the {input.Columns.Length} columns.", parameter);
			}
			else
				throw new ArgumentException("Input rows are required.", parameter);
		}

		private void ValidateParameters(string parameter, IEnumerable<Parameter> parameters)
		{
			var invalidParameterCsv = parameters
				.To(_ => _.Name)
				.Without(this._ObjectSchema.Parameters
					.If(_ => !_.Return)
					.To(_ => _.Name))
				.ToCsv();
			if (!invalidParameterCsv.IsBlank())
				throw new ArgumentException($"{this._ObjectSchema.Name} does not have the following parameters: {invalidParameterCsv}.",
					parameter);
		}
	}
}
