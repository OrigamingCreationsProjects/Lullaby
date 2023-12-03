using System;
using System.Linq;
using Lullaby.Entities.Enemies;
using Lullaby.Entities.Enemies.States;
using UnityEngine;
using TMPro;

public class CurrentState : MonoBehaviour
{
    private TextMeshPro stateText;

    private BossEnemy entityRef;
    // Start is called before the first frame update
    void Start()
    {
        stateText = GetComponent<TextMeshPro>();
        entityRef = GetComponentInParent<BossEnemy>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!entityRef) {Debug.LogWarning("BossEnemy script couldn't be found"); return;}
        try
        {
            var state = entityRef.states.current;
            var text = "";
            if (state is BEAttackingState) text = "ATTACKING STATE";
            else if (state is CirculatingState) text = "CIRCULATING STATE";
            else if (state is BEIdleState) text = "IDLE STATE";
            stateText.text = text;
        }
        catch (Exception e)
        {
            Debug.LogWarning("Possibly states.current has not been set yet. Try again.");
            throw;
        }
        
    }
}
