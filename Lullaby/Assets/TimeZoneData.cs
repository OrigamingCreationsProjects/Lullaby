using System;
using System.Collections;
using System.Collections.Generic;
using Lullaby;
using UnityEngine;

public class TimeZoneData : MonoBehaviour
{
    public int zoneData = 0;
    public float startPuzzleTime = 0.0f;
    public ZoneType zoneIndex = ZoneType.StartZone;
    public Levels level = Levels.First_Level;
    public string currentLevel = "First_Level";
    
    public void Start()
    {
        switch (level)
        {
            case Levels.First_Level:
                currentLevel = "First_Level";
                break;
            case Levels.Second_Level:
                currentLevel = "Second_Level";
                break;
            case Levels.Boss_Level:
                currentLevel = "Boss_Level";
                break;
        }
    }

    public void SetStartPuzzleZone()
    {
        if (zoneData == 0)
        {
            GamePlayerData.instance.startFirstPuzzleTime = Time.time;
            gameObject.SetActive(false);
        }
        else if(zoneData == 1)
        {
            GamePlayerData.instance.startSecondPuzzleTime = Time.time;
            gameObject.SetActive(false);
        }

        if (Levels.Boss_Level == level)
        {
            GamePlayerData.instance.startBossFightTime = Time.time;
            gameObject.SetActive(false);
        }
    }

    public void SetFinishPuzzleZone()
    {
        if (zoneData == 0)
        {
            GamePlayerData.instance.firstPuzzleTime = Time.time - GamePlayerData.instance.startFirstPuzzleTime;
            FindObjectOfType<DatabaseManager>().SendRequest(currentLevel, GamePlayerData.instance.firstPuzzleTime);
            gameObject.SetActive(false);
            //Debug.Log("Tiempo de puzzle " + zoneData + ": " + (GamePlayerData.instance.firstPuzzleTime) + " segundos");
        }
        else if (zoneData == 1)
        {
            GamePlayerData.instance.secondPuzzleTime = Time.time - GamePlayerData.instance.startSecondPuzzleTime;
            FindObjectOfType<DatabaseManager>().SendRequest(currentLevel, GamePlayerData.instance.secondPuzzleTime);
            gameObject.SetActive(false);
            //Debug.Log("Tiempo de puzzle " + zoneData + ": " + (GamePlayerData.instance.secondPuzzleTime) + " segundos");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(GameTags.Player))
        {
            if (zoneIndex == ZoneType.StartZone)
            {
                SetStartPuzzleZone();
            }
            else if (zoneIndex == ZoneType.FinishZone)
            {
                SetFinishPuzzleZone();
            }
            else if (zoneIndex == ZoneType.SendZone)
            {
                SendData();
            }
        }
    }

    public void SendData()
    {
        //FindObjectOfType<DatabaseManager>().SendRequest();
    }
    public enum ZoneType
    {
        StartZone,
        FinishZone,
        SendZone
    }
    public enum Levels
    {
        First_Level,
        Second_Level,
        Boss_Level
    }
}
