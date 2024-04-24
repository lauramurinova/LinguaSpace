using System;
using System.Collections;
using System.Collections.Generic;
using Meta.WitAi.TTS.Utilities;
using Meta.XR.MRUtilityKit;
using Newtonsoft.Json.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.UI;

public enum TranslateLanguage
{
    en = 0,
    es = 1,
    hu = 2,
    sk = 3
}

public class TranslationManager : MonoBehaviour
{
    [SerializeField] private TTSSpeaker _speaker;
    [SerializeField] private MRUK _mruk;
    [SerializeField] private GameObject _labelPrefab;
    [SerializeField] private TMP_Dropdown _languageDropdown;
    [SerializeField] private TextToSpeechManager _textToSpeechManager;

    private List<TranslateObject> _translateObjects = new List<TranslateObject>();
    private TranslateLanguage _currentLanguage = TranslateLanguage.en;
    
    private void Awake()
    {
        _mruk.SceneLoadedEvent.AddListener(LoadLabels);
    }

    public void LoadLabels()
    {
        var _mrukRoom = _mruk.GetCurrentRoom();
            
        foreach (var anchor in _mrukRoom.Anchors)
        {
            var text = anchor.GetLabelsAsEnum().ToString();
            var parts = text.Split('_');
            text = parts[0];
            var label = Instantiate(_labelPrefab, anchor.transform).GetComponent<TranslateObject>();
            label.Initiate(text, _textToSpeechManager);
            _translateObjects.Add(label);
        }
    }

    public void Speak(string text)
    {
        _speaker.Speak(text);
    }

    public void ChangeLanguage(TranslateLanguage desiredLanguage)
    {
        foreach (var translateObject in _translateObjects)
        {
            var translateEvent = new UnityEvent<string>();
            StartCoroutine(TranslateText(translateEvent, translateObject.GetLabel(),
                _currentLanguage, desiredLanguage));
            translateEvent.AddListener(translatedText =>
            {
                translateObject.ChangeLabel(translatedText);
                translateEvent.RemoveAllListeners();
            });
        }

        _currentLanguage = desiredLanguage;
    }

    public IEnumerator TranslateText(UnityEvent<string> translateEvent, string textToTranslate, TranslateLanguage originLanguage, TranslateLanguage desiredLanguage)
    {
        string url = $"https://translation.googleapis.com/language/translate/v2?key={GetApiKey()}";
        string jsonRequestBody = "{\"q\":\"" + textToTranslate + "\",\"source\":\"" + originLanguage + "\",\"target\":\"" + desiredLanguage + "\"}";

        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonRequestBody);

        UnityWebRequest request = new UnityWebRequest(url, UnityWebRequest.kHttpVerbPOST);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string jsonResponse = request.downloadHandler.text;
            var translatedText = ParseTranslatedText(jsonResponse);
            translateEvent.Invoke(translatedText);
        }
        else
        {
            Debug.LogError("Translation request failed: " + request.error);
        }
    }

    public TranslateLanguage GetCurrentLanguage()
    {
        return _currentLanguage;
    }
    
    public void OnLanguageDropdownValueChanged(int index)
    {
        string selectedLanguage = _languageDropdown.options[index].text;
        
        if (selectedLanguage == "Spanish")
        {
            ChangeLanguage(TranslateLanguage.es);
        }
        else if (selectedLanguage == "English")
        {
            ChangeLanguage(TranslateLanguage.en);
        }
        else if (selectedLanguage == "Hungarian")
        {
            ChangeLanguage(TranslateLanguage.hu);
        }
        else if (selectedLanguage == "Slovak")
        {
            ChangeLanguage(TranslateLanguage.sk);
        }
    }


    private string ParseTranslatedText(string jsonResponse)
    {
        JObject json = JObject.Parse(jsonResponse);
        string translatedText = json["data"]["translations"][0]["translatedText"].Value<string>();
        return translatedText;
    }

    private void Start()
    {
        GetApiKey();
    }

    private string GetApiKey()
    {
        return Resources.Load<TextAsset>("Security/APIKey").ToString();
    }
}
