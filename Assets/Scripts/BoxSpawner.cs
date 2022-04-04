using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class BoxSpawner : MonoBehaviour
{
    public static float SpawnInterval = 3;
    public static int ColorSelectionMax = 0;
    
    private Bounds _bounds;
    
    private void Awake()
    {
        _bounds = GetComponent<Collider>().bounds;
    }

    private void OnEnable()
    {
        StartCoroutine(RandomSpawn());
    }

    private IEnumerator RandomSpawn()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(SpawnInterval, SpawnInterval + 1));
            var loc = new Vector3(
                Random.Range(_bounds.min.x, _bounds.max.x),
                Random.Range(_bounds.min.y, _bounds.max.y),
                Random.Range(_bounds.min.z, _bounds.max.z));

            var color = (BoxColor) Random.Range(0, ColorSelectionMax);
            var box = ObjectPool.Pool.SpawnBox(loc, color);
            if (box == null)
                continue;
            
            var force = transform.TransformVector(0, -1, -1) * Random.Range(50, 200);
            box.GetComponent<Rigidbody>().AddForce(force);
        }
    }
}