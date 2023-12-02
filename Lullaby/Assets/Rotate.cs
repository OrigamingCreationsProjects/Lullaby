using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    public float yAngle = 90;

    public float initialXRotation = 14f;
    // Start is called before the first frame update
    void Start()
    {
        transform.localEulerAngles += new Vector3(initialXRotation, 0,0);
    }

    // Update is called once per frame
    void Update()
    {
        transform.localEulerAngles += new Vector3(0,yAngle,0) *Time.deltaTime;
    }
}
