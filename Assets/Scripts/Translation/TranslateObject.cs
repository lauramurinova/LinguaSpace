using System;
using System.Collections.Generic;
using Oculus.Interaction;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class TranslateObject : MonoBehaviour
{
    public string labelName;
    [SerializeField] private TextMeshProUGUI _textLabel;
    [SerializeField] private Button _speakButton;
    [SerializeField] private Button _labelButton;
    [SerializeField] private Button _listenButton;
    [SerializeField] private Button _reloadWordsButton;
    
    [SerializeField] private Button[] _adjectiveButtons;
    [SerializeField] private Image[] _adjectiveButtonsImages;
    [SerializeField] private TextMeshProUGUI _adjectiveText1;
    [SerializeField] private TextMeshProUGUI _adjectiveText2;
    [SerializeField] private TextMeshProUGUI _adjectiveText3;
    [SerializeField] private GameObject _micBorder;
    
    
    private string _name = "";
    private WordSuggesterHelper _wordSuggesterHelper;
    private int _toggleAdjectives = 0;
    private float _timer = 1f;
    private bool _enable = false;
    private string _lastSelectedWord = "";

    
    public void Initiate(string name, WordSuggesterHelper wordSuggesterHelper)
    {
        _name = name;
        _textLabel.text = AppManager.Instance.CapitalizeFirstLetter(name);

        LoadConnectedWords(name, wordSuggesterHelper);
        SetupListeners(name);

        labelName = name;
        transform.LookAt(Camera.main.transform);
    }

    private void Update()
    {
        if (_toggleAdjectives > 0)
        {
            _timer += Time.deltaTime;
            if (_timer > 0.5f)
            {
                if (_toggleAdjectives < 2)
                {
                    if (_enable)
                    {
                        foreach (var btn in _adjectiveButtons)
                        {
                            btn.GetComponent<Animator>().SetTrigger("Appear");
                        }
                    }
                    else
                    {
                        foreach (var btn in _adjectiveButtons)
                        {
                            btn.GetComponent<Animator>().SetTrigger("Dissapear");
                        }
                    }
                }

                _toggleAdjectives = 0;
            }
        }
    }

    public void DisableAdjectiveButtons(PointerEvent _pointerEvent)
    {
        _toggleAdjectives++;
        _enable = false;
    }
    
    public void EnableAdjectiveButtons(PointerEvent _pointerEvent)
    {
        _toggleAdjectives++;
        _enable = true;
        
    }

    private void LoadConnectedWords(string name, WordSuggesterHelper wordSuggesterHelper)
    {
        _wordSuggesterHelper = wordSuggesterHelper;
        _wordSuggesterHelper.Populate(name, AppManager.Instance.GetCurrentLanguage(), _adjectiveText1, _adjectiveText2, _adjectiveText3);
    }

    private void SetupListeners(string name)
    {
        RemoveOldListeners();
        SetupTTSListeners(name);
        SetupSTTListeners();
        SetupReloadButton(name);
        SetAdjectiveButtonsListeners();
    }

    public void SetLastSelectedWord(string text)
    {
        _lastSelectedWord = text;
    }

    private void SetAdjectiveButtonsListeners()
    {
        for(int i = 0; i < _adjectiveButtons.Length; i++)
        {
            _adjectiveButtons[i].onClick.RemoveAllListeners();
            var btn = _adjectiveButtons[i];
            var index = i;
            _adjectiveButtons[i].onClick.AddListener(delegate
            {
                AppManager.Instance.SpeakTTS(btn.GetComponentInChildren<TextMeshProUGUI>().text);
                _lastSelectedWord = btn.GetComponentInChildren<TextMeshProUGUI>().text + " " + _name;
                _adjectiveButtonsImages[index].enabled = true;
                for(int j = 0; j < _adjectiveButtonsImages.Length; j++)
                {
                    if (index != j)
                    {
                        _adjectiveButtonsImages[j].enabled = false;
                    }
                }
            });
        }
    }

    private void RemoveOldListeners()
    {
        _speakButton.onClick.RemoveAllListeners();
        _labelButton.onClick.RemoveAllListeners();
        _listenButton.onClick.RemoveAllListeners();
        _reloadWordsButton.onClick.RemoveAllListeners();
    }

    private void SetupSTTListeners()
    {
        _listenButton.onClick.AddListener(delegate { AppManager.Instance.ListenSTT(_lastSelectedWord); _micBorder.SetActive(true);
            Invoke(nameof(DeactivateMic), 3f);
        });
    }

    private void SetupReloadButton(string name)
    {
        _reloadWordsButton.onClick.AddListener(delegate
        {
            _wordSuggesterHelper.Populate(name, AppManager.Instance.GetCurrentLanguage(), _adjectiveText1, _adjectiveText2, _adjectiveText3);
        });
    }

    private void SetupTTSListeners(string name)
    {
        _speakButton.onClick.AddListener(delegate
        {
            AppManager.Instance.SpeakTTS(_lastSelectedWord);
        });
        _labelButton.onClick.AddListener(delegate
        {
            _lastSelectedWord = name;
            for(int j = 0; j < _adjectiveButtonsImages.Length; j++)
            {
                _adjectiveButtonsImages[j].enabled = false;
            }
            AppManager.Instance.SpeakTTS(_lastSelectedWord);
        });
    }

    private void DeactivateMic()
    {
        _micBorder.SetActive(false);
    }

    public void ChangeLabel(string name)
    {
        _name = name;
        _textLabel.text = AppManager.Instance.CapitalizeFirstLetter(name);
        _wordSuggesterHelper.Populate(name, AppManager.Instance.GetCurrentLanguage(), _adjectiveText1, _adjectiveText2, _adjectiveText3);
        SetupListeners(name);
        labelName = name;

    }

    public string GetLabel()
    {
        return _name;
    }
}
