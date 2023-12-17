using System;
using System.Collections;
using System.Collections.Generic;
using Lullaby;
using Lullaby.UI.Menus;
using TMPro;
using UnityEngine;

public class GamePlayerData : Singleton<GamePlayerData>
{
    public string date = "2010-07-11 22:58:00";
    public int age = 0;
    public string gender = "Masculino";
    public float firstPuzzleTime = 0.1f;
    public float secondPuzzleTime = 0.1f;
    public float bossFightTime = 0.1f;
    public float startFirstPuzzleTime = 0.0f;
    public float startSecondPuzzleTime = 0.0f;
    public float startBossFightTime = 0.0f;
    
    // Start is called before the first frame update
    void Start()
    {
        date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void UpdateAge(int newAge)
    {
        age = newAge;
    }

    public void UpdateGender(TextMeshProUGUI newGender)
    {
        gender = newGender.text;
    }
    
    public void UpdateBossTime()
    {
        bossFightTime = Time.time - startBossFightTime;
        FindObjectOfType<DatabaseManager>().SendRequest("Boss_Level", bossFightTime);
    }
    
}
