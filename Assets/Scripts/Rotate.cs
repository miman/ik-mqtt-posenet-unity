using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Miman
{
    public class Rotate : MonoBehaviour
    {
        public float speed = 10;
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            transform.Rotate(Vector3.up * speed * Time.deltaTime);
        }
    }
}
