using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class RuntimeSet<T> : ScriptableObject //Add IEnumerable to use as a collection?
{
	private List<T> items = new List<T>();

	//Event

	public event Action<RuntimeSet<T>> OnRuntimeSetChanged;
	public event Action<RuntimeSet<T>> OnBeforeRuntimeSetCleared;

	public T GetItem(int index)
	{
		if (items.Count > 0)
			return items[index];
		else
			return default;
	}

	public void Add(T item)
	{
		if (!items.Contains(item))
			items.Add(item);

		OnRuntimeSetChanged?.Invoke(this);
	}

	public void AddRange(IEnumerable<T> collection)
	{
		items.AddRange(collection);
		OnRuntimeSetChanged?.Invoke(this);
	}

	public void Remove(T item)
	{
		if (items.Contains(item))
			items.Remove(item);

		OnRuntimeSetChanged?.Invoke(this);
	}

	public List<T> GetAll()
	{
		return items;
	}

	public void Clear()
	{
		OnBeforeRuntimeSetCleared?.Invoke(this);
		items.Clear();
		OnRuntimeSetChanged?.Invoke(this);
	}

	public int Count { get { return items.Count; } }
}
