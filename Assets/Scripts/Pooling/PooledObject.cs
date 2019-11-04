using UnityEngine;

public class PooledObject : MonoBehaviour
{
	[SerializeField]
	private bool _resetRigidbodyOnSpawn = true;
	private Rigidbody _rb;
	[SerializeField]
	private bool _resetRigidbody2DOnSpawn = false;
	private Rigidbody2D _rb2D;

    private ObjectPool _pool;

    public virtual void spawn( ObjectPool pool )
	{
		if (_resetRigidbodyOnSpawn)
		{
			if (!_rb)
			{
				_rb = GetComponent<Rigidbody>();
			}
			_rb.velocity = Vector3.zero;
			_rb.angularVelocity = Vector3.zero;
		}
		if (_resetRigidbody2DOnSpawn)
		{
			if (!_rb2D)
			{
				_rb2D = GetComponent<Rigidbody2D>();
			}
			_rb2D.velocity = Vector3.zero;
			_rb2D.angularVelocity = 0;
		}

		_pool = pool;
	}

    public virtual void despawn()
	{
		_pool.returnToPool(this);
	}
}
