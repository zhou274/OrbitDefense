////////////////////////////////////////////////////////////////////////////
// bl_SpawnerManager
//
//
//                    Lovatto Studio 2016
////////////////////////////////////////////////////////////////////////////
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class bl_SpawnerManager : Singleton<bl_SpawnerManager>
{
    [Header("Settings")]
    public int PrefabsPoolLenght = 20;
    public Vector2 SpawnBetween = new Vector2(0.2f, 0.5f);

    [Header("References")]
    [SerializeField]private RectTransform[] SpawnerPositions;
    [SerializeField]private GameObject[] SpawnerPrefabs;
    [SerializeField]private Transform EnemyParent;

    private List<GameObject> PoolPrefabs = new List<GameObject>();
    private int currentPool = 0;

    /// <summary>
    /// 
    /// </summary>
    void Start()
    {
        for(int i = 0; i < PrefabsPoolLenght; i++)
        {
            GameObject pref = Instantiate(SpawnerPrefabs[Random.Range(0, SpawnerPrefabs.Length)]) as GameObject;
            // pref.transform.SetParent(EnemyParent, false);
            pref.transform.parent = EnemyParent;
            PoolPrefabs.Add(pref);
            pref.SetActive(false);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public void Spawn()
    {
        StartCoroutine(SpawnLoop());
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    IEnumerator SpawnLoop()
    {
        while (true)
        {
            int spp = Random.Range(0, SpawnerPositions.Length);
            Vector3 position = SpawnerPositions[spp].position;
            position.x = Random.Range(position.x - (SpawnerPositions[spp].sizeDelta.x * 0.5f), position.x + (SpawnerPositions[spp].sizeDelta.x * 0.5f));
            position.x += (Random.Range(-5, 5));
            PoolPrefabs[currentPool].transform.position = position;
            PoolPrefabs[currentPool].SetActive(true);

            currentPool = (currentPool + 1) % PrefabsPoolLenght;
            float t = Random.Range(SpawnBetween.x, SpawnBetween.y);
            yield return new WaitForSeconds(t);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public void HideAll()
    {
        StopAllCoroutines();
        foreach(GameObject g in PoolPrefabs)
        {
            g.SetActive(false);
        }
    }
    public void ResumeSpawn()
    {
        StartCoroutine(SpawnLoop());
    }
    public static bl_SpawnerManager Instance
    {
        get
        {
            return ((bl_SpawnerManager)mInstance);
        }
        set
        {
            mInstance = value;
        }
    }
}