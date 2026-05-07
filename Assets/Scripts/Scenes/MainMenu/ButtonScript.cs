using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
[RequireComponent(typeof(Animator))]
public class ButtonScript : MonoBehaviour, IPointerEnterHandler
{
    public bool Interactable
    {
        get
        {
            return (GetComponent<Button>().interactable);
        }
        set
        {
            GetComponent<Button>().interactable = value;
            if (value)
            {
                text.color = originalColor;
            }
            else
            {
                text.color = originalColor * tintColor;
            }
        }
    }

    Button button;
    Animator animator;
    TMP_Text text;

    [SerializeField] private int buttonIndex;
    [SerializeField] private Color originalColor;
    [SerializeField] private Color tintColor = new Color(80, 80, 80, 128);

    private void Awake()
    {
        button = GetComponent<Button>();
        animator = GetComponent<Animator>();
        text = transform.GetChild(0).GetComponent<TMP_Text>();
        originalColor = text.color;
    }

    private void Start()
    {
        for (int i = 0; i < transform.parent.childCount; i++) 
        {
            if (transform.parent.GetChild(i).gameObject == gameObject)
            {
                buttonIndex = i;
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        GameEvents.Instance.ButtonHover(buttonIndex);
    }

    public void Select() => button.Select();

    //public void Play() => animator.speed = 1f;

    //public void Stop() => animator.speed = 0f;

    public void Invoke()
    {
        button.onClick.Invoke();
    }
}
