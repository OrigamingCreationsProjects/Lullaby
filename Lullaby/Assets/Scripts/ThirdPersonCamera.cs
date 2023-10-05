using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    [Header("Camera Settings")]
    //public Player player;
    public float maxDistance = 15f;
    public float initialAngle = 20f;
    public float heightOffset = 1f;
    [Header("Following Settings")]
    public float verticalUpDeadZone = 0.15f;
    public float verticalDownDeadZone = 0.15f;
    public float verticalAirUpDeadZone = 4f;
    public float verticalAirDownDeadZone = 0;
    public float maxVerticalSpeed = 10f;
    public float maxAirVerticalSpeed = 100f;
    public float upwardRotationSpeed = 90f;
    
    protected float m_cameraDistance;
    protected float m_cameraTargetYaw;
    protected float m_cameraTargetPitch;

    protected Vector3 m_cameraTargetPosition;
    protected Quaternion m_currentUpRotation;

    protected Camera m_camera;
    protected CinemachineVirtualCamera m_virtualCamera;
    protected Cinemachine3rdPersonFollow m_cameraBody;
    protected CinemachineBrain m_brain;
    
    protected Transform m_target;
    
    protected string k_targetName = "Player Follower Camera Target";
    
    
    [Header("References")]
    public Transform orientation;
    public Transform player;

    public Transform playerObj;

    public Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
