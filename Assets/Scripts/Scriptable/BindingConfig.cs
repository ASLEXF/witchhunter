using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BindingConfigEntry
{
    public string trackName;
    public string sceneName;
    public string gameObjectPath;
}

//[Serializable]
//public class BindingConfig
//{
//    public List<BindingEntry> bindings;
//}

//[Serializable]
//public class BindingConfigEntry
//{
//    public string key;
//    public BindingConfig value;
//}

//[System.Serializable]
//public class BindingConfigWrapper
//{
//    public Dictionary<string, List<BindingConfigEntry>> timelines;

//    public static BindingConfigWrapper FromJson(string json)
//    {
//        // Use JsonUtility to parse the top-level structure
//        BindingConfigWrapper wrapper = new BindingConfigWrapper();
//        wrapper.timelines = new Dictionary<string, List<BindingConfigEntry>>();

//        var jsonObject = JsonUtility.FromJson<DictionaryWrapper>(json);
//        foreach (var key in jsonObject.keys)
//        {
//            var timelineEntries = JsonUtility.FromJson<TimelineEntryWrapper>($"{{\"entries\": {jsonObject.values[key]}}}");
//            wrapper.timelines.Add(key, timelineEntries.entries);
//        }

//        return wrapper;
//    }

//    [System.Serializable]
//    private class DictionaryWrapper
//    {
//        public List<string> keys;
//        public List<string> values;
//    }

//    [System.Serializable]
//    private class TimelineEntryWrapper
//    {
//        public List<BindingConfigEntry> entries;
//    }
//}
