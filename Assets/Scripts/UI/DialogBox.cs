using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;

public class DialogBox : MonoBehaviour
{
    private static DialogBox _instance;

    public static DialogBox Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<DialogBox>();
                if (_instance == null)
                    Debug.LogWarning("DialogBox null!");
            }
            return _instance;
        }
    }

    Image _image;
    TMP_Text _title, _content;
    PlayerInput _playerInput;

    [SerializeField] private float _typingLetterDuration = 0.02f;
    [SerializeField] private float _skippingLetterDuration = 0.005f;
    [SerializeField] private float _skippingSentenceDuration = 0.1f;

    private List<Sentence> input;
    private int index = 0;
    private bool isTyping = false;
    private bool isSkipping = false;

    private void Awake()
    {
        if (_instance == null)
            _instance = this;

        _playerInput = GetComponent<PlayerInput>();
        _image = GetComponent<Image>();
        _title = transform.Find("Title").GetComponent<TMP_Text>();
        _content = transform.Find("Content").GetComponent<TMP_Text>();
    }

    private void Start()
    {
        Hide();

        GameEvents.Instance.OnDialogBoxStart += StartTyping;
    }

    public void StartTyping()
    {
        input = ScriptReader.Instance.output;
        if (input != null)
        {
            index = 0;
            StartCoroutine(TypingSentence());
            show();
        }
        else
        {
            Debug.Log("text script not found!");
        }
    }

    private void OnTextScriptUpdate()
    {
        input = ScriptReader.Instance.output;
        if (input != null)
        {
            index = 0;
            StartCoroutine(TypingSentence());
        }
        else
        {
            Debug.Log("text script not found!");
        }
    }

    IEnumerator TypingSentence()
    {
        while (input[index].type == 1 ||  input[index].type == 2)
        {
            index++;
        }

        isTyping = true;
        _title.text = input[index].title;
        _content.text = "";
        foreach (char c in input[index].content)
        {   
            if (!isTyping)
            {
                _content.text = input[index].content;
                break;
            }
            _content.text += c;
            yield return new WaitForSeconds(_typingLetterDuration);
        }
        isTyping = false;
    }

    IEnumerator SkippingSentence()
    {
        isSkipping = true;
        if (isTyping)
        {
            FinishTyping();
        }
        else
        {
            NextSentence();
        }
        for (; index < input.Count; NextSentence())
        {
            //Debug.Log("index: " + index);
            _title.text = input[index].title;
            _content.text = "";
            foreach (char c in input[index].content)
            {
                if (!isSkipping)
                {
                    _content.text = input[index].content;
                    break;
                }
                _content.text += c;
                yield return new WaitForSeconds(_skippingLetterDuration);
            }
            if (!isSkipping)
            {
                break;
            }
            yield return new WaitForSeconds(_skippingSentenceDuration);
        }
    }

    public void OnConfirm(InputAction.CallbackContext value)
    {
        if(gameObject != null && gameObject.activeSelf)
        {
            if (value.started)
            {
                if (!isSkipping)
                {
                    if (isTyping)
                    {
                        FinishTyping();
                    }
                    else
                    {
                        NextSentence();
                        if (index < input.Count)
                        {
                            StartCoroutine(TypingSentence());
                        }
                    }
                }
            }
        }
    }

    public void OnSkip(InputAction.CallbackContext value)
    {
        if(gameObject != null && gameObject.activeSelf)
        {
            if (value.performed)
            {
                StartCoroutine(SkippingSentence());
            }
            else if (value.canceled)
            {
                isSkipping = false;
            }
        }
    }

    private void FinishTyping() => isTyping = false;

    private void NextSentence()
    {
        index++;
        while (index < input.Count && input[index].type != 3 && input[index].type != 4)
        {
            index++;
        }
        if (index >= input.Count)
        {
            GameEvents.Instance.DialogBoxEnded();
            Hide();
        }
    }

    public void LoadText(string fileName)
    {
        Clear();
        ScriptReader.Instance.LoadTextAsset(fileName);
    }

    public void LoadAndStartText(string fileName)
    {
        Clear();
        ScriptReader.Instance.LoadAndStartTextAsset(fileName);
    }

    public void Clear()
    {
        input = null;
        index = 0;
        isTyping = false;
        isSkipping = false;
    }

    public void Hide()
    {
        _image.enabled = false;
        _title.enabled = false;
        _content.enabled = false;
        _playerInput.enabled = false;
    }

    private void show()
    {
        _image.enabled = true;
        _title.enabled = true;
        _content.enabled = true;
        _playerInput.enabled = true;
    }

    public void ClearAndHide()
    {
        Clear();
        Hide();
    }
}
