using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using DG.Tweening;
using Lullaby;
using Lullaby.Entities;
using Lullaby.Entities.States;
using Lullaby.UI;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerMoonLauncher : MonoBehaviour
{
    
    public AnimationCurve pathCurve;
    [Range(0,50)]
    public float speed = 10f;
    private float speedModifier = 1;

    [Space]
    [Header("Booleans")]
    public bool insideLaunchStar;
    public bool flying;
    public bool almostFinished;

    [HideInInspector] public Transform launchObject;
    
    [Space]
    [Header("Public References")]
    public CinemachineDollyCart moonPathCart;
    public Transform playerParent;
    public CinemachineImpulseSource impulseSource;
    [Space]
    [Header("Launch Preparation Sequence")]
    public float prepMoveDuration = .15f;
    public float launchInterval = .5f;
    
    [Header("Particles")]
    public ParticleSystem followParticles;
    
    private MoonAnimation _moonAnimation;

    private TrailRenderer _flyTrail;

    private Player _player; 
    
    // Start is called before the first frame update
    void Start()
    {
        moonPathCart = GameObject.FindGameObjectWithTag(GameTags.MoonPathCart).GetComponent<CinemachineDollyCart>();
        playerParent = GameObject.FindGameObjectWithTag(GameTags.MoonCartParent).transform;
        impulseSource = FindObjectOfType<CinemachineImpulseSource>();
        _flyTrail = moonPathCart.GetComponentInChildren<TrailRenderer>();
        _player = GetComponent<Player>();
    }
    
    public void StartCenterLaunch()
    {
        StartCoroutine(CenterLaunch());
    }
    
    IEnumerator CenterLaunch()
    {
        // movement.enabled = false;
        //transform.parent = null;
        DOTween.KillAll();
        
        //Checks to see if there is a Camera Trigger at the DollyTrack object - if there is activate its camera
        if (launchObject.GetComponent<CameraTrigger>() != null)
            launchObject.GetComponent<CameraTrigger>().SetCamera();
        
        if (launchObject.GetComponent<SpeedModifier>() != null)
            speedModifier = launchObject.GetComponent<SpeedModifier>().modifier;
        //Checks to see if there is a Star Animation at the DollyTrack object
        if (launchObject.GetComponentInChildren<MoonAnimation>() != null)
            _moonAnimation = launchObject.GetComponentInChildren<MoonAnimation>();

        moonPathCart.m_Position = 0;
        moonPathCart.m_Path = null;
        moonPathCart.m_Path = launchObject.GetComponent<CinemachineSmoothPath>();
        moonPathCart.enabled = true;

        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();

        Sequence CenterLaunch = DOTween.Sequence();
        CenterLaunch.Append(transform.DOMove(moonPathCart.transform.position, .2f));
        CenterLaunch.Join(transform.DORotate(moonPathCart.transform.eulerAngles + new Vector3(90, 0, 0), .2f));
        CenterLaunch.Join(_moonAnimation.Reset(.2f));
        CenterLaunch.OnComplete(() => LaunchSequence());
    }
    Sequence LaunchSequence()
    {
        float distance;
        CinemachineSmoothPath path = launchObject.GetComponent<CinemachineSmoothPath>();
        float finalSpeed = path.PathLength / (speed * speedModifier);

        playerParent.transform.position = launchObject.position;
        playerParent.transform.rotation = transform.rotation;

        //transform.position = launchObject.position;
        flying = true;
        // animator.SetBool("flying", true);

        Sequence s = DOTween.Sequence();

        s.AppendCallback(() => transform.parent = playerParent.transform);                                           // Attatch the player to the empty gameObject
        s.AppendCallback(() => transform.localPosition = new Vector3(0, 0, 0));                                           // Attatch the player to the empty gameObject
        Debug.Log("Vamos a empezar el movimiento");
        s.Append(transform.DOMove(transform.localPosition - transform.up, prepMoveDuration));                        // Pull player a little bit back
        s.Join(transform.DOLocalRotate(new Vector3(0, 360 * 2, 0), prepMoveDuration, RotateMode.LocalAxisAdd).SetEase(Ease.OutQuart));
        s.Join(_moonAnimation.PullMoon(prepMoveDuration));
        s.AppendInterval(launchInterval);                                                                            // Wait for a while before the launch
        s.AppendCallback(() => _flyTrail.emitting = true);
        s.AppendCallback(() => followParticles.Play());
        Debug.Log("Vamos a empezar la traslacion");
        s.Append(DOVirtual.Float(moonPathCart.m_Position, 1, finalSpeed, PathSpeed).SetEase(pathCurve));                // Lerp the value of the Dolly Cart position from 0 to 1
        s.Join(_moonAnimation.PunchMoon(.5f)); //QUIZA CAMBIAR POR VARIABLE
        s.Join(transform.DOLocalMove(new Vector3(0,0,-.5f), .5f));                                                   // Return player's Y position
        s.Join(transform.DOLocalRotate(new Vector3(0, 360, 0),                                                       // Slow rotation for when player is flying
            (finalSpeed/1.3f), RotateMode.LocalAxisAdd)).SetEase(Ease.InOutSine); 
        s.AppendCallback(() => Land());                                                                              // Call Land Function

        return s;
    }
     void Land()
     {
         playerParent.DOComplete();
         moonPathCart.enabled = false;
         moonPathCart.m_Position = 0;
         transform.parent = null;

         flying = false;
         almostFinished = false;
         impulseSource.GenerateImpulse();
         // animator.SetBool("flying", false);
         //
         followParticles.Stop();
         _flyTrail.emitting = false;
         GetComponent<Player>().states.Change<FallPlayerState>();
     }
     public void PathSpeed(float x)
     {
         moonPathCart.m_Position = x;
     }
     
     private void OnTriggerEnter(Collider other)
     {
         // if (other.CompareTag(GameTags.MoonLauncher))
         // {
         //     insideLaunchStar = true;
         //     launchObject = other.transform;
         // }

         if (other.CompareTag(GameTags.MoonLauncher))
         {
             Debug.Log("Moon launcher: " + other.name);
            _player.moonLauncher.launchObject = other.transform;
            _player.insideMoon = true;
            other.gameObject.GetComponentInChildren<ContextIndicator>().ShowContextIndicator();
         }

         if (other.CompareTag(GameTags.MoonCameraTrigger))
             other.GetComponent<CameraTrigger>().SetCamera();
     }

     private void OnTriggerExit(Collider other)
     {
         if (other.CompareTag(GameTags.MoonLauncher))
         {
             insideLaunchStar = false;
            _player.insideMoon = false;
            other.gameObject.GetComponentInChildren<ContextIndicator>().HideContextIndicator();
         }
     }
}
