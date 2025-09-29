// Copyright (c) 2021 Samuel Abraham

using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using TypeCache.Collections;
using TypeCache.Extensions;

namespace TypeCache.Reflection;

[DebuggerDisplay("Property: {Name}")]
public sealed class PropertyEntity
{
	private readonly RuntimeTypeHandle _PropertyTypeHandle;
	private readonly string _QualifiedName;
	private readonly RuntimeTypeHandle _TypeHandle;
	private readonly Lazy<Delegate>? _GetValue;
	private readonly Lazy<Delegate>? _SetValue;

	public PropertyEntity(PropertyInfo propertyInfo)
	{
		propertyInfo.ThrowIfNull();
		propertyInfo.DeclaringType.ThrowIfNull();

		this._PropertyTypeHandle = propertyInfo.PropertyType.TypeHandle;
		this._QualifiedName = Invariant($"{propertyInfo.DeclaringType.Name}.{propertyInfo.Name}");
		this._TypeHandle = propertyInfo.DeclaringType.TypeHandle;

		this.Attributes = new ReadOnlyCollection<Attribute>(propertyInfo.GetCustomAttributes());
		this.CanRead = propertyInfo.CanRead;
		this.CanWrite = propertyInfo.CanWrite;
		this.IsStaticGet = propertyInfo.GetMethod?.IsStatic ?? false;
		this.IsStaticSet = propertyInfo.SetMethod?.IsStatic ?? false;
		this.Name = propertyInfo.Name;
		this.Parameters = propertyInfo.GetIndexParameters()
			.OrderBy(_ => _.Position)
			.Select(_ => new ParameterEntity(_))
			.ToArray();

		this._GetValue = propertyInfo switch
		{
			{ CanRead: true, GetMethod.IsStatic: true } => new(this.CreateStaticGetValueDelegate),
			{ CanRead: true, GetMethod.IsStatic: false } => new(this.CreateGetValueDelegate),
			_ => null
		};
		this._SetValue = propertyInfo switch
		{
			{ CanWrite: true, GetMethod.IsStatic: true } => new(this.CreateStaticSetValueDelegate),
			{ CanWrite: true, GetMethod.IsStatic: false } => new(this.CreateSetValueDelegate),
			_ => null
		};
	}

	public IReadOnlyCollection<Attribute> Attributes { get; }

	public bool CanRead { get; }

	public bool CanWrite { get; }

	public bool IsStaticGet { get; }

	public bool IsStaticSet { get; }

	public string Name { get; }

	public IReadOnlyList<ParameterEntity> Parameters { get; }

	public Type PropertyType => this._PropertyTypeHandle.ToType();

	public Type Type => this._TypeHandle.ToType();

	public PropertyIndexer this[ITuple index]
		=> new PropertyIndexer(this._QualifiedName, this._GetValue, this._SetValue, index);

	public object? GetStaticValue()
	{
		var getValue = this._GetValue?.Value as Func<object?>;
		getValue.ThrowIfNull(message: () => Invariant($"Property [{this._QualifiedName}] does not have a static property getter."));

		return getValue();
	}

	/// <param name="instance">The object instance to get the property value from.</param>
	public object? GetValue(object instance)
	{
		var getValue = this._GetValue?.Value as Func<object, object?>;
		getValue.ThrowIfNull(message: () => Invariant($"Property [{this._QualifiedName}] does not have a property getter."));

		return getValue(instance);
	}

	/// <param name="value">The property value to set.</param>
	public void SetStaticValue(object? value)
	{
		var setValue = this._SetValue?.Value as Action<object?>;
		setValue.ThrowIfNull(message: () => Invariant($"Property [{this._QualifiedName}] does not have a static property setter."));

		setValue(value);
	}

	/// <param name="instance">The object instance to set the property value on.</param>
	/// <param name="value">The property value to set.</param>
	public void SetValue(object instance, object? value)
	{
		var setValue = this._SetValue?.Value as Action<object, object?>;
		setValue.ThrowIfNull(message: () => Invariant($"Property [{this._QualifiedName}] does not have a property setter."));

		setValue(instance, value);
	}

	public bool TryGetStaticValue(out object? value)
	{
		var getValue = this._GetValue?.Value as Func<object?>;
		var success = getValue is not null;

		value = getValue?.Invoke();
		return success;
	}

