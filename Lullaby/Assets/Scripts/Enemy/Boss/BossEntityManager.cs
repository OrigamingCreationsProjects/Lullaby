using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

using Lullaby.Entities.Enemies;
using UnityEngine;
using Random = UnityEngine.Random;
using DG.Tweening;
using Lullaby;
using Lullaby.Entities;
using Lullaby.Entities.Enemies.States;
using Lullaby.Entities.Events;
using Unity.VisualScripting;
using Quaternion = UnityEngine.Quaternion;
using Sequence = DG.Tweening.Sequence;
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
    public float attackWaitTime = 3f;
    
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
    private Color[] bossColors = {Color.red, Color.blue, Color.green, Color.yellow};
    [SerializeField] 
    private List<Transform> bossSlots;
    
    private int indexAI = 0;

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
    private Vector3 bossStartPos;
    private BossEnemy mainBoss;
    private Coroutine AI_Loop_Coroutine;
    public Coroutine Divide_Coroutine;

    private float angleTop;
    private float angleBottom;
    private float angleLeft;
    private float angleRight;

    private float[] angles;
    private Vector2[] dirs = {Vector2.right, Vector2.up, Vector2.left, Vector2.down};
    
    private Player player;
    private float yAngle;
    private BossStages stage = BossStages.FirstStage;

    private bool invoked = false;
    
    // NOMBRES PROPIEDADES SHADER
    private string _customColorPropertyName = "_CustomizableColor";
    
    
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
                bossStartPos = boss.transform.position;
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
            var script = boss.GetComponent<BossEnemy>();
            var randomIdx = Random.Range(0, bossMaterials.Length);
            script.GetBody().GetComponent<SkinnedMeshRenderer>().material.SetColor(_customColorPropertyName,  bossColors[i%bossColors.Length]);
            script.enemyEvents.OnRetreat += AnEnemyIsRetreating;
            script.enemyEvents.OnAttack += AnEnemyIsAttacking;
            script.enemyEvents.OnPlayerSeen += PlayerIsDetected;
            bossBuffer.Add(script);
            boss.SetActive(false);
            //boss.GetComponent<BossEnemy>().enabled = false;
            
        }

      
    }
    public void InitializeBossSlots()
    {
        
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
        mainBoss.transform.position = bossStartPos;
        Divide(bossBuffer.Count-1);
    }

    private void InitializePlane()
    {
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
        
        InitializeStage();
        InitializeStep();
    }

    void Start()
    {
        InitializeMainBoss();
        InitializeBossBuffer();
        enemyShot = new List<bool>() {false,false,false,false};
        enemyIndexes = new List<int>();

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
        enemyRetreating = true;
        bossBuffer[idx].Disable();
        bossBuffer[idx].states.Change<BEIdleState>();
        if(BossEnemy.MainBoss != bossBuffer[idx]) 
            bossBuffer[idx].transform.position = BossEnemy.MainBoss.position;
        bossBuffer[idx].slot = bossSlots[bossBuffer.Count-1-idx];
        bossBuffer[idx].stage = mainBoss.stage;
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
                
                EnableBosses();
                enemyRetreating = false;
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
        for (int index = bossBuffer.Count-1; index >= 0; index--)
        {
            bossBuffer[index].step = true;
            bossBuffer[index].enabled = true;
            bossBuffer[index].rotateAnimComponent.enabled = true;
        }
        
        StartAI();
    }
    
    public void Retreat()
    {
        Sequence s = DOTween.Sequence();
        yAngle = 0f;
        enemyAttacking = false;
        enemyRetreating = true;
        s.AppendCallback(() => BossEnemy.MainBoss.transform.DOMove(retreatPos.position, retreatTime));
        s.AppendInterval(retreatTime);
        s.AppendCallback(() => Divide(bossBuffer.Count-1));
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
        indexAI++;
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
            if (bossBuffer[i].controller.enabled)
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
        
        for (int i = 0; i < bossBuffer.Count(); i++)
        {
            if (bossBuffer[i].gameObject.activeSelf && !enemyShot[i])
                enemyIndexes.Add(i);
        }

        if (AliveBossCount() == 0)
        {
            return null;
        }
        if (enemyIndexes.Count == 0)
        {
            for (int i = 0; i < enemyShot.Count; i++)
            {
                enemyShot[i] = false;
            }
            var idx = Random.Range(0, bossBuffer.Count);
            enemyShot[idx] = true;
            return bossBuffer[idx];
        }

        BossEnemy randomEnemy;
        int randomIndex = Random.Range(0, enemyIndexes.Count);
        randomEnemy = bossBuffer[enemyIndexes[randomIndex]];
        enemyShot[enemyIndexes[randomIndex]] = true;
        enemyIndexes.Clear();
        return randomEnemy;
    }


    #endregion

    private BossEnemy GetEnemy()
    {
        BossEnemy boss = null;
        for (int i = bossBuffer.Count - 1 ; i >= 0; i--)
        { 
            if (!enemyShot[i]  && bossBuffer[i].controller.enabled)
                {
                    boss = bossBuffer[i];
                    enemyShot[i] = true;
                    break;  
                }
        }

        if (boss == null && AliveBossCount() != 0)
        {
            enemyShot = new List<bool>() { false, false, false, false };
            if( bossBuffer[bossBuffer.Count - 1].enabled) boss = bossBuffer[bossBuffer.Count - 1];
        }
        return boss;

    }
    #region -- COROUTINES --
    
    IEnumerator AI_Loop(BossEnemy boss)
    {
        var index = indexAI;
        while (AliveBossCount() > 0)
        {
            Debug.Log($"AI LOOP NUMBER {index} IS RUNNING");
            while (enemyRetreating) yield return null;
            
            BossEnemy attackingEnemy;
            attackingEnemy = RandomEnemyExcludingOne();
            
            if (attackingEnemy == null)
                yield break;

            if (stage == BossStages.FirstStage)
            {
                while (!EnemyCloseEnough()) yield return null;
                while (enemyAttacking) yield return null;
                yield return new WaitForSeconds(Random.Range(mainBoss.stats.current.attackMinWaitingTime,
                    mainBoss.stats.current.attackMaxWaitingTime));
                while (attackingEnemy.states.current is not CirculatingState) yield return null;
                enemyAttacking = true;
                yield return new WaitForSeconds(attackWaitTime);
                attackingEnemy.enemyEvents.OnPreparedToAttack?.Invoke();
            }
            else
            {
                yield return new WaitForSeconds(Random.Range(mainBoss.stats.current.distAttackMinWaitingTime,
                    mainBoss.stats.current.distAttackMaxWaitingTime));
                Debug.Log($"ATTACKING ENEMY IS {attackingEnemy.name}");
                attackingEnemy.enemyEvents.OnPreparedToAttack?.Invoke();
                yield return new WaitForSeconds(attackWaitTime);
            }
        }
    }

   /* IEnumerator Divide()
    {
        for (int idx = bossBuffer.Count - 1; idx >= 0; idx--)
        {
            bossBuffer[idx].Disable();
            bossBuffer[idx].states.Change<BEIdleState>();
            if(BossEnemy.MainBoss != bossBuffer[idx]) 
                bossBuffer[idx].transform.position = BossEnemy.MainBoss.position;
            bossBuffer[idx].slot = bossSlots[bossBuffer.Count-1-idx];
            bossBuffer[idx].stage = mainBoss.stage;
            bossBuffer[idx].gameObject.SetActive(true);
            bossBuffer[idx].angleAssigned = angles[(angles.Length -1 -idx)%angles.Length];
            bossBuffer[idx].dirFacing = dirs[(dirs.Length -1 -idx) % dirs.Length];
            if (idx == bossBuffer.Count - 1) bossBuffer[idx].IsInvincible = false;
            bossBuffer[idx].transform.position = bossSlots[bossBuffer.Count - 1 - idx].position;
            /*Tweener tween = bossBuffer[idx].transform.DOMove(bossSlots[bossBuffer.Count-1-idx].position, spawnTime);
            tween.OnUpdate(delegate
            {
                if (mainBoss.stage == BossStages.FirstStage)
                {
                    if (Vector3.Distance(bossSlots[bossBuffer.Count - 1 - idx].position, bossBuffer[idx].position) >
                        inPositionThreshold)
                    {
                        tween.ChangeEndValue(bossSlots[bossBuffer.Count - 1 - idx].position,
                            spawnTime / spawnTimeMultiplier, true);
                    }
                }

            });
            yield return new WaitWhile(() =>
                    Vector3.Distance(bossSlots[bossBuffer.Count - 1 - idx].position, bossBuffer[idx].position) >
                        inPositionThreshold
                );*/
   /*    }

   enemyRetreating = false;
   EnableBosses();
   yAngle = rotationSpeed;
   yield return null;
}
*/
   
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
            if (!boss.InPlace)
            {
                return false;
            }
        }
    
        return true;
    }
   
    /// <summary>
    /// Flags a bool in case one of the enemy clones is attacking.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    private void AnEnemyIsAttacking(object sender, OnValueChange args)
    {
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
    {
        player = mainBoss.player;
        InitializePlane(); // Called in Start so the player controller has time to be assigned in Awake call.
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

    #region EVENT DRIVEN

    public void ResetBossesStates()
    {
        foreach (var boss in bossBuffer)
        {
            boss.states.Change<BEIdleState>();
        }
    }
    #endregion
    
}
