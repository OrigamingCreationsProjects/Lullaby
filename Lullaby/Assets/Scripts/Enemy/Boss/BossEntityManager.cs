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
using UnityEditor.Localization.Plugins.XLIFF.V20;
using UnityEngine.UIElements;

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
    private List<int> enemyIndexes;
    private Transform retreatPos;
    private Transform bossStartPos;
    private BossEnemy mainBoss;
    private Coroutine AI_Loop_Coroutine;
    private Player player;
    private float yAngle;
    
    
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
        mainBoss = bossBuffer.First().GetComponent<BossEnemy>();
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
                retreatPos = boss.transform;
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
            boss = Instantiate(bossPrefab, BossEnemy.MainBoss.position, transform.rotation, transform);
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
        for (int i = 0; i < numOfBosses; i++)
        {
            /* TRIANGLE FORMATION
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
            circleCoord *= mainBoss.stats.current.FsMaxDistToPlayer;
            emptyObj.transform.parent = transform;
            emptyObj.transform.localPosition = new Vector3(circleCoord.x, mainBoss.position.y,circleCoord.y);
          
            bossSlots.Add(emptyObj.transform);
        }
        mainBoss.transform.position = bossStartPos.position;
        Divide(bossBuffer.Count-1);
    }

    #endregion
    #region -- MONOBEHAVIOUR --

    void Awake()
    {
        InitializeBossBuffer();
    }

    void Start()
    {
    }

    void Update()
    {
        if (player != null)
        {
            
            transform.position = new Vector3(player.position.x, transform.position.y, player.position.z);
            if (enemyAttacking) return;
            if (enemyRetreating) return;
            transform.Rotate(new Vector3(0,yAngle,0) * Time.deltaTime);
        } 
    }

    #endregion
    #region -- PARTICULAR FUNCTIONS --

    public void UpdateSlotsPosition()
    {
        int index = 0;
        foreach (Transform slot in bossSlots)
        {
            var circleCoord = new Vector2(Mathf.Cos(CirclePos[index]),Mathf.Sin(CirclePos[index]));
            circleCoord = circleCoord.normalized;
            circleCoord *= mainBoss.stats.current.SsMaxDistToPlayer;
            
            
            slot.transform.position = new Vector3(circleCoord.x + transform.position.x, mainBoss.position.y,circleCoord.y + transform.position.z);
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
        if (idx == bossBuffer.Count - 1) bossBuffer[idx].IsInvincible = false;
        Tweener tween = bossBuffer[idx].transform.DOMove(bossSlots[bossBuffer.Count-1-idx].position, spawnTime);
        tween.OnUpdate(delegate
        {
            if (Vector3.Distance(bossSlots[bossBuffer.Count-1-idx].position, bossBuffer[idx].position) > inPositionThreshold)
            {
                tween.ChangeEndValue(bossSlots[bossBuffer.Count-1-idx].position, spawnTime/spawnTimeMultiplier,true);
            }
        }).OnComplete(() =>
        {
            idx -= 1;
            if (idx > -1) Divide(idx);
            else
            {
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
            bossBuffer[index].states.Change<CirculatingState>();
        }
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
            if (bossBuffer[i].states.current is CirculatingState && bossBuffer[i].gameObject.activeSelf && bossBuffer[i] != BossEnemy.MainBoss)
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
    public BossEnemy RandomEnemyExcludingOne(BossEnemy exclude)
    {
        enemyIndexes = new List<int>();

        for (int i = 0; i < bossBuffer.Count(); i++)
        {
            if (bossBuffer[i].states.current is CirculatingState && bossBuffer[i] != exclude && bossBuffer[i].gameObject.activeSelf && bossBuffer[i] != BossEnemy.MainBoss)
                enemyIndexes.Add(i);
        }

        if (enemyIndexes.Count == 0)
            return null;

        BossEnemy randomEnemy;
        int randomIndex = Random.Range(0, enemyIndexes.Count);
        randomEnemy = bossBuffer[enemyIndexes[randomIndex]];

        return randomEnemy;
    }


    #endregion

    private BossEnemy GetEnemy()
    {
        BossEnemy boss = null;
        foreach (var VARIABLE in bossBuffer) {
                        if (!VARIABLE.IsInvincible)
                        {
                            boss = VARIABLE;
                        }
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

        Debug.Log("AI LOOP RUNNING");
        
        yield return new WaitUntil(() => EnemyCloseEnough());
        Debug.Log("ENEMIES ARE CLOSE ENOUGH");
        yield return new WaitWhile(()=> enemyAttacking);
        Debug.Log("NO ENEMY IS ATTACKING");
        yield return new WaitWhile(() => enemyRetreating);
        Debug.Log("NO ENEMY IS RETREATING");
        if(BossEnemy.MainBoss.stage != BossStages.FirstStage) yield return new WaitForSeconds(Random.Range(BossEnemy.MainBoss.stats.current.distAttackMinWaitingTime,BossEnemy.MainBoss.stats.current.distAttackMaxWaitingTime));
        else yield return new WaitForSeconds(Random.Range(BossEnemy.MainBoss.stats.current.attackMinWaitingTime,BossEnemy.MainBoss.stats.current.attackMaxWaitingTime));
        BossEnemy attackingEnemy = RandomEnemyExcludingOne(boss);

        if (attackingEnemy == null)
            attackingEnemy = RandomEnemy();

        if (attackingEnemy == null && AliveBossCount() == 1)
            attackingEnemy = BossEnemy.MainBoss;
        
        if (attackingEnemy == null)
            yield break;
        attackingEnemy = GetEnemy();
        Debug.Log($"The attacking enemy is: {attackingEnemy}");
        
        yield return new WaitUntil(()=> attackingEnemy.states.current is CirculatingState);
      
        attackingEnemy.enemyEvents.OnPreparedToAttack?.Invoke();
        
        if (AliveBossCount() > 0)
            AI_Loop_Coroutine = StartCoroutine(AI_Loop(attackingEnemy));
        
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
    {
       InitializeBossSlots();
    }

    private void OnSecondStage(object sender, EventArgs args)
    {
        UpdateSlotsPosition();
        Retreat();
    }

    private void OnFinalStage(object sender, EventArgs args)
    {
        ReviveBosses();
        Retreat();
    }
    #endregion
    
    
}
