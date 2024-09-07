using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Blade : MonoBehaviour
{
    private Camera mainCam;
    private Collider bladeCollider;
    private bool slicing = false;
    private bool once = true;
    public Vector3 Direction { get; private set; }
    public float minSliceVel = 0.01f;
    public float targetTime = 0.5f;
    public float StreakMultiplier = 2;
    public TextMeshProUGUI streakText;

    private float currentTime;
    private List<GameObject> currentObjectsStreak;

    private void Start()
    {
        mainCam = Camera.main;
        bladeCollider = GetComponent<Collider>();

        currentObjectsStreak = new List<GameObject>();

        currentTime = targetTime;
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
            StartSlicing();

        else if (Input.GetMouseButtonUp(0))
            StopSlicing();
        else if (slicing)
            ContinueSlicing();

        StreakUI();
    }
    private void OnEnable()
    {
        StopSlicing();
    }
    private void OnDisable()
    {
        StopSlicing();
    }
    private void StartSlicing()
    {
        Vector3 newPosition = mainCam.ScreenToWorldPoint(Input.mousePosition);
        newPosition.z = 0;

        transform.position = newPosition;

        slicing = true;
        bladeCollider.enabled = true;
    }
    private void StopSlicing()
    {
        slicing = false;
        if (bladeCollider)
            bladeCollider.enabled = false;
    }
    private void ContinueSlicing()
    {
        Vector3 newPosition = mainCam.ScreenToWorldPoint(Input.mousePosition);
        newPosition.z = 0;

        Direction = newPosition - transform.position;
        float velocity = Direction.magnitude / Time.deltaTime;

        bladeCollider.enabled = velocity > minSliceVel;

        transform.position = newPosition;
    }
    private void StreakUI()
    {
        currentTime -= Time.deltaTime;

        if (currentTime <= 0f)
        {
            currentObjectsStreak.Clear();
            currentTime = targetTime;

            once = true;
            Invoke(nameof(DisableStreakText), targetTime + 3);
        }

        //Show streak text when performing a streak and multiply score
        if (currentObjectsStreak.Count >= 2)
        {
            streakText.gameObject.SetActive(true);
            
            //update position only once
            if (once)
            {
                streakText.transform.position = transform.position;
                once = false;
                //multiply score based on each item score
                for (int i = 0; i < currentObjectsStreak.Count; i++)
                    GameManager.instance.MultiplyScore(currentObjectsStreak[i].GetComponent<SlicedObject>().itemScore, StreakMultiplier);
            }
            //update text
            streakText.text = $"Streak x {currentObjectsStreak.Count}";
        }
    }
    private void DisableStreakText()
    {
        streakText.gameObject.SetActive(false);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ScoreItem") && !other.GetComponent<SlicedObject>().sliced)
        {
            currentObjectsStreak.Add(other.gameObject);
        }
    }
}
