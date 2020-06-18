using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartController : MonoBehaviour
{
  public Transform TargetPoint;
  public bool isMoving = false;

  float speed = 3.5f;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()

    {
      var step = speed * Time.deltaTime;
        if(isMoving);
        {
          transform.position =  Vector3.MoveTowards (transform.position, TargetPoint.position, step);
        }
    }
}
