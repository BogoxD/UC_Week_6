using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlicedObject : MonoBehaviour
{
    public GameObject wholeObject;
    public GameObject slicedObject;
    public ParticleSystem particles;

    private Rigidbody objectRb;
    private Collider objectCollider;

    private bool sliced = false;
    private void Slice(Vector3 direciton)
    {
        particles.Play();
        sliced = true;
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Blade") && !sliced)
        {
            Debug.Log("Slice");
            Slice(other.GetComponent<Blade>().Direction);
        }
    }
}
