using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public bool isInitiated = false;
    public int Stage = 1;

    async Task Start()
    {
        // load sprite assets
        await AssetPreloader.PreloadAllIcons();

        // load the first scene
        StartCoroutine(FirstSceneInitialize());
    }

    IEnumerator FirstSceneInitialize()
    {
        if (!DebugMode.IsDebugMode)
            PlayerController.Instance.HideThePlayer();
        DialogBox.Instance.Hide();

        StartCoroutine(SceneLoader.Instance.GameSceneInitiate());

        yield return null;

        isInitiated = true;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

            if (hit.collider != null)
            {
                Debug.Log("Clicked on: " + hit.transform.name);
            }
        }
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
