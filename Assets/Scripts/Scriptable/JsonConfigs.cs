using System;

[Serializable]
public class BindingConfigEntry
{
    public string trackName;
    public string sceneName;
    public string gameObjectPath;
}

[Serializable]
public class NPCInteractConfigEntry
{
    public int status;
    public bool isInteractable;
}

[Serializable]
public class NPCItemConfigEntry
{
    public int itemID;
    public int min;
    public int max;
}

[Serializable]
public class NPCTalkConfigEntry
{
    public int stage;
    public string fileName;
}
