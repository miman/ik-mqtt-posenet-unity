using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    [Header("Resource assets")]
    [Tooltip("The animation to display when an object is hit")]
    public GameObject explosionAnimation;
    [Tooltip("The sound player for sound effects")]
    public AudioSource audioSource;
    [Tooltip("The sound to play when an object is hit")]
    public AudioClip explosionClip;

    // Start is called before the first frame update
    void Start()
    {
        audioSource.clip = explosionClip;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Obstacle")
        {
            Debug.Log("Obstacle was hit");
            audioSource.Play();
            addExplosion(collision.gameObject);
            Destroy(collision.gameObject);
        } else
        {
//            Debug.Log("Hit an object that isn't an Obstacle");
        }
    }

    // Add an obstacle add the given position
    void addExplosion(GameObject thingExploding)
    {
        Instantiate(explosionAnimation, thingExploding.transform.position, thingExploding.transform.rotation);
        Debug.Log("Explosion added @ position: " + thingExploding.transform.position);
    }
}
