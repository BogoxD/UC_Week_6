using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlicedObject : MonoBehaviour
{
    public GameObject wholeObject;
    public GameObject slicedObject;
    public ParticleSystem particles;
    public AudioClip cuttingSound;
    public int itemScore;

    private Rigidbody objectRb;
    private Collider objectCollider;

    public bool sliced = false;
    private void Start()
    {
        Destroy(gameObject, 5f);
    }
    private void Slice(Vector3 direciton)
    {
        particles.Play();
        sliced = true;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Blade") && !sliced)
        {
            Slice(other.GetComponent<Blade>().Direction);

            if (gameObject.CompareTag("DamageItem"))
            {
                GameManager.instance.DecreaseHealth();
            }
            else
            {
                GameManager.instance.UpdateScore(itemScore);
            }

            //play cutting sound audio for each object type
            GameManager.instance.PlayAudioSourceOneShot(cuttingSound);
        }
    }
}
