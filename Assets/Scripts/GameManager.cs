using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Level")]
    [SerializeField] public int maxToSpawnObjects = 10;

    [Header("Physics Variables")]
    [SerializeField] [Range(5, 10)] public float minForceSpeed = 5f;
    [SerializeField] [Range(10, 20)] public float maxForceSpeed = 5f;
    [SerializeField] [Range(0, 10)] public float maxTorqueYSpeed = 5f;
    [SerializeField] [Range(0, 10)] public float maxTorqueXSpeed = 5f;

    private float currentForceSpeed;
    private float currentTorqueYspeed;
    private float currentTorqueXspeed;

    [Header("Spawning")]
    [SerializeField] public float spawnXRange = 10f;
    [SerializeField] public int spawnSpeed = 2;

    [Header("References")]
    [SerializeField] public GameObject[] prefabs;

    void Start()
    {
        StartCoroutine(LaunchObject());
    }

    IEnumerator LaunchObject()
    {
        for (int i = 0; i < maxToSpawnObjects; i++)
        {
            //spawn random object from prefabs
            int randomPrefabIndex = Random.Range(0, prefabs.Length);

            //get random X pos based on transform
            float randomXpos = Random.Range(-spawnXRange, spawnXRange);

            GameObject spawnedObject = Instantiate(prefabs[randomPrefabIndex], transform.position + Vector3.right * randomXpos, Quaternion.identity, transform);

            //add random torque speed
            RandomTorqueSpeed();
            //add random force
            RandomForce();

            if (spawnedObject.TryGetComponent(out Rigidbody rb))
            {
                rb.AddTorque(Vector3.right * currentTorqueXspeed + Vector3.up * currentTorqueYspeed, ForceMode.Impulse);
                rb.AddForce(Vector3.up * currentForceSpeed, ForceMode.Impulse);

                yield return new WaitForSeconds(spawnSpeed);
            }
        }

    }

    private void RandomForce()
    {
        currentForceSpeed = Random.Range(minForceSpeed, maxForceSpeed);
    }
    private void RandomTorqueSpeed()
    {
        currentTorqueXspeed = Random.Range(-maxTorqueXSpeed, maxTorqueXSpeed);
        currentTorqueYspeed = Random.Range(-maxTorqueYSpeed, maxTorqueYSpeed);
    }
}
