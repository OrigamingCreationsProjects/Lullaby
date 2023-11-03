using System;
using System.Collections;
using System.Collections.Generic;
using Lullaby.Entities;
using UnityEngine;

public class LifeOrbPrototype : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Player player))
        {
            player.health.Increase(10);
            gameObject.SetActive(false);
        }
    }
}
