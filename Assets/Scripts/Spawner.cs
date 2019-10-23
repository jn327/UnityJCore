using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
	public GameObject objectToSpawn;
	private float _spawnFrequencyMin = 0.5f;
	private float _spawnFrequencyMax = 1.0f;
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
		Instantiate(objectToSpawn, transform.position, Quaternion.identity);

		_nextSpawnTime = Time.time + Random.Range(_spawnFrequencyMin, _spawnFrequencyMax);

		DebugManager.addLogEvent("Spawning something");
	}
}
