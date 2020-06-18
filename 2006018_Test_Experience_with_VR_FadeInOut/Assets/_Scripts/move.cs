using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class move : MonoBehaviour
{
    public GameObject box;
    Vector3 startPos = new Vector3(2, 1, 1);
    Vector3 startPos2 = new Vector3(-2, 3, 2);
    void Start()
    {
      for (int i =0; i<20000; i++ ) {
        Instantiate(box, randomPosition(), rotationBox(0,0,0 ));
      }

    }

    // Update is called once per frame
    void Update()
    {
        //transform.Translate(Vector3.forward * Time.deltaTime);
    }
    Vector3 randomPosition()
    {
      Vector3 v = new Vector3(0,0,0);
      v.x = Random.Range(-100, 100);
      v.y = Random.Range(-100, 100);
      v.z = Random.Range(-100, 100);
      return v;
    }
    private static Quaternion rotationBox(float x, float y, float z){ return new Quaternion(x,y,z,1);}
}
