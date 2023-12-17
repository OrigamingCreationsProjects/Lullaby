using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class DatabaseManager : MonoBehaviour
{
    string username;
    string password;
    string uri;
    string contentType = "application/json";

    private void Awake()
    {
        LoadCredentials();
    }
    void Start()
    {
        //StartCoroutine(SendPostRequest());
    }
    
    public void SendRequest(string currentTable, float timeToSend)
    {
        StartCoroutine(SendPostRequest(currentTable, timeToSend));
    }
    
    string CreateJSON(string tabla, string date, int age, string gender, string dataTime)
    {
        string json = "First_Level";
        switch (tabla)
        {
            case "First_Level":
                json = CreateLevel1JSON(tabla, date, age, gender, dataTime);
                break;
            case "Second_Level":
                json = CreateLevel2JSON(tabla, date, age, gender, dataTime);
                break;
            case "Boss_Level":
                json = CreateBossLevelJSON(tabla, date, age, gender, dataTime);
                break;
        }
        //Construye JSON para la petición REST         
        // string json = $@"{{
        //     ""username"":""{username}"",
        //     ""password"":""{password}"",
        //     ""table"":""{tabla}"",
        //     ""data"": {{
        //         ""Date"": ""{date}"",
        //         ""Age"": ""{age}"",
        //         ""Gender"": ""{gender}"",
        //         ""FirstPuzzleTime"": ""{firstPuzzleTime}"",
        //         ""SecondPuzzleTime"": ""{secondPuzzleTime}""
        //     }}
        // }}";

        return json;
    }

    
    string CreateLevel1JSON(string tabla, string date, int age, string gender, string firstPuzzleTime)
    {
        //Construye JSON para la petición REST         
        string json = $@"{{
            ""username"":""{username}"",
            ""password"":""{password}"",
            ""table"":""{tabla}"",
            ""data"": {{
                ""Date"": ""{date}"",
                ""Age"": ""{age}"",
                ""Gender"": ""{gender}"",
                ""FirstPuzzleTime"": ""{firstPuzzleTime}""      
            }}
        }}";

        return json;
    }
    string CreateLevel2JSON(string tabla, string date, int age, string gender, string secondPuzzleTime)
    {
        //Construye JSON para la petición REST         
        string json = $@"{{
            ""username"":""{username}"",
            ""password"":""{password}"",
            ""table"":""{tabla}"",
            ""data"": {{
                ""Date"": ""{date}"",
                ""Age"": ""{age}"",
                ""Gender"": ""{gender}"",
                ""SecondPuzzleTime"": ""{secondPuzzleTime}""          
            }}
        }}";

        return json;
    }
    string CreateBossLevelJSON(string tabla, string date, int age, string gender, string bossFightTime)
    {
        //Construye JSON para la petición REST         
        string json = $@"{{
            ""username"":""{username}"",
            ""password"":""{password}"",
            ""table"":""{tabla}"",
            ""data"": {{
                ""Date"": ""{date}"",
                ""Age"": ""{age}"",
                ""Gender"": ""{gender}"",
                ""BossFightTime"": ""{bossFightTime}""           
            }}
        }}";

        return json;
    }
    
    IEnumerator SendPostRequest(string currentTable, float timeToSend)
    {
        string data = CreateJSON(currentTable, GamePlayerData.instance.date, 
            GamePlayerData.instance.age, GamePlayerData.instance.gender, 
            timeToSend.ToString());

        using (UnityWebRequest www = UnityWebRequest.Post(uri, data, contentType))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                print("Error: " + www.error);
            }
            else
            {
                print("Respuesta: " + www.downloadHandler.text);
            }
        }
    }

    void LoadCredentials()
    {
        //string configPath = "Assets/DataBase/configBuild.json";
        TextAsset configAsset = Resources.Load<TextAsset>("DataBase/configBuild");
        if(Application.isEditor)
        {
            //configPath = "Assets/DataBase/config.json";
            configAsset = Resources.Load<TextAsset>("DataBase/config");
        }
        if (configAsset != null)
        {
            //string configJson = File.ReadAllText(configPath);
            var config = JsonUtility.FromJson<Credentials>(configAsset.text);

            username = config.username;
            password = config.password;
            uri = config.uri;
          
        }
        else
        {
            Debug.LogError("Config file not found!");
        }
    }

    [System.Serializable]
    private class Credentials
    {
        public string username;
        public string password;
        public string uri;
    }
}
