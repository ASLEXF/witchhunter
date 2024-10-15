using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class JsonDeserializer
{
    public void ProcessJson(string filePath, string name, Type type, Action<object> action)
    {
        try
        {
            string json = File.ReadAllText(filePath);
            var configs = JsonConvert.DeserializeObject(json, type);

            var listType = typeof(List<>).MakeGenericType(type);
            var config = (IList)Activator.CreateInstance(listType);

            //Convert.ChangeType(configs, type);

            //configs.TryGetValue(name, out config);

            action(config);
        }
        catch (JsonException ex)
        {
            Console.WriteLine("JSON Exception: " + ex.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Exception: " + ex.Message);
        }
    }
}
