using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour {

    public Transform player;

    public Vector3 offset;

    // Use this for initialization
    void Start () {
    }
	
	// Update is called once per frame
	void Update () {
        // We only trigger on Z-axis
        transform.position = new Vector3(transform.position.x, transform.position.y, player.transform.position.z + offset.z);
	}
}
