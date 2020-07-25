// Copyright (c) 2020 Samuel Abraham

using sam987883.Common.Extensions;
using sam987883.Common.Models;
using sam987883.Database.Models;
using System;
using System.Collections.Generic;

namespace sam987883.Database
{
	public class SchemaValidator
	{
		private readonly ObjectSchema _ObjectSchema;

		public SchemaValidator(ObjectSchema objectSchema) =>
			this._ObjectSchema = objectSchema;

		public void Validate(BatchRequest batch)
		{
			this._ObjectSchema.Type.Assert($"{nameof(BatchRequest)}.{nameof(batch.Table)}", ObjectType.Table);
			if (batch.Delete || batch.Update.Any())
				this.ValidateInputColumnsHavePrimaryKeys($"{nameof(BatchRequest)}.{nameof(batch.Input)}.{nameof(batch.Input.Columns)}", batch.Input.Columns);
			if (batch.Insert.Any())
				this.ValidateInputDataColumns($"{nameof(BatchRequest)}.{nameof(batch.Insert)}", batch.Input.Columns, batch.Insert);
			if (batch.Update.Any())
				this.ValidateInputDataColumns($"{nameof(BatchRequest)}.{nameof(batch.Update)}", batch.Input.Columns, batch.Update);
			this.ValidateInputRows($"{nameof(BatchRequest)}.{nameof(batch.Input)}.{nameof(batch.Input.Rows)}", batch.Input);
			if (batch.Delete || batch.Update.Any())
				this.ValidateOnColumns($"{nameof(BatchRequest)}.{nameof(batch.On)}", batch.On);
			this.ValidateOutputColumns($"{nameof(BatchRequest)}.{nameof(batch.OutputDeleted)}", batch.OutputDeleted);
			this.ValidateOutputColumns($"{nameof(BatchRequest)}.{nameof(batch.OutputInserted)}", batch.OutputInserted);
		}

		public void Validate(DeleteRequest delete)
		{
			this._ObjectSchema.Type.Assert($"{nameof(DeleteRequest)}.{nameof(delete.From)}", ObjectType.Table);
			this.ValidateExpression($"{nameof(DeleteRequest)}.{nameof(delete.Where)}", delete.Where);
			this.ValidateOutputColumns($"{nameof(DeleteRequest)}.{nameof(delete.Output)}", delete.Output);
		}

		public void Validate(UpdateRequest update)
		{
			this._ObjectSchema.Type.Assert($"{nameof(UpdateRequest)}.{nameof(update.Table)}", ObjectType.Table);
			this.ValidateColumnsWritable($"{nameof(UpdateRequest)}.{nameof(update.Set)}", update.Set.To(set => set.Column).ToList());
			this.ValidateExpression($"{nameof(UpdateRequest)}.{nameof(update.Where)}", update.Where);
			this.ValidateOutputColumns($"{nameof(UpdateRequest)}.{nameof(update.OutputDeleted)}", update.OutputDeleted);
			this.ValidateOutputColumns($"{nameof(UpdateRequest)}.{nameof(update.OutputInserted)}", update.OutputInserted);
		}

		public void Validate(SelectRequest select)
		{
			this.ValidateAliases($"{nameof(select)}.{nameof(select.Output)}", select.Output.To(_ => _.Alias).If(alias => !alias.IsBlank()).ToList());
			this.ValidateColumns($"{nameof(select)}.{nameof(select.Output)}", select.Output.To(_ => _.Column).ToList());
			this.ValidateExpression($"{nameof(select)}.{select.Where}", select.Where);
			this.ValidateExpression($"{nameof(select)}.{select.Having}", select.Having);
			this.ValidateColumns($"{nameof(select)}.{nameof(select.OrderBy)}", select.OrderBy.To(orderBy => orderBy.Column).ToList());
		}

		public void Validate(StoredProcedureRequest storedProcedure)
		{
			this._ObjectSchema.Type.Assert(nameof(storedProcedure), ObjectType.StoredProcedure);
			if (storedProcedure.Parameters != null)
				this.ValidateParameters($"{nameof(StoredProcedureRequest)}.{nameof(storedProcedure.Parameters)}", storedProcedure.Parameters.Keys.ToList().ToArray());
		}

