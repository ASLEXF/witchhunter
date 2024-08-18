using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;

    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogWarning("game manager not found!");
            }
            return _instance;
        }
    }

    public bool isInitiated = false;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        StartCoroutine(GameInitialize());
    }

    IEnumerator GameInitialize()
    {
        PlayerController.Instance.HideThePlayer();
        DialogBox.Instance.Hide();

        StartCoroutine(SceneLoader.Instance.GameSceneInitiate());

        yield return null;

        isInitiated = true;
    }

    private void Update()
    {
        //// 检测鼠标左键点击
        //if (Input.GetMouseButtonDown(0))
        //{
        //    Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //    RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

        //    if (hit.collider != null)
        //    {
        //        // 输出被点击对象的名称到控制台
        //        Debug.Log("Clicked on: " + hit.transform.name);
        //    }
        //}
    }

    public void StartExplorationMode()
    {
        GameEvents.Instance.ExplorationModeStarted();
    }

    public void StartBattleMode()
    {
        GameEvents.Instance.BattleModeStarted();
    }

}
