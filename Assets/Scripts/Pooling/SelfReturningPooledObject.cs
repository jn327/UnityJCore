using UnityEngine;

public class SelfReturningPooledObject : PooledObject
{
	[SerializeField]
    private MinMaxFloat _time = new MinMaxFloat( 0.5f, 1.0f );

	private float _despawnTime = 0;

	public void resetTimer () 
	{
		_despawnTime = Time.time + _time.GetRandom();
	}


	public override void despawn()
	{
		base.despawn(); 
	}

	public override void spawn ( ObjectPool pool ) 
	{
		base.spawn(pool);

		resetTimer();
	}

	// Update is called once per frame
	void Update () 
	{
		if (Time.time >= _despawnTime)
		{
			despawn();
		}
	}
}
