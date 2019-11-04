using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
	[SerializeField]
	private PooledObject _objectToSpawn = default;

	[SerializeField]
	private Transform _objectParent = default;
	
	private Stack<PooledObject> _pool = new Stack<PooledObject>();

	public void returnToPool( PooledObject obj )
	{
		obj.gameObject.SetActive(false);
		_pool.Push(obj);
	}
	
	public PooledObject getFromPool( )
	{
		PooledObject obj = null;
		if (_pool.Count > 0)
		{
			obj = _pool.Pop();
			obj.gameObject.SetActive(true);
		}
		else if (_objectToSpawn != null)
		{
			obj = Instantiate<PooledObject>(_objectToSpawn, transform.position, Quaternion.identity);
		}

		if (obj != null)
		{
			obj.spawn(this);
			if (_objectParent != null)
			{
				obj.transform.parent = _objectParent;
			}
		}
		
		return obj;
	}
}