	public bool TryGetValue(object instance, out object? value)
	{
		var getValue = this._GetValue?.Value as Func<object, object?>;
		var success = getValue is not null;

		value = getValue?.Invoke(instance);
		return success;
	}

	public bool TrySetStaticValue(object? value)
	{
		var setValue = this._SetValue?.Value as Action<object?>;
		var success = setValue is not null;

		setValue?.Invoke(value);
		return success;
	}

	public bool TrySetValue(object instance, object? value)
	{
		var setValue = this._SetValue?.Value as Action<object, object?>;
		var success = setValue is not null;

		setValue?.Invoke(instance, value);
		return success;
	}

	private Delegate CreateGetValueDelegate()
	{
		ParameterExpression instance = nameof(instance).ToParameterExpression<object>();
		if (this.Parameters.Count > 0)
		{
			ParameterExpression index = nameof(index).ToParameterExpression<ITuple>();
			var valueTupleType = MethodEntity.GetValueTupleType(this.Parameters);
			var parameters = MethodEntity.GetValueTupleFields(index.Cast(valueTupleType), this.Parameters.Count).ToArray();
			return instance.Cast(this.Type)
				.Property(this.Name, parameters)
				.As<object>()
				.Lambda<Func<object, ITuple, object?>>([instance, index])
				.Compile();
		}

		return instance
			.Cast(this.Type)
			.Property(this.Name)
			.As<object>()
			.Lambda<Func<object, object?>>([instance])
			.Compile();
	}

	private Delegate CreateSetValueDelegate()
	{
		ParameterExpression instance = nameof(instance).ToParameterExpression<object>();
		ParameterExpression value = nameof(value).ToParameterExpression<object?>();

		if (this.Parameters.Count > 0)
		{
			ParameterExpression index = nameof(index).ToParameterExpression<ITuple>();
			var valueTupleType = MethodEntity.GetValueTupleType(this.Parameters);
			var parameters = MethodEntity.GetValueTupleFields(index.Cast(valueTupleType), this.Parameters.Count).ToArray();

			return instance
				.Cast(this.Type)
				.Property(this.Name, parameters)
				.Assign(value.Convert(this.PropertyType))
				.Lambda<Action<object, ITuple, object?>>([instance, index, value])
				.Compile();
		}

		return instance
			.Cast(this.Type)
			.Property(this.Name)
			.Assign(value.Convert(this.PropertyType))
			.Lambda<Action<object, object?>>([instance, value])
			.Compile();
	}

	private Delegate CreateStaticGetValueDelegate()
	{
		var propertyInfo = this.Type.GetProperty(this.Name)!;
		if (this.Parameters.Count > 0)
		{
			ParameterExpression index = nameof(index).ToParameterExpression<ITuple>();
			var valueTupleType = MethodEntity.GetValueTupleType(this.Parameters);
			var parameters = MethodEntity.GetValueTupleFields(index.Cast(valueTupleType), this.Parameters.Count).ToArray();
			return propertyInfo
				.ToExpression(null, parameters)
				.As<object>()
				.Lambda<Func<ITuple, object?>>([index])
				.Compile();
		}

		return propertyInfo
			.ToExpression(null)
			.As<object>()
			.Lambda<Func<object?>>()
			.Compile();
	}

	private Delegate CreateStaticSetValueDelegate()
	{
		ParameterExpression value = nameof(value).ToParameterExpression<object?>();
		var propertyInfo = this.Type.GetProperty(this.Name)!;

		if (this.Parameters.Count > 0)
		{
			ParameterExpression index = nameof(index).ToParameterExpression<ITuple>();
			var valueTupleType = MethodEntity.GetValueTupleType(this.Parameters);
			var parameters = MethodEntity.GetValueTupleFields(index.Cast(valueTupleType), this.Parameters.Count).ToArray();

			return propertyInfo
				.ToExpression(null, parameters)
				.Assign(value.Convert(this.PropertyType))
				.Lambda<Action<ITuple, object?>>([index, value])
				.Compile();
		}

		return propertyInfo
			.ToExpression(null)
			.Assign(value.Convert(this.PropertyType))
			.Lambda<Action<object?>>([value])
			.Compile();
	}
}
