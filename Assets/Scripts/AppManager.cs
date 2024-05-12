using System;
using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using Random = System.Random;

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
    
    [Header("UI")]
    [SerializeField] private TMP_Dropdown _languageDropdown;
    [SerializeField] private AnswerFeedback _answerFeedback;
    [SerializeField] private GameObject _correctUIPrefab;
    [SerializeField] private GameObject _tryAgainUIPrefab;
    
    [Header("SFX")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip _correctSFX;
    [SerializeField] private AudioClip _tryAgainSFX;
    
    // standard languages are for free - uses WIT.AI
    [SerializeField] private Languages[] _standardLanguages;
    
    // premium languages should be for subscription - uses Google API
    [SerializeField] private Languages[] _premiumLanguages;

    private TextToSpeechManager _currentTextToSpeechManager;
    private SpeechToTextManager _currentSpeechToTextManager;
    
    private Languages _currentLanguage = Languages.en;
    private static AppManager _instance;
    private bool isSpeaking = false;

    
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

    /// <summary>
    /// Called by the languages dropdown menu.
    /// Handles the switch between value and language enum, assigns the currently selected language.
    /// </summary>
    public void OnLanguageDropdownValueChanged(int index)
    {
        string selectedLanguage = _languageDropdown.options[index].text;

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
        if (isSpeaking) return;
        isSpeaking = true;
        _currentTextToSpeechManager.Speak(text);
        StartCoroutine(isSpeakingToggle(1.3f));
    }

    private IEnumerator isSpeakingToggle(float duration)
    {
        yield return new WaitForSeconds(duration);
        isSpeaking = false;
    }

    /// <summary>
    /// Called to initiate speech to text.
    /// Api to use is based on the language definiton.
    /// </summary>
    public void ListenSTT()
    {
        // TODO - when Object label has the listen button update the string according to that
        _currentSpeechToTextManager.StartRecording("TEXT FROM LABEL THAT USER WANTS TO CHECK THE PRONUNCIATION OF");
    }

    /// <summary>
    /// Gives the user audio feedback, to congratulate.
    /// </summary>
    public void GivePositiveFeedbackToUser()
    {
        _standardTextToSpeechManager.Speak(_congratulationTexts[UnityEngine.Random.Range(0, _congratulationTexts.Length)]);
        _answerFeedback.ShowAnswerUI(_correctUIPrefab);
        PlaySFX(_correctSFX);
    }
    
    /// <summary>
    /// Gives the user audio feedback to try again.
    /// </summary>
    public void GiveNegativeFeedbackToUser()
    {
        _standardTextToSpeechManager.Speak(_tryAgainTexts[UnityEngine.Random.Range(0, _tryAgainTexts.Length)]);
        _answerFeedback.ShowAnswerUI(_tryAgainUIPrefab);
        PlaySFX(_tryAgainSFX);
    }
    
    public void PlaySFX(AudioClip soundEffect)
    {
        if (audioSource != null && soundEffect != null)
        {
            audioSource.PlayOneShot(soundEffect);
        }
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
