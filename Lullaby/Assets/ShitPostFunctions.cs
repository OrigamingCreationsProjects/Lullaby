using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class ShitPostFunctions : MonoBehaviour
{
    public CinemachineClearShot[] cameras;
    public AudioSource audioSource;
    private int cameraIndex = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            ChangeCamera();
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            audioSource.Play();
        }
    }
    
    public void ChangeCamera()
    {
        foreach (var camera in cameras)
        {
            camera.Priority = 0;
        }
        cameras[cameraIndex%cameras.Length].Priority = 1000;
        cameraIndex++;
    }
}
