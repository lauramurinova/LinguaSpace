using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TranslateObject : MonoBehaviour
{
    public string labelName;
    [SerializeField] private TextMeshProUGUI _textLabel;
    [SerializeField] private Button _speakButton;
    [SerializeField] private Button _listenButton;
    [SerializeField] private Button _reloadWordsButton;
    
    [SerializeField] private Button[] _adjectiveButtons;
    [SerializeField] private TextMeshProUGUI _adjectiveText1;
    [SerializeField] private TextMeshProUGUI _adjectiveText2;
    [SerializeField] private TextMeshProUGUI _adjectiveText3;
    
    
    private string _name = "";
    private WordSuggesterHelper _wordSuggesterHelper;

    
    public void Initiate(string name, WordSuggesterHelper wordSuggesterHelper)
    {
        _name = name;
        _textLabel.text = name;

        _wordSuggesterHelper = wordSuggesterHelper;
        _wordSuggesterHelper.Populate(name, AppManager.Instance.GetCurrentLanguage(), _adjectiveText1, _adjectiveText2, _adjectiveText3);
        
        _speakButton.onClick.AddListener(delegate { AppManager.Instance.SpeakTTS(name);});
        _listenButton.onClick.AddListener(delegate { AppManager.Instance.ListenSTT(name);});
        _reloadWordsButton.onClick.AddListener(delegate
        {
            _wordSuggesterHelper.Populate(name, AppManager.Instance.GetCurrentLanguage(), _adjectiveText1, _adjectiveText2, _adjectiveText3);
        });

        foreach (var btn in _adjectiveButtons)
        {
            btn.onClick.AddListener(delegate { AppManager.Instance.SpeakTTS(btn.GetComponentInChildren<TextMeshProUGUI>().text);});
        }
        

        labelName = name;
        // _button.onClick.AddListener(delegate { AppManager.Instance.SpeakTTS(name);});

        transform.LookAt(Camera.main.transform);
    }

    public void ChangeLabel(string name)
    {
        _name = name;
        _textLabel.text = name;

        _speakButton.onClick.RemoveAllListeners();
        _listenButton.onClick.RemoveAllListeners();
        _reloadWordsButton.onClick.RemoveAllListeners();
        _speakButton.onClick.AddListener(delegate { AppManager.Instance.SpeakTTS(name);});
        _listenButton.onClick.AddListener(delegate { AppManager.Instance.ListenSTT(name);});
        _reloadWordsButton.onClick.AddListener(delegate{_wordSuggesterHelper.Populate(name, AppManager.Instance.GetCurrentLanguage(), _adjectiveText1, _adjectiveText2, _adjectiveText3);});
        
        foreach (var btn in _adjectiveButtons)
        {
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(delegate { AppManager.Instance.SpeakTTS(btn.GetComponentInChildren<TextMeshProUGUI>().text);});
        }
        labelName = name;
        // _button.onClick.RemoveAllListeners();
        // _button.onClick.AddListener(delegate { AppManager.Instance.SpeakTTS(name);});

    }

    public void SetRelatedWords(List<Tuple<string, string>> relatedWords)
    {
        for (int i = 0; i < relatedWords.Count; i++)
        {
            _adjectiveButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = relatedWords[0].Item2;
        }
    }

    public string GetLabel()
    {
        return _name;
    }
}
