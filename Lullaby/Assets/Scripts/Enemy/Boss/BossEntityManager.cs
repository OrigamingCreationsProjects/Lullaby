using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Lullaby.Entities.Enemies;
using UnityEngine;
using Random = UnityEngine.Random;
using DG.Tweening;
using Lullaby;
using Lullaby.Entities;
using Lullaby.Entities.Enemies.States;
using Lullaby.Entities.Events;
using UnityEngine.UIElements;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class BossEntityManager : MonoBehaviour
{
    /// <summary>
    /// Returns the total number of Boss instances in the scene to be generated. Includes the main instance of the Boss.
    /// </summary>
    [Header("Boss Manager Configuration")]
    [SerializeField, Range(0,12)] 
    private int numOfBosses = 3;
    [SerializeField, Range(0f, 360f), Tooltip("Defines the rotation speed of the object.")] 
    private float rotationSpeed;
    [SerializeField,Tooltip("Defines the time a boss takes to reach retreat position.")] 
    private float retreatTime = 3f;
    [SerializeField, Tooltip("Defines the retreat position, if left blank it will be the same as the starting position.")]
    private Transform retreatPos;

    [SerializeField, Tooltip("Defines the platform in which the final boss fight takes place")]
    public Transform fightPlatform;
    [SerializeField, Tooltip("Defines the time the manager waits when an enemy is about to attack.")]
    private float attackWaitTime = 3f;
    
    [Header("Spawn Configuration"), Space(2f)]
    
    [Tooltip("Defines how much time a boss division takes."), SerializeField, Range(0f,10f)] 
    private float spawnTime = 1.5f;
    [SerializeField, Range(1f, 10f), Tooltip("Number by which the spawnTime is divided when updating the position in the tweener.")] 
    private float spawnTimeMultiplier;
    [SerializeField, Range(0f,2f), Tooltip("Threshold in which the tweener decides if destination has been reached.")] 
    private float inPositionThreshold;
    
    
    [Header("Objects Assignation"), Space(2f)]
    [SerializeField] 
    private GameObject bossPrefab;

    [SerializeField, Tooltip("Main Boss should be already in scene so include only materials for copies.")] 
    private Material[] bossMaterials;
    [SerializeField] 
    private List<Transform> bossSlots;
    
   
   
    /// <summary>
    /// Flags to notify one enemy is attacking or retreating.
    /// </summary>
    public bool enemyAttacking { get; private set;}
    public bool enemyRetreating { get; private set; }
    /// <summary>
    /// Flag to notify the player has been seen;
    /// </summary>
    public bool playerSeen { get; private set; }
    /// <summary>
    /// Defines positions in a circle. Equal distance between them.
    /// </summary>
    private float[] CirclePos = new []{0, Mathf.PI/2, Mathf.PI, (3*Mathf.PI)/2,  60*Mathf.PI/180, 120*Mathf.PI/180, 240*Mathf.PI/180,300*Mathf.PI/180,30*Mathf.PI/180,150*Mathf.PI/180,210*Mathf.PI/180,330*Mathf.PI/180 };
   
    /// <summary>
    /// Returns a List containing all boss' instances.
    /// </summary>
    private List<BossEnemy> bossBuffer;
    private List<BossEnemy> deadBossBuffer;
    private List<bool> enemyShot;
    private List<int> enemyIndexes;
    private bool step;
    private Transform bossStartPos;
    private BossEnemy mainBoss;
    private Coroutine AI_Loop_Coroutine;

    private float angleTop;
    private float angleBottom;
    private float angleLeft;
    private float angleRight;

    private float[] angles;
    private Vector2[] dirs = {Vector2.right, Vector2.up, Vector2.left, Vector2.down};
    
    private Player player;
    private float yAngle;
    private BossStages stage;

    private bool invoked = false;
    
    #region -- INITIALIZERS --
    /// <summary>
    /// Instantiates the bosses instances defined by the corresponding parameter {numOfBosses}
    /// </summary>
    private void InitializeBossBuffer()
    {
        
        bossBuffer = new List<BossEnemy>();
        deadBossBuffer = new List<BossEnemy>();
        var bossCount = 0;
        bossCount = CountActiveBosses(bossCount);
        InstantiateBosses(bossCount);
        mainBoss.gameObject.SetActive(true);
        //InitializeBossSlots();

    }

    private int CountActiveBosses(int bossCount)
    {
        foreach (Transform boss in transform)
        {
            if (boss.TryGetComponent(out BossEnemy script))
            {
                bossStartPos = boss.transform;
                if(retreatPos == null) retreatPos = boss.transform;
                script.enemyEvents.OnPlayerSeen += PlayerIsDetected;
                script.enemyEvents.OnAttack += AnEnemyIsAttacking;
                script.enemyEvents.OnRetreat += AnEnemyIsRetreating;
                script.enemyEvents.OnSecondStageReached += OnSecondStage;
                script.enemyEvents.OnFinalStageReached += OnFinalStage;
                bossBuffer.Add(script);
                bossCount++; 
            }
        }

        return bossCount;
    }
    public void InstantiateBosses(int bossCount)
    {
        for (int i = bossCount; i < numOfBosses; i++)
        {
            GameObject boss;
            boss = Instantiate(bossPrefab, mainBoss.position, transform.rotation, transform);
            boss.name = $"Boss_Clone{i}";
            boss.GetComponent<BossEnemy>().GetBody().GetComponent<MeshRenderer>().material = bossMaterials[i%bossMaterials.Length] ;
            boss.GetComponent<BossEnemy>().enemyEvents.OnRetreat += AnEnemyIsRetreating;
            boss.GetComponent<BossEnemy>().enemyEvents.OnAttack += AnEnemyIsAttacking;
            boss.GetComponent<BossEnemy>().enemyEvents.OnPlayerSeen += PlayerIsDetected;
            bossBuffer.Add(boss.GetComponent<BossEnemy>());
            boss.SetActive(false);
            //boss.GetComponent<BossEnemy>().enabled = false;
            
        }

      
    }
    public void InitializeBossSlots()
    {
        player = mainBoss.player;
        var maxDistanceOffset = mainBoss.stats.current.FsMaxDistToPlayer;
        var minDistanceOffset = mainBoss.stats.current.FsMinDistToPlayer;
        var platformSize = fightPlatform.GetComponent<Renderer>().bounds.size;
        var platformBounds = Mathf.Min(platformSize.x, platformSize.z);
        var scaleFactor = Mathf.Clamp(maxDistanceOffset, minDistanceOffset, platformBounds/2);
        for (int i = 0; i < numOfBosses; i++)
        {
            /* //TRIANGLE FORMATION
            var minusValue = i % 2 == 0 ? 1 : -1;
            var offset = i % 2 == 0 ? 0 : -1;
            var xScale = minusValue * xOffset * (i - offset);
            var zScale =  zOffset * (i - offset);
            emptyObj.transform.position =  new Vector3(mainBoss.transform.position.x + zScale, mainBoss.transform.position.y, mainBoss.transform.position.z + xScale);
            */
            var emptyObj = new GameObject($"bossSlot_{i}");
            
            // CIRCLE FORMATION
            var circleCoord = new Vector2(Mathf.Cos(CirclePos[i]),Mathf.Sin(CirclePos[i]));
            circleCoord = circleCoord.normalized;
            circleCoord *= scaleFactor;
            emptyObj.transform.parent = transform;
            emptyObj.transform.localPosition = new Vector3(circleCoord.x,0f,circleCoord.y);
          
            bossSlots.Add(emptyObj.transform);
        }
        mainBoss.transform.position = bossStartPos.position;
        Divide(bossBuffer.Count-1);
    }

    private void InitializePlane()
    {
        var player = FindObjectOfType<Player>();
        RaycastHit raycastHit;
        if (Physics.Raycast(player.transform.position, Vector3.down, out raycastHit, player.controller.height + 2f) && fightPlatform == null)
        {
            fightPlatform = raycastHit.collider.transform;
        }

        var platformBounds = fightPlatform.GetComponent<Renderer>();
        var platformBottomRightCorner = new Vector2(platformBounds.bounds.max.x, platformBounds.bounds.min.z);
        var platformBottomLeftCorner = new Vector2(platformBounds.bounds.min.x, platformBounds.bounds.min.z);
        var platformTopLeftCorner = new Vector2(platformBounds.bounds.min.x, platformBounds.bounds.max.z);
        var platformTopRightCorner = new Vector2(platformBounds.bounds.max.x, platformBounds.bounds.max.z);
        
        var center = new Vector2(platformBounds.bounds.center.x, platformBounds.bounds.center.z);
        var centerToTL = (platformTopLeftCorner - center).normalized;
        var centerToTR = (platformTopRightCorner - center).normalized;
        var centerToBL = (platformBottomLeftCorner - center).normalized;
        var centerToBR = (platformBottomRightCorner - center).normalized;

        angleTop = Vector2.Angle(centerToTL, centerToTR);
        angleLeft = Vector2.Angle(centerToTL, centerToBL);
        angleRight = Vector2.Angle(centerToTR, centerToBR);
        angleBottom = Vector2.Angle(centerToBL, centerToBR);

        angles = new[] { angleRight, angleTop, angleLeft, angleBottom };

    }
    private void InitializeMainBoss() => mainBoss = BossEnemy.MainBoss;
    private void InitializeStep() => step = true;
    private void InitializeStage() => stage = BossStages.FirstStage;
    #endregion
    #region -- MONOBEHAVIOUR --

    void Awake()
    {
        InitializeMainBoss();
        InitializeBossBuffer();
        InitializeStage();
        InitializeStep();
    }

    void Start()
    {
        
        enemyShot = new List<bool>() {false,false,false,false};
    }

    void Update()
    {
        HandleMovement();
    }

    #endregion
    #region -- PARTICULAR FUNCTIONS --

    private void HandleMovement()
    {
        switch (stage)
        {
            case BossStages.FirstStage:
                if (player == null || enemyAttacking || enemyRetreating) return;
                transform.position = new Vector3(player.position.x, transform.position.y, player.position.z);
                transform.Rotate(new Vector3(0,yAngle,0) * Time.deltaTime);
                break;
            case BossStages.SecondStage:
                if (enemyAttacking) return;
                transform.Rotate(new Vector3(0,yAngle,0) * Time.deltaTime);
                break;
            case BossStages.FinalStage:
                break;
        }
    }
    public void UpdateSlotsPosition(float yOffset = 0f)
    {
        int index = 0;
        var maxDistanceOffset = mainBoss.stats.current.SsMaxDistToPlayer;
        var platformSize = fightPlatform.GetComponent<Renderer>().bounds.size;
        var platformBounds = Mathf.Max(platformSize.x, platformSize.z);
        var scaleFactor = Mathf.Clamp(maxDistanceOffset, (platformBounds/2) + (maxDistanceOffset/2), maxDistanceOffset*2);
        foreach (Transform slot in bossSlots)
        {
            var circleCoord = new Vector2(Mathf.Cos(CirclePos[index]),Mathf.Sin(CirclePos[index]));
            circleCoord = circleCoord.normalized;
            circleCoord *= scaleFactor;
            slot.transform.localPosition = new Vector3(circleCoord.x , transform.position.y + yOffset,circleCoord.y);
            index++;
        }
    }
    /// <summary>
    /// Animates the boss entity division. Called on OnPlayerDetected callback.
    /// </summary>
    /// <returns></returns>
    public void Divide(int idx)
    {
        mainBoss.Disable();
        bossBuffer[idx].Disable();
        if(BossEnemy.MainBoss != bossBuffer[idx]) 
            bossBuffer[idx].transform.position = BossEnemy.MainBoss.position;
        bossBuffer[idx].slot = bossSlots[bossBuffer.Count-1-idx];
        bossBuffer[idx].stage = mainBoss.stage;
        bossBuffer[idx].states.Change<BEIdleState>();
        bossBuffer[idx].gameObject.SetActive(true);
        bossBuffer[idx].angleAssigned = angles[(angles.Length -1 -idx)%angles.Length];
        bossBuffer[idx].dirFacing = dirs[(dirs.Length -1 -idx) % dirs.Length];
        if (idx == bossBuffer.Count - 1) bossBuffer[idx].IsInvincible = false;
        Tweener tween = bossBuffer[idx].transform.DOMove(bossSlots[bossBuffer.Count-1-idx].position, spawnTime);
        tween.OnUpdate(delegate
        {
            if (mainBoss.stage == BossStages.FirstStage)
            {
                if (Vector3.Distance(bossSlots[bossBuffer.Count-1-idx].position, bossBuffer[idx].position) > inPositionThreshold)
                {
                    tween.ChangeEndValue(bossSlots[bossBuffer.Count-1-idx].position, spawnTime/spawnTimeMultiplier,true);
                }
            }
           
        }).OnComplete(() =>
        {
            idx -= 1;
            if (idx > -1) Divide(idx);
            else
            {
                enemyRetreating = false;
                EnableBossesControllers();
                yAngle = rotationSpeed;
            }
        });
    }
    
    public void CheckInvincibilityStatus()
    {
        for (int index = 0; index < bossBuffer.Count; index++)
        {
            if (index == bossBuffer.Count - 1) bossBuffer[index].IsInvincible = false;
        }
    }

    public void EnableBosses()
    {
        for (int index = 0; index < bossBuffer.Count; index++)
        {
            bossBuffer[index].step = true;
            bossBuffer[index].enabled = true;
            bossBuffer[index].rotateAnimComponent.enabled = true;
            if (bossBuffer[index].stage == BossStages.FinalStage && !invoked)
            {
                bossBuffer[index].StartCoroutine(bossBuffer[index].EnemyMovement());
               
            }
                
            bossBuffer[index].states.Change<CirculatingState>();
        }

        if(BossEnemy.MainBoss.stage == BossStages.FinalStage) invoked = true;
    }

    private void EnableBossesControllers()
    {
        foreach (BossEnemy boss in bossBuffer)
        {
            boss.SetController(true);
            if (BossEnemy.MainBoss == boss)
            {
                mainBoss.enemyEvents.OnDivisonFinished?.Invoke();
                EnableBosses();
                StartAI();
            } 
        }
        
       
          
    }
    public void Retreat()
    {
        Sequence s = DOTween.Sequence();
        yAngle = 0f;
        enemyAttacking = false;
        enemyRetreating = true;
        StopCoroutine(AI_Loop_Coroutine);
        //mainBoss.transform.position = retreatPos.position;
        s.AppendCallback(() => mainBoss.SetController(false));
        s.AppendCallback(() => BossEnemy.MainBoss.transform.DOMove(retreatPos.position, retreatTime));
        s.AppendInterval(retreatTime);
        s.AppendCallback(() => Divide(bossBuffer.Count-1));
        //Divide(bossBuffer.Count-1);
    }
    
    public void DisableBossGameObjects()
    {
        for (int index = 1; index < bossBuffer.Count; index++)
        {
            bossBuffer[index].gameObject.SetActive(false);
        }
    }

     /// <summary>
    /// Counts the number of bosses currently alive
    /// </summary>
    /// <returns>Integer with number of bosses alive.</returns>
    public int AliveBossCount() {return bossBuffer.Count;}
    public void RemoveBossFromBuffer(BossEnemy item){bossBuffer.Remove(item); deadBossBuffer.Add(item);}
    public void ReviveBosses()
    {

        bossBuffer[bossBuffer.Count-1].IsInvincible = true;
        foreach(BossEnemy boss in deadBossBuffer)
        {
            boss.IsInvincible = true;
            boss.health.ResetHealth();
            bossBuffer.Add(boss);
       
        }

        deadBossBuffer = new List<BossEnemy>();
    }

    private void ResetRotation()
    {
        transform.rotation = Quaternion.identity;
        
    }
    public void StartAI()
    {
        AI_Loop_Coroutine = StartCoroutine(AI_Loop(null));
    }

   
    
    /// <summary>
    /// Calls a random enemy Boss to attack
    /// </summary>
    /// <returns>Selected random Boss script</returns>
    public BossEnemy RandomEnemy()
    {
        enemyIndexes = new List<int>();

        for (int i = 0; i < bossBuffer.Count(); i++)
        {
            if (bossBuffer[i].states.current is CirculatingState && bossBuffer[i].controller.enabled)
                enemyIndexes.Add(i);
        }

        if (enemyIndexes.Count == 0)
            return null;

        BossEnemy randomEnemy;
        int randomIndex = Random.Range(0, enemyIndexes.Count);
        randomEnemy = bossBuffer[enemyIndexes[randomIndex]];

        return randomEnemy;
    }
/// <summary>
/// Selects a Boss to attack excluding a boss who has already been set for attack
/// </summary>
/// <param name="exclude"> The boss to be excluded from the selection</param>
/// <returns>Selected Boss script</returns>
    public BossEnemy RandomEnemyExcludingOne()
    {
        enemyIndexes = new List<int>();

        for (int i = 0; i < bossBuffer.Count(); i++)
        {
            if (bossBuffer[i].states.current is CirculatingState && bossBuffer[i].controller.enabled && !enemyShot[i])
                enemyIndexes.Add(i);
        }

        if (enemyIndexes.Count == 0)
        {
            enemyShot = new List<bool>() { false, false, false, false };
            return null;
        }

        BossEnemy randomEnemy;
        int randomIndex = Random.Range(0, enemyIndexes.Count);
        randomEnemy = bossBuffer[enemyIndexes[randomIndex]];
        enemyShot[enemyIndexes[randomIndex]] = true;
        return randomEnemy;
    }


    #endregion

    private BossEnemy GetEnemy()
    {
        BossEnemy boss = null;
        for (int i = bossBuffer.Count - 1 ; i >= 0; i--)
        { 
            if (!enemyShot[i] && bossBuffer[i].states.current is CirculatingState && bossBuffer[i].controller.enabled)
            {
                boss = bossBuffer[i];
                enemyShot[i] = true;
                break;  
            }
        }

        if (boss == null && AliveBossCount() != 0)
        {
            enemyShot = new List<bool>() { false, false, false, false };
            if( bossBuffer[bossBuffer.Count - 1].states.current is CirculatingState && bossBuffer[bossBuffer.Count - 1].enabled) boss = bossBuffer[bossBuffer.Count - 1];
        }
        return boss;

    }
    #region -- COROUTINES --
    
    IEnumerator AI_Loop(BossEnemy boss)
    {
        if (AliveBossCount() == 0)
        {
            StopCoroutine(AI_Loop(null));
            yield break;
        }
        yield return new WaitUntil(() => EnemyCloseEnough());
        if(BossEnemy.MainBoss.stage != BossStages.FinalStage) yield return new WaitWhile(()=> enemyAttacking);
        
        yield return new WaitWhile(() => enemyRetreating);
        if(BossEnemy.MainBoss.stage != BossStages.FirstStage) yield return new WaitForSeconds(Random.Range(BossEnemy.MainBoss.stats.current.distAttackMinWaitingTime,BossEnemy.MainBoss.stats.current.distAttackMaxWaitingTime));
        else yield return new WaitForSeconds(Random.Range(BossEnemy.MainBoss.stats.current.attackMinWaitingTime,BossEnemy.MainBoss.stats.current.attackMaxWaitingTime));
        BossEnemy attackingEnemy;
        if (BossEnemy.MainBoss.stage != BossStages.FinalStage)
        {
            attackingEnemy = RandomEnemyExcludingOne();
        
            if (attackingEnemy == null)
                attackingEnemy = RandomEnemy();

            if (attackingEnemy == null && AliveBossCount() == 1)
                attackingEnemy = BossEnemy.MainBoss;
        
            if (attackingEnemy == null)
                yield break;
            
            yield return new WaitUntil(()=> attackingEnemy.states.current is CirculatingState);
            enemyAttacking = true;
            yield return new WaitForSeconds(attackWaitTime);
            attackingEnemy.enemyEvents.OnPreparedToAttack?.Invoke();
            if (AliveBossCount() > 0)
                AI_Loop_Coroutine = StartCoroutine(AI_Loop(attackingEnemy));
        }
        else
        {
            attackingEnemy = GetEnemy();
            if (attackingEnemy == null && AliveBossCount() == 1)
                attackingEnemy = BossEnemy.MainBoss;
            if (attackingEnemy == null)
                yield break;
            attackingEnemy.enemyEvents.OnPreparedToAttack?.Invoke();
            yield return new WaitForSeconds(attackWaitTime);
            if (AliveBossCount() > 0)
                AI_Loop_Coroutine = StartCoroutine(AI_Loop(null));
        }
        
       
        
    }

   
    #endregion

    #region AI_CHECKERS & EVENT HANDLERS
    /// <summary>
    /// Returns a bool in case one enemy is close enough to attack the player.
    /// </summary>
    /// <returns>bool</returns>
    private bool EnemyCloseEnough()
    {
        foreach (var boss in bossBuffer)
        {
            // Computes the distance to the player from each boss. Takes only into account the distance in the
            // circunference around the player. Not the distance in component y.
            var playerPos = player.position;
            playerPos.y = boss.position.y;
            var distFromPlayer = (playerPos - boss.position).magnitude;
            // Checks the distance to the player is enough to attack it. This check only happens in the First Stage.
            if (distFromPlayer <= boss.stats.current.FsMinDistToPlayer || boss.stage != BossStages.FirstStage)
            {
                return true;
            }
        }
    
        return false;
    }
   
    /// <summary>
    /// Flags a bool in case one of the enemy clones is attacking.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    private void AnEnemyIsAttacking(object sender, OnValueChange args)
    {
        Debug.Log("AN ENEMY IS ATTACKING");
        enemyAttacking = args.value;
    }
    /// <summary>
    /// Flags a bool in case one of the enemy clones is retreating.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    private void AnEnemyIsRetreating(object sender, OnValueChange args)
    { 
        enemyRetreating = args.value;
    }

    private void PlayerIsDetected(object sender, OnValueChange args)
    { InitializePlane(); // Called in Start so the player controller has time to be assigned in Awake call.
       InitializeBossSlots();
    }

    private void OnSecondStage(object sender, EventArgs args)
    {
        stage = BossStages.SecondStage;
        transform.position = fightPlatform.position;
        ResetRotation();
        UpdateSlotsPosition(6f);
        Retreat();
    }

    private void OnFinalStage(object sender, EventArgs args)
    {
        stage = BossStages.FinalStage;
        ResetRotation();
        ReviveBosses();
        UpdateSlotsPosition();
        Retreat();
    }
    #endregion
    
    
}
