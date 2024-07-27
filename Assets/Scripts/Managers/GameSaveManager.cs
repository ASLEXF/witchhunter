//using System.Collections;
//using System.Collections.Generic;
//using System.IO;
//using UnityEngine;
//using System.Runtime.Serialization.Formatters.Binary;

//public class GameSaveManager : MonoBehaviour
//{
//    public string GameSaveDir = Application.dataPath + "/savedata";  // Application.persistentDataPath

//    public Inventory MyInventory;

//    public void SaveGame()
//    {
//        Debug.Log("Game Save Folder Path:" + GameSaveDir);

//        if (!Directory.Exists(GameSaveDir))
//        {
//            Directory.CreateDirectory(GameSaveDir);
//        }

//        BinaryFormatter bf = new BinaryFormatter();

//        FileStream file = File.Create(GameSaveDir + "/inventory.txt");

//        var json = JsonUtility.ToJson(MyInventory);

//        bf.Serialize(file, json);

//        file.Close();
//    }

//    public void LoadGame()
//    {
//        BinaryFormatter bf = new BinaryFormatter();

//        if(File.Exists(GameSaveDir + "/inventory.txt"))
//        {
//            try
//            {
//                FileStream file = File.OpenRead(GameSaveDir + "/inventory.txt");

//                JsonUtility.FromJsonOverwrite(bf.Deserialize(file), MyInventory);

//                file.Close();
//            }
//            catch(IOException e)
//            {
//                Debug.LogError("Error opening file: " + e.Message);
//            }
//        }
//        else
//        {
//            Debug.Log("save file not found!");
//        }
//    }
//}
