using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSpawner : MonoBehaviour
{
    [SerializeField]
    private int _nItemsToSpawn = 50;
    [SerializeField]
    private int _itemsToSpawnPerFrame = 1;
    private int _itemsSpawnedInUpdate = 0;
    [SerializeField]
    private int _maxItemsSpawnedItemsInUpdate = 2500;
    
    [SerializeField]
    private GameObject[] _objectsToSpawn = default;
    [SerializeField]
    private AnimationCurve _spawnObjectCurve = default;

    [SerializeField]
    private Vector3MinMaxSet _spawnPos = default;
    [SerializeField]
    private AnimationCurve3 _spawnPosCurve = default;

    void Start()
    {
        spawnItems(_nItemsToSpawn);
    }

    void Update()
    {
        if (_itemsSpawnedInUpdate < _maxItemsSpawnedItemsInUpdate)
        {
            spawnItems(_itemsToSpawnPerFrame);
            _itemsSpawnedInUpdate += _itemsToSpawnPerFrame;
        }
    }

    private void spawnItems(int nItems)
    {
        for (int i = 0; i < nItems; i++)
        {
            float spanwIndexNormalized = _spawnObjectCurve.Evaluate(Random.value);
            int spawnIndex = (int)(spanwIndexNormalized * (float)(_objectsToSpawn.Length - 1));

            Vector3 spawnPosNormal = _spawnPosCurve.Evaluate3Random();
            Vector3 spawnPos = _spawnPos.min;
            spawnPos.x = _spawnPos.getLerpedX(spawnPosNormal.x);
            spawnPos.y = _spawnPos.getLerpedY(spawnPosNormal.y);
            spawnPos.z = _spawnPos.getLerpedZ(spawnPosNormal.z);

            GameObject obj = Instantiate(_objectsToSpawn[spawnIndex], spawnPos, Quaternion.identity);
            obj.transform.parent = gameObject.transform;
        }
    }
}
