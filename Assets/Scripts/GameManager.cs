using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Level")]
    [SerializeField] public int maxToSpawnObjects = 10;

    [Header("Physics Variables")]
    [SerializeField] [Range(5, 10)] public float minForceSpeed = 5f;
    [SerializeField] [Range(10, 20)] public float maxForceSpeed = 5f;
    [SerializeField] [Range(0, 10)] public float maxTorqueYSpeed = 5f;
    [SerializeField] [Range(0, 10)] public float maxTorqueXSpeed = 5f;

    [Header("Spawning")]
    [SerializeField] public float spawnXRange = 10f;
    [SerializeField] public int spawnSpeed = 2;
    [SerializeField] public int minObjectsPerSpawn = 1;
    [SerializeField] public int maxObjectsPerSpawn = 3;

    [Header("Health")]
    [SerializeField] public int maxHealth = 3;
    [SerializeField] public List<GameObject> heartsUI;

    [Header("References")]
    [SerializeField] public GameObject[] prefabs;

    [Header("Buttons")]
    [SerializeField] public GameObject restartButton;

    private float currentForceSpeed;
    private float currentTorqueYspeed;
    private float currentTorqueXspeed;

    private int currentHealth;
    private int currentObjectPerSpawn;

    private Scene scene;
    private GameObject playerBlade;

    private void Awake()
    {
        //clean up
        SceneManager.sceneLoaded -= OnSceneLoaded;

        SceneManager.sceneLoaded += OnSceneLoaded;

        scene = SceneManager.GetActiveScene();
    }
    void Start()
    {
        if (!instance)
            instance = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        //set currentHealth
        currentHealth = maxHealth;

        //start launching objects
        StartCoroutine(LaunchObject());

    }
    IEnumerator LaunchObject()
    {
        for (int i = 0; i < maxToSpawnObjects; i++)
        {
            RandomObjectsPerSpawn(minObjectsPerSpawn, maxObjectsPerSpawn);

            for (int j = 0; j < currentObjectPerSpawn; j++)
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
                }
            }

            yield return new WaitForSeconds(spawnSpeed);
        }

    }
    private void RandomObjectsPerSpawn(int min, int max)
    {
        currentObjectPerSpawn = Random.Range(min, max);
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
    public void DecreaseHealth()
    {
        currentHealth--;

        //Update UI
        if (heartsUI.Count >= 0)
        {
            //deactivate object and remove from list
            heartsUI[heartsUI.Count - 1].SetActive(false);
            heartsUI.RemoveAt(heartsUI.Count - 1);
        }

        if (currentHealth <= 0)
        {
            //GAME OVER DAMNIT
            restartButton.SetActive(true);
            playerBlade.SetActive(false);
            Time.timeScale = 0;
        }
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        //do stuff when scene is loaded
        GameObject[] tempHeartsUI = GameObject.FindGameObjectsWithTag("HeartUI");

        //assign hearts ui
        for (int i = 0; i < tempHeartsUI.Length; i++)
            heartsUI.Add(tempHeartsUI[i]);

        //assign restart button
        restartButton = GameObject.Find("Restart_Button");
        if (restartButton) restartButton.SetActive(false);

        //assign player blade
        playerBlade = GameObject.FindGameObjectWithTag("Blade");

    }
    public void RestartSpawning()
    {
        foreach (Transform child in gameObject.transform)
            Destroy(child.gameObject);

        maxToSpawnObjects = 999;

        Time.timeScale = 1f;
    }
}
