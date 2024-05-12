using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
using Random = System.Random;
using Toggle = UnityEngine.UI.Toggle;

[Serializable]
public enum Languages
{
    en = 0,
    es = 1,
    hu = 2,
    fr = 3,
    bg = 4
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
    
    [SerializeField] private Toggle[] _languageToggles;
    
    // standard languages are for free - uses WIT.AI
    [SerializeField] private Languages[] _standardLanguages;
    
    // premium languages should be for subscription - uses Google API
    [SerializeField] private Languages[] _premiumLanguages;

    [SerializeField] private TextToSpeechManager _currentTextToSpeechManager;
    [SerializeField] private SpeechToTextManager _currentSpeechToTextManager;
    
    private Languages _currentLanguage = Languages.en;
    private static AppManager _instance;
    
    private string[] _congratulationTexts = new[]
    {
        "Congratulations, you got it right!",
        "Good job!",
        "Nice work, you got it!"
    };
    
    private string[] _tryAgainTexts = new[]
    {
        "You didn't get it this time, try again!",
        "It's not quite right, you can try again!",
        "It's pronounced a bit differently, we can practice more together!"
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
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Start()
    {
        SetCurrentManagers();
    }

    public void ChangeLanguage(string selectedLanguage)
    {
        if (selectedLanguage.Contains("Spanish"))
        {
            _translationManager.ChangeLabels(Languages.es);
            SetCurrentLanguage(Languages.es);
        }
        else if (selectedLanguage.Contains("English"))
        {
            _translationManager.ChangeLabels(Languages.en);
            SetCurrentLanguage(Languages.en);
        }
        else if (selectedLanguage.Contains("Hungarian"))
        {
            _translationManager.ChangeLabels(Languages.hu);
            SetCurrentLanguage(Languages.hu);
        }
        else if (selectedLanguage.Contains("French"))
        {
            _translationManager.ChangeLabels(Languages.fr);
            SetCurrentLanguage(Languages.fr);
        }
        else if (selectedLanguage.Contains("Bulgarian"))
        {
            _translationManager.ChangeLabels(Languages.bg);
            SetCurrentLanguage(Languages.bg);
        }
        SetCurrentManagers();
        Debug.Log("Changed to " + GetCurrentLanguage());
    }
    
    /// <summary>
    /// Translates the given text from given language to desired language.
    /// The translated text is returned in a string within the Unity Event;
    /// </summary>
    public void TranslateTextToLanguage(UnityEvent<string> translateEvent, string text, Languages originLanguage, Languages desiredLanguage)
    {
        _translationManager.TranslateText(translateEvent, text, originLanguage, desiredLanguage);
    }

    /// <summary>
    /// Called to initiate text to speech.
    /// Api to use is based on the language definiton.
    /// </summary>
    public void SpeakTTS(string text)
    {
        _currentTextToSpeechManager.Speak(text);
    }
    
    public void SpeakTTS(GameObject obj)
    {
        _currentTextToSpeechManager.Speak(obj.name);
    }

    /// <summary>
    /// Called to initiate speech to text.
    /// Api to use is based on the language definiton.
    /// </summary>
    public void ListenSTT(string nameToRecognize)
    {
        Debug.Log("I AM HERE");
        _currentSpeechToTextManager.StartRecording(nameToRecognize);
    }
    
    public void ListenSTT()
    {
        _currentSpeechToTextManager.StartRecording("");
    }

    /// <summary>
    /// Gives the user audio feedback, to congratulate.
    /// </summary>
    public void GivePositiveFeedbackToUser()
    {
        _standardTextToSpeechManager.Speak(_congratulationTexts[UnityEngine.Random.Range(0, _congratulationTexts.Length)]);
    }
    
    /// <summary>
    /// Gives the user audio feedback to try again.
    /// </summary>
    public void GiveNegativeFeedbackToUser()
    {
        _standardTextToSpeechManager.Speak(_tryAgainTexts[UnityEngine.Random.Range(0, _tryAgainTexts.Length)]);
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
