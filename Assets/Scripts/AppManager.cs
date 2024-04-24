using System;
using System.Linq;
using TMPro;
using UnityEngine;

[Serializable]
public enum Languages
{
    en = 0,
    es = 1,
    hu = 2,
    fr = 3,
    hr = 4
}

public class AppManager : MonoBehaviour
{
    public static AppManager Instance { get { return _instance; } }

    [Header("Translation")] [SerializeField]
    private TranslationManager _translationManager;
    
    [Header("TextToSpeech")]
    [SerializeField] private TextToSpeechManager _standardTextToSpeechManager;
    [SerializeField] private TextToSpeechManager _premiumTextToSpeechManager;
    
    [Header("SpeechToText")]
    [SerializeField] private SpeechToTextManager _standardSpeechToTextManager;
    [SerializeField] private SpeechToTextManager _premiumSpeechToTextManager;
    
    [Header("UI")]
    [SerializeField] private TMP_Dropdown _languageDropdown;
    
    // standard languages are for free - uses WIT.AI
    [SerializeField] private Languages[] _standardLanguages;
    
    // premium languages should be for subscription - uses Google API
    [SerializeField] private Languages[] _premiumLanguages;

    private TextToSpeechManager _currentTextToSpeechManager;
    private SpeechToTextManager _currentSpeechToTextManager;
    
    private Languages _currentLanguage = Languages.en;
    private static AppManager _instance;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Start()
    {
        SetCurrentManagers();
    }

    /// <summary>
    /// Called by the languages dropdown menu.
    /// Handles the switch between value and language enum, assigns the currently selected language.
    /// </summary>
    public void OnLanguageDropdownValueChanged(int index)
    {
        string selectedLanguage = _languageDropdown.options[index].text;

        switch (selectedLanguage)
        {
            case "Spanish":
            {
                _translationManager.ChangeLabels(Languages.es);
                SetCurrentLanguage(Languages.es);
                break;
            }
            case "English":
            {
                _translationManager.ChangeLabels(Languages.en);
                SetCurrentLanguage(Languages.en);
                break;
            }
            case "French":
            {
                _translationManager.ChangeLabels(Languages.fr);
                SetCurrentLanguage(Languages.fr);
                break;
            }
            case "Hungarian":
            {
                _translationManager.ChangeLabels(Languages.hu);
                SetCurrentLanguage(Languages.hu);
                break;
            }
            case "Croatian":
            {
                _translationManager.ChangeLabels(Languages.hr);
                SetCurrentLanguage(Languages.hr);
                break;
            }
        }

        SetCurrentManagers();
    }

    /// <summary>
    /// Called to initiate text to speech.
    /// Api to use is based on the language definiton.
    /// </summary>
    public void SpeakTTS(string text)
    {
        _currentTextToSpeechManager.Speak(text);
    }

    /// <summary>
    /// Called to initiate speech to text.
    /// Api to use is based on the language definiton.
    /// </summary>
    public void ListenSTT()
    {
        _currentSpeechToTextManager.StartRecording();
    }
    
    /// <summary>
    /// Sets the managers based on the current language standard.
    /// </summary>
    private void SetCurrentManagers()
    {
        if (_standardLanguages.Contains(GetCurrentLanguage()))
        {
            _currentTextToSpeechManager = _standardTextToSpeechManager;
            _currentSpeechToTextManager = _standardSpeechToTextManager;
        }
        else if(_premiumLanguages.Contains(GetCurrentLanguage()))
        {
            _currentTextToSpeechManager = _premiumTextToSpeechManager;
            _currentSpeechToTextManager = _premiumSpeechToTextManager;
        }
    }

    /// <summary>
    /// Returns currently used TTS manager.
    /// </summary>
    public TextToSpeechManager GetCurrentTextToSpeechManager()
    {
        return _currentTextToSpeechManager;
    }

    /// <summary>
    /// Returns currently used STT manager.
    /// </summary>
    public SpeechToTextManager GetCurrentSpeechToTextManager()
    {
        return _currentSpeechToTextManager;
    }

    /// <summary>
    /// Returns enum of currently selected language.
    /// </summary>
    public Languages GetCurrentLanguage()
    {
        return _currentLanguage;
    }

    public void SetCurrentLanguage(Languages language)
    {
        _currentLanguage = language;
    }
}
