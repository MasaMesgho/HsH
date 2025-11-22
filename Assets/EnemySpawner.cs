using NUnit.Framework;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject Enemy1;
    public GameObject Enemy2;
    public GameObject Enemy3;
    [SerializeField] private List<GameObject> EnemyList;
    [SerializeField] private int MaxEnemyCount = 15;
    [SerializeField] private int SpawnDelay = 10;
    [SerializeField] private int MinX = 0;
    [SerializeField] private int MinZ = 0;
    [SerializeField] private int MaxX = 0;
    [SerializeField] private int MaxZ = 0;
    private float timer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        int seconds = (int)(timer % 60);

        if (seconds > SpawnDelay & (EnemyList.Count < MaxEnemyCount))
        {
            Vector3 Spawn = new Vector3(Random.Range(MinX, MaxX), 100, Random.Range(MinZ, MaxZ));
            Quaternion Rotation = new Quaternion(-45, 0, 0, 0);
            GameObject target = Instantiate (Enemy1, Spawn, Quaternion.identity  );
            target.transform.position = Spawn;
            target.transform.rotation =  Rotation;
            EnemyList.Add(target);
            timer = 0;
        }

    }
}
