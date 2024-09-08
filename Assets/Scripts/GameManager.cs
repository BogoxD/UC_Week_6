using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

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
    [SerializeField] public float spawnSpeed = 2;
    [SerializeField] public int minObjectsPerSpawn = 1;
    [SerializeField] public int maxObjectsPerSpawn = 3;

    [Header("Health")]
    [SerializeField] public int maxHealth = 3;
    [SerializeField] public List<GameObject> heartsUI;

    [Header("References")]
    [SerializeField] public GameObject[] prefabs;

    [Header("UI")]
    [SerializeField] public TextMeshProUGUI scoreText;
    [SerializeField] public TextMeshProUGUI topScoreText;
    [SerializeField] public GameObject restartButton;

    [Header("Audio")]
    [SerializeField] public AudioSource audioSource;

    private float currentForceSpeed;
    private float currentTorqueYspeed;
    private float currentTorqueXspeed;

    private int currentHealth;
    private int currentObjectPerSpawn;

    private Scene currentScene;
    private GameObject playerBlade;

    private int currentScore;

    private bool isPlaying = false;

    [Header("Difficulty Increase")]
    ///increase difficulty every x seconds
    public float difficultyTimer = 10f;
    private float difficultyCurrentTimer;

    private void Awake()
    {
        //clean up
        SceneManager.sceneLoaded -= OnSceneLoaded;

        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    private void Start()
    {
        //Initialize signleton instance

        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        //set currentHealth
        currentHealth = maxHealth;

        difficultyCurrentTimer = difficultyTimer;
        //start launching objects
        StartCoroutine(LaunchObject());

    }
    private void Update()
    {
        //increase difficulty over time
        if (isPlaying)
            IncreaseDifficultyOverTime();
        else
            ResetDifficulty();
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
            topScoreText.gameObject.SetActive(true);
            playerBlade.SetActive(false);
            Time.timeScale = 0;

            if (PlayerPrefs.GetInt("TopScore") < currentScore)
                PlayerPrefs.SetInt("TopScore", currentScore);
            else if (PlayerPrefs.GetInt("TopScore") == 0)
                PlayerPrefs.SetInt("TopScore", currentScore);

            topScoreText.text = $"Top Score: {PlayerPrefs.GetInt("TopScore")}";

            currentScore = 0;
        }
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        //set currentScene
        currentScene = SceneManager.GetActiveScene();

        //check which scene is loaded
        if (currentScene.name == "Main_Menu")
        {
            isPlaying = false;
            return;
        }
        else
            isPlaying = true;

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

        //assign score text
        if (GameObject.FindGameObjectWithTag("ScoreUI").TryGetComponent(out TextMeshProUGUI scoreTextTMPro))
            scoreText = scoreTextTMPro;

        //assign top score text
        if (GameObject.FindGameObjectWithTag("TopScoreUI").TryGetComponent(out TextMeshProUGUI topScoreTextTMPro))
        {
            topScoreText = topScoreTextTMPro;
            topScoreText.gameObject.SetActive(false);
        }
    }
    public void RestartSpawning()
    {
        foreach (Transform child in gameObject.transform)
        {
            if (child.TryGetComponent(out AudioSource audioSource)) continue;
            Destroy(child.gameObject);
        }

        maxToSpawnObjects = 999;

        currentHealth = maxHealth;

        Time.timeScale = 1f;

    }
    public void UpdateScore(int itemScore)
    {
        if (scoreText)
        {
            currentScore += itemScore;
            scoreText.text = $"Score: {currentScore}";
        }
    }
    public void MultiplyScore(float itemScore, float multiplier)
    {
        currentScore += (int)(itemScore * multiplier);
    }
    public void PlayAudioSourceOneShot(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }
    public void IncreaseDifficultyOverTime()
    {
        difficultyCurrentTimer -= Time.deltaTime;

        if (difficultyCurrentTimer <= 0)
        {
            minObjectsPerSpawn++;
            maxObjectsPerSpawn++;
            spawnSpeed -= 0.2f;

            difficultyCurrentTimer = difficultyTimer;
        }
    }
    private void ResetDifficulty()
    {
        //a bit hard coded at the moment but whatever, to fix later
        minObjectsPerSpawn = 1;
        maxObjectsPerSpawn = 3;
        spawnSpeed = 2f;
    }
}
