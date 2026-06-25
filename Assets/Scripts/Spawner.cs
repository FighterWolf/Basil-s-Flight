using UnityEngine;
using System.Collections.Generic;

public class Spawner : MonoBehaviour
{

    public Entity[] entitiesToSpawn;
    public List<Entity> entitiesLinkedToSpawner = new List<Entity>();

    public bool allowRandomYHeight;

    public float maxNumberOfEntities;

    public float timeBetweenEntitySpawn;
    [SerializeField] private float stopwatch;

    public Transform[] waypointsToFollow;

    private LevelHandler lvlHandler;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        lvlHandler = FindFirstObjectByType<LevelHandler>();
    }

    // Update is called once per frame
    void Update()
    {
        if (lvlHandler)
        {
            if (lvlHandler.numberOfKillsNeeded > 0)
            {
                maxNumberOfEntities = lvlHandler.numberOfKillsNeeded - lvlHandler.currentKillPoints;
            }
        }
        
        CleanListOfDestroyedEntity();

        if (!LevelHandler.isLevelComplete)
        {
            if (stopwatch < timeBetweenEntitySpawn && entitiesLinkedToSpawner.Count < maxNumberOfEntities)
            {
                stopwatch += Time.deltaTime;
            }

            if (stopwatch >= timeBetweenEntitySpawn && entitiesLinkedToSpawner.Count < maxNumberOfEntities)
            {
                SpawnEntity();
                stopwatch = 0;
            }
        }
    }

    void SpawnEntity()
    {
        float randomX = Random.Range(-8, 8);
        float randomZ = Random.Range(-8, 8);

        float randomY = allowRandomYHeight ? Random.Range(-8, 8) : 0;

        Vector3 spawnCoords = new Vector3(transform.position.x+randomX,transform.position.y+randomY,transform.position.z+randomZ);

        int index = Random.Range(0,entitiesToSpawn.Length-1);

        GameObject newEntity = Instantiate(entitiesToSpawn[index].gameObject,spawnCoords,Quaternion.identity);
        entitiesLinkedToSpawner.Add(newEntity.GetComponent<Entity>());
        if (waypointsToFollow.Length > 0)
        {
            newEntity.GetComponent<Entity>().waypoints = waypointsToFollow;
        }
    }

    void CleanListOfDestroyedEntity()
    {
        entitiesLinkedToSpawner.RemoveAll(deletedEntity => deletedEntity == null);
        entitiesLinkedToSpawner.RemoveAll(destroyedEntity => destroyedEntity.isDisabled);
    }
}
