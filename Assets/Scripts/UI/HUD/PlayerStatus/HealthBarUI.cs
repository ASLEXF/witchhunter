using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    PlayerHealth playerHealth;

    [SerializeField] GameObject[] hearts;
    [SerializeField] Sprite redHeart;
    [SerializeField] Sprite greyHeart;

    private void Awake()
    {
        playerHealth = PlayerController.Instance.transform.GetChild(1).GetComponent<PlayerHealth>();
    }

    private void Start()
    {
        hideHealthBar();

        GameEvents.Instance.OnBattleModeStarted += showHealthBar;
        GameEvents.Instance.OnExplorationModeStarted += hideHealthBar;
        GameEvents.Instance.OnStoryModeStarted += hideHealthBar;

        GameEvents.Instance.OnPlayerHealthChanged += showHealthBar;
    }

    private void showHealthBar()
    {
        for (int i = 0; i < playerHealth.CurrentHealth; i++)
        {
            hearts[i].GetComponent<Animator>().enabled = true;
            hearts[i].GetComponent<Image>().sprite = redHeart;
            hearts[i].SetActive(true);
        }

        for (int i = playerHealth.CurrentHealth; i < playerHealth.MaxHealth; i++)
        {
            hearts[i].GetComponent<Animator>().enabled = false;
            hearts[i].GetComponent<Image>().sprite = greyHeart;
            hearts[i].SetActive(true);
        }

        for (int i = playerHealth.MaxHealth; i < hearts.Length; i++)
        {
            hearts[i].SetActive(false);
        }
    }

    private void hideHealthBar()
    {
        for (int i = 0;i < hearts.Length; i++)
        {
            hearts[i].SetActive(false);
        }
    }

    private void OnDestroy()
    {
        GameEvents.Instance.OnBattleModeStarted -= showHealthBar;
        GameEvents.Instance.OnExplorationModeStarted -= hideHealthBar;
        GameEvents.Instance.OnStoryModeStarted -= hideHealthBar;
        GameEvents.Instance.OnPlayerHealthChanged -= showHealthBar;
    }
}
