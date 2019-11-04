﻿using UnityEngine;

public class Spawner : ObjectPool
{	
	[SerializeField]
    private MinMaxFloat _spawnFrequency = new MinMaxFloat( 0.5f, 1.0f );

	private float _nextSpawnTime = 0;

	// Update is called once per frame
	void Update () 
	{
		if (Time.time >= _nextSpawnTime)
		{
			doSpawn();
		}
	}

	private void doSpawn()
	{
		GameObject obj = getFromPool().gameObject;
		obj.transform.position = transform.position;
		obj.transform.rotation = Quaternion.identity;

		_nextSpawnTime = Time.time + _spawnFrequency.GetRandom();
	}
}