using UnityEngine;
using System.Collections;
using System;
/// <summary>
/// Heap class stores an array of objects T with a predetermined max object count, and handles the addition and removal of those objects
/// <para>Objects must implement the IHeapObject interface</para>
/// </summary>
/// <typeparam name="T"></typeparam>
public class Heap<T> where T : IHeapObject<T>
{

	T [] objects; // Array of objects currently part of this heap
	int currentObjectCount; // Number of objects currently part of this heap
	System.Random random; // Object that handles generating random values
	/// <summary>
	/// Returns the current number of objects that are part of this heap (Read Only)
	/// </summary>
	public int Count
	{
		get
		{
			return currentObjectCount;
		}
	}
	/// <summary>
	/// Returns the last object that is part of this heap
	/// </summary>
	public T Last
	{
		get
		{
			return objects [currentObjectCount-1];
		}
	}
	/// <summary>
	/// Constructs new heap of objects T of a predetermined max object count
	/// </summary>
	/// <param name="maxHeapCount"></param>
	public Heap (int maxHeapCount)
	{
		random = new System.Random (); // Construct the random generator used byt this heap instance
		objects = new T [maxHeapCount]; // Construct the heap array with the given max object count
	}
	/// <summary>
	/// Add a specified object to the heap
	/// </summary>
	public void Add (T newObject)
	{
		newObject.heapIndex = currentObjectCount;
		objects [currentObjectCount] = newObject;
		currentObjectCount++;
	}
	/// <summary>
	/// Remove the oldest object from this heap and return a reference of it to the caller
	/// </summary>
	/// <returns>T</returns>
	public T RemoveOldest ()
	{
		T oldestObject = objects [0]; // Get the oldest object in the heap (object at position 0)...
		Remove (oldestObject); // And remove it from the heap
		return oldestObject;
	}
	/// <summary>
	/// Remove a random object from this heap and return a reference of it to the caller
	/// </summary>
	/// <returns>T</returns>
	public T RemoveRandom ()
	{
		int randInt = random.Next (0, currentObjectCount); // Generate a random integer smaller than the current object count...
		T returnObject = objects [randInt]; // Get the object from the heap position of that integer...
		Remove (returnObject); // And remove it from the heap
		return returnObject;
    }
	/// <summary>
	/// Remove the specified object from this heap and adjust the remaining objects' heap positions and indexes accordingly
	/// </summary>
	/// <param name="specifiedObject"></param>
	public void Remove (T specifiedObject)
	{
		for (int i = specifiedObject.heapIndex; i < currentObjectCount - 1; i++) // Loop through all the objects behind the reomved object...
		{
			objects [i] = objects [i + 1]; // Adjust the position...
			objects [i].heapIndex--; // Heap Index...
		}
		currentObjectCount--; // And finally reduce the current object count to reflect the changes
	}
}
/// <summary>
/// Interface that allows an object to be part of a heap. Objects can only be part of one heap at a time
/// <para>Provides a Heap Index field that denotes the position of the object in its heap</para>
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IHeapObject<T>
{
	/// <summary>
	/// The object's index within the heap that it most recently occupies, does not imply it is currently part of a heap
	/// </summary>
	int heapIndex
	{
		get;
		set;
	}
}
