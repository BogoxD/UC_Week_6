using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blade : MonoBehaviour
{
    private Camera mainCam;
    private Collider bladeCollider;
    private bool slicing = false;
    public Vector3 Direction { get; private set; }
    public float minSliceVel = 0.01f;

    private void Start()
    {
        mainCam = Camera.main;
        bladeCollider = GetComponent<Collider>();
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
            StartSlicing();

        else if (Input.GetMouseButtonUp(0))
            StopSlicing();
        else if (slicing)
            ContinueSlicing();
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
}
