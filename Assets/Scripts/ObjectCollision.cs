using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectCollision : MonoBehaviour
{
    [Header("Resource assets")]
    [Tooltip("The animation to display when an object is hit")]
    public GameObject explosionAnimation;
    [Tooltip("The sound player for sound effects")]
    public AudioSource audioSource;
    [Tooltip("The sound to play when an object is hit")]
    public AudioClip explosionClip;

    [Header("Other objects")]
    [Tooltip("The Object spawner responsible for all these objects")]
    public ObjectSpawner objectSpawner;

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
        if (collision.collider.tag == "Player")
        {
            Debug.Log("Player was hit");
            audioSource.Play();
            addExplosion(collision.gameObject);
            objectSpawner.obstacleHit(gameObject);
        }
        else
        {
//            Debug.Log("Hit an object that isn't an Obstacle");
        }
    }

    // Add an obstacle add the given position
    void addExplosion(GameObject thingExploding)
    {
        if (explosionAnimation != null)
        {
            Instantiate(explosionAnimation, thingExploding.transform.position, thingExploding.transform.rotation);
            Debug.Log("Explosion added for '" + thingExploding.name + "' @ position: " + thingExploding.transform.position);
        }
    }
}