		private void ValidateExpression(string parameter, ExpressionSet? expressionSet)
		{
			if (expressionSet == null)
				return;

			if (expressionSet.Expressions.Any())
			{
				var fields = expressionSet.Expressions.To(expression => expression.Field);
				var columns = this._ObjectSchema.Columns.To(column => column.Name);
				var invalidFieldCsv = fields.Without(columns).ToCsv();
				if (!invalidFieldCsv.IsBlank())
					throw new ArgumentException($"{this._ObjectSchema.Name} does not have the following columns: {invalidFieldCsv}.",
						parameter);

				var stringFields = expressionSet.Expressions
					.If(expression => expression.Operator == ComparisonOperator.Like
						|| expression.Operator == ComparisonOperator.NotLike
						|| expression.Operator == ComparisonOperator.StartWith
						|| expression.Operator == ComparisonOperator.NotStartWith
						|| expression.Operator == ComparisonOperator.EndWith
						|| expression.Operator == ComparisonOperator.NotEndWith)
					.To(expression => expression.Field);
				var stringColumns = this._ObjectSchema.Columns.If(column => column.Type.Contains("char", StringComparison.OrdinalIgnoreCase)).To(column => column.Name);
				invalidFieldCsv = stringFields.Without(stringColumns).ToCsv();
				if (!invalidFieldCsv.IsBlank())
					throw new ArgumentException($"The following columns in {this._ObjectSchema.Name} cannot use string operators: {invalidFieldCsv}.",
						parameter);
			}

			expressionSet.ExpressionSets.Do(item => this.ValidateExpression(parameter, item));
		}

		private void ValidateInputColumnsHavePrimaryKeys(string parameter, string[] columns)
		{
			if (columns.Any())
			{
				var primaryKeys = this._ObjectSchema.Columns.If(column => column.PrimaryKey).To(column => column.Name).ToList();
				if (primaryKeys.Any())
				{
					if (!columns.Has(primaryKeys, StringComparer.OrdinalIgnoreCase))
						throw new ArgumentException("Input columns must contain all primary keys for batch UPDATE.", parameter);
				}
				else
					throw new ArgumentException($"Table {this._ObjectSchema.Name} must have a primary key for batch UPDATE.", "Table");
			}
			else
				throw new ArgumentException("No input columns for batch UPDATE.", parameter);
		}

		private void ValidateInputDataColumns(string parameter, IEnumerable<string> inputColumns, IEnumerable<string> dataColumns)
		{
			if (dataColumns.Any())
			{
				var invalidColumnCsv = dataColumns.Without(inputColumns).ToCsv(column => $"[{column}]");
				if (!invalidColumnCsv.IsBlank())
					throw new ArgumentException($"Data columns contain columns that are not available from the input: {invalidColumnCsv}", parameter);

				var writableColumns = this._ObjectSchema.Columns.If(column => !column.Readonly).To(column => column.Name);
				invalidColumnCsv = dataColumns.Without(writableColumns).ToCsv(column => $"[{column}]");
				if (!invalidColumnCsv.IsBlank())
					throw new ArgumentException($"Data columns for table {this._ObjectSchema.Name} contain non-writable columns: {invalidColumnCsv}", parameter);
			}
			else
				throw new ArgumentException("Columns are required.", parameter);
		}

		private void ValidateAliases(string parameter, IEnumerable<string> aliases)
		{
			if (aliases.Any())
			{
				var uniqueAliases = aliases.ToHashSet(StringComparer.OrdinalIgnoreCase);
				if (aliases.Count() != uniqueAliases.Count)
					throw new ArgumentException($"Duplicate aliases in SELECT statement.", parameter);
			}
		}

		private void ValidateColumns(string parameter, IEnumerable<string> columns)
		{
			if (columns.Any())
			{
				var writableColumns = this._ObjectSchema.Columns.To(column => column.Name);
				var invalidColumnCsv = columns.Without(writableColumns).ToCsv(column => $"[{column}]");
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

		private void ValidateOnColumns(string parameter, IEnumerable<string> columns)
		{
			if (columns.Any())
			{
				var primaryKeys = this._ObjectSchema.Columns
					.If(column => column.PrimaryKey)
					.To(column => column.Name)
					.ToList();
				if (primaryKeys.Any() && !columns.Has(primaryKeys, StringComparer.OrdinalIgnoreCase))
					throw new ArgumentException("ON columns must contain all primary keys for SQL MERGE.", parameter);
			}
			else
				throw new ArgumentException("SQL MERGE requires ON columns.", parameter);
		}

		private void ValidateOutputColumns(string parameter, IEnumerable<string> outputColumns)
		{
			if (outputColumns.Any())
			{
				var columns = this._ObjectSchema.Columns.To(column => column.Name);
				var invalidColumnCsv = outputColumns
					.Without(columns)
					.ToCsv(column => $"[{column}]");
				if (!invalidColumnCsv.IsBlank())
					throw new ArgumentException($"Invalid OUTPUT columns for table {this._ObjectSchema.Name}: {invalidColumnCsv}", parameter);
			}
		}

		private void ValidateParameters(string parameter, IEnumerable<string> parameters)
		{
			var invalidParameterCsv = parameters
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
