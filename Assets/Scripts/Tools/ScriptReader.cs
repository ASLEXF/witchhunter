using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem.Android;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Runtime.InteropServices.ComTypes;

[System.Serializable]
public struct Sentence
{
    public int type;  // transition, lugline, title & content, content only
    public string title;  // title that should be written in the dialog
    public string content;  // content that should be written in the dialog
    public string other;  // for transition, position, weather, etc

    public Sentence(int type = 0, string title = "", string content = "", string other = "")
    {
        this.type = type;
        this.title = title;
        this.content = content;
        this.other = other;
    }
}

public class ScriptReader : MonoBehaviour
{
    private static ScriptReader _instance;

    public static ScriptReader Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject singletonObject = new GameObject("ScriptReader");
                _instance = singletonObject.AddComponent<ScriptReader>();
            }
            return _instance;
        }
    }

    public List<Sentence> output = new List<Sentence>();

    [SerializeField] private TextAsset textAsset;

    //#region Debug

    //[CustomEditor(typeof(ScriptReader))]
    //public class QueueDisplayEditor : Editor
    //{
    //    public override void OnInspectorGUI()
    //    {
    //        base.OnInspectorGUI();

    //        ScriptReader exampleScript = (ScriptReader)target;
            
    //        for (int i = 0; i < exampleScript.output.Count; i++)
    //        {
    //            EditorGUILayout.LabelField("Item " + i + ":");
    //            EditorGUI.indentLevel++;

    //            EditorGUILayout.LabelField("type: " + exampleScript.output[i].type.ToString());
    //            EditorGUILayout.LabelField("title: " + exampleScript.output[i].title);
    //            EditorGUILayout.LabelField("content: " + exampleScript.output[i].content);
    //            EditorGUILayout.LabelField("other: " + exampleScript.output[i].other);

    //            EditorGUI.indentLevel--;
    //        }
    //    }
    //}

    //#endregion

    private readonly string[] transitions =
    {
        "CUT",
        "FADE IN",
        "FADE OUT",
        "FLASHBACK",
        "JUMP CUT",
        "SLOW MOTION",
        "SILHOUETTE",
        "ANIMATED TRANSITION",
        "MATCH CUT",
        "AUDIO TRANSITION",
    };  // only support first 4 methods
    private readonly string[] luglines =
    {
        "INT",
        "EXT",
    };

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    //private void Start()
    //{
    //    ReadScript(textAsset);
    //}

    public void LoadTextAsset(string fileName)
    {
        string file = "Assets/Addressables/StoryScripts/" + fileName + ".txt";
        Addressables.LoadAssetAsync<TextAsset>(file).Completed += handle => OnTextAssetLoaded(handle, fileName);
    }

    void OnTextAssetLoaded(AsyncOperationHandle<TextAsset> handle, string fileName)
    {
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            textAsset = handle.Result;
            ReadScript(textAsset);
            GameEvents.Instance.TextScriptUpdated();
        }
        else
        {
            Debug.LogError("Addressables TextAsset not found: " + fileName);
        }
    }

    private void ParagraphAnalyser(string paragraph)
    {
        string[] sentences = paragraph.Split('\n');

        Sentence temp = new Sentence();

        foreach (string sentence in sentences)
        {
            string trimmedSentence = sentence.Trim();

            if (trimmedSentence.Equals(trimmedSentence.ToUpper()))
            {
                // case 1: transition
                foreach (string s in transitions)
                {
                    if (trimmedSentence.Equals(s + ":"))
                    {
                        output.Add(new Sentence(1, "", "", s));
                        goto outerLoop;
                    }
                }

                // case 2: lugline
                foreach (string s in luglines)
                {
                    if (trimmedSentence.StartsWith(s))
                    {
                        output.Add(new Sentence(2, "", "", trimmedSentence));
                        goto outerLoop;
                    }
                }

                // case 3: title & content
                if (!trimmedSentence.Contains("."))
                {
                    temp.type = 3;
                    temp.title = trimmedSentence;
                    temp.content = "";
                    temp.other = "";
                    goto outerLoop;
                }
            }
            if (temp.type == 3)
            {
                temp.content = trimmedSentence;
                output.Add(temp);
                temp = new Sentence();
            }
            else
            {
                // case 4: content
                output.Add(new Sentence(4, "", trimmedSentence, ""));
            }
            outerLoop: { }
        }
    }

    public void ReadScript(TextAsset file)
    {
        if (file != null)
        {
            string fileContent = file.text;
            string[] paragraphs = fileContent.Split(new string[] { "\r\n\r\n", "\n\n" }, System.StringSplitOptions.RemoveEmptyEntries);

            Queue<Sentence> result = new Queue<Sentence>();

            foreach (string paragraph in paragraphs)
            {
                //Debug.Log("Paragraph:\n" + paragraph);
                ParagraphAnalyser(paragraph);
            }
        }
        else
        {
            Debug.LogError("Text file reference not set!");
        }
    }
}
