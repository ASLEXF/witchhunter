using System;
using System.Collections.Generic;

// ·ÏÆú

namespace SceneManagement
{
    [Serializable]
    public class SceneGroup
    {
        public string GrouppName = "";
        public List<SceneData> Scenes;
    }

    [Serializable]
    public class SceneData
    {
        //public SceneReference Reference;
        //public string Name => Reference.Name;
        public SceneType SceneType;
    }

    public enum SceneType { ActiveScene, MainMenu, UserInterface, HUD, Cinematic, Environment, Tooling }
}
