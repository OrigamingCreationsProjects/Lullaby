using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using DG.Tweening;
using Lullaby;
using Lullaby.Entities;
using Lullaby.Entities.States;
using UnityEngine;

public class PlayerMoonLauncher : MonoBehaviour
{
    
    public AnimationCurve pathCurve;
    [Range(0,50)]
    public float speed = 10f;
    float speedModifier = 1;

    [Space]
    [Header("Booleans")]
    public bool insideLaunchStar;
    public bool flying;
    public bool almostFinished;

    private Transform launchObject;

    [Space]
    [Header("Public References")]
    public CinemachineDollyCart dollyCart;
    float cameraRotation;
    public Transform playerParent;

    [Space]
    [Header("Launch Preparation Sequence")]
    public float prepMoveDuration = .15f;
    public float launchInterval = .5f;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (insideLaunchStar)
            if (Input.GetKeyDown(KeyCode.B))
            {
                GetComponent<Player>().states.Change<MoonFlyPlayerState>();
                StartCoroutine(CenterLaunch());
            }


        if (flying)
        {
            //animator.SetFloat("Path", dollyCart.m_Position);
            playerParent.transform.position = dollyCart.transform.position;
            if (!almostFinished)
            {
                playerParent.transform.rotation = dollyCart.transform.rotation;
            }
        }

        if(dollyCart.m_Position > .7f && !almostFinished && flying)
        {
            almostFinished = true;
            //thirdPersonCamera.m_XAxis.Value = cameraRotation;

            playerParent.DORotate(new Vector3(360 + 180, 0, 0), .5f, RotateMode.LocalAxisAdd).SetEase(Ease.Linear)
                .OnComplete(() => playerParent.DORotate(
                    new Vector3(-90, playerParent.eulerAngles.y, playerParent.eulerAngles.z), .2f));
           
        }
    }
    IEnumerator CenterLaunch()
    {
        // movement.enabled = false;
        //transform.parent = null;
        DOTween.KillAll();

        //Checks to see if there is a Camera Trigger at the DollyTrack object - if there is activate its camera
        if (launchObject.GetComponent<CameraTrigger>() != null)
            launchObject.GetComponent<CameraTrigger>().SetCamera();
        //
        // //Checks to see if there is a Camera Trigger at the DollyTrack object - if there is activate its camera
        // if (launchObject.GetComponent<SpeedModifier>() != null)
        //     speedModifier = launchObject.GetComponent<SpeedModifier>().modifier;
        //
        // //Checks to see if there is a Star Animation at the DollyTrack object
        // if (launchObject.GetComponentInChildren<StarAnimation>() != null)
        //     starAnimation = launchObject.GetComponentInChildren<StarAnimation>();

        dollyCart.m_Position = 0;
        dollyCart.m_Path = null;
        dollyCart.m_Path = launchObject.GetComponent<CinemachineSmoothPath>();
        dollyCart.enabled = true;

        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();

        Sequence CenterLaunch = DOTween.Sequence();
        CenterLaunch.Append(transform.DOMove(dollyCart.transform.position, .2f));
        CenterLaunch.Join(transform.DORotate(dollyCart.transform.eulerAngles + new Vector3(90, 0, 0), .2f));
        //CenterLaunch.Join(starAnimation.Reset(.2f));
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
        //s.Join(starAnimation.PullStar(prepMoveDuration));
        s.AppendInterval(launchInterval);                                                                            // Wait for a while before the launch
        //s.AppendCallback(() => trail.emitting = true);
        //s.AppendCallback(() => followParticles.Play());
        Debug.Log("Vamos a empezar la traslacion");
        s.Append(DOVirtual.Float(dollyCart.m_Position, 1, finalSpeed, PathSpeed).SetEase(pathCurve));                // Lerp the value of the Dolly Cart position from 0 to 1
        //s.Join(starAnimation.PunchStar(.5f));
        s.Join(transform.DOLocalMove(new Vector3(0,0,-.5f), .5f));                                                   // Return player's Y position
        s.Join(transform.DOLocalRotate(new Vector3(0, 360, 0),                                                       // Slow rotation for when player is flying
            (finalSpeed/1.3f), RotateMode.LocalAxisAdd)).SetEase(Ease.InOutSine); 
        s.AppendCallback(() => Land());                                                                              // Call Land Function

        return s;
    }
     void Land()
     {
         playerParent.DOComplete();
         dollyCart.enabled = false;
         dollyCart.m_Position = 0;
         //movement.enabled = true;
         transform.parent = null;

         flying = false;
         almostFinished = false;
         // animator.SetBool("flying", false);
         //
         // followParticles.Stop();
         // trail.emitting = false;
         GetComponent<Player>().states.Change<FallPlayerState>();
     }
     public void PathSpeed(float x)
     {
         dollyCart.m_Position = x;
     }
     
     private void OnTriggerEnter(Collider other)
     {
         if (other.CompareTag(GameTags.MoonLauncher))
         {
             insideLaunchStar = true;
             launchObject = other.transform;
         }

         if (other.CompareTag(GameTags.MoonCameraTrigger))
             other.GetComponent<CameraTrigger>().SetCamera();
         
         
     }

     private void OnTriggerExit(Collider other)
     {
         if (other.CompareTag(GameTags.MoonLauncher))
         {
             insideLaunchStar = false;
         }
     }
}
