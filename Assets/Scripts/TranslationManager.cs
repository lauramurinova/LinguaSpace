using System.Collections;
using System.Collections.Generic;
using Meta.XR.MRUtilityKit;
using Newtonsoft.Json.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

public enum TranslateLanguage
{
    en = 0,
    es = 1,
    hu = 2,
    fr = 3,
    hr = 4
}

public class TranslationManager : MonoBehaviour
{
    [Header("MRUK")]
    [SerializeField] private MRUK _mruk;
    [SerializeField] private MRUKAnchor.SceneLabels _sceneLabelsToShow;
    [SerializeField] private GameObject _labelPrefab;
    
    [Header("Translation")]
    [SerializeField] private TMP_Dropdown _languageDropdown;
    [SerializeField] private TextToSpeechManager _textToSpeechManager;

    private List<TranslateObject> _translateObjects = new List<TranslateObject>();
    private TranslateLanguage _currentLanguage = TranslateLanguage.en;

    private string _translationApiUrl = "https://translation.googleapis.com/language/translate/v2?key=";

    /// <summary>
    /// Loads all objects label within the current room.
    /// Called by MRUK on scene loaded event.
    /// </summary>
    public void LoadLabels()
    {
        var _mrukRoom = _mruk.GetCurrentRoom();
            
        foreach (var anchor in _mrukRoom.Anchors)
        {
            if(!_sceneLabelsToShow.ToString().Contains(anchor.GetLabelsAsEnum().ToString())) continue;
            
            var labelObject = Instantiate(_labelPrefab, anchor.transform).GetComponent<TranslateObject>();
            labelObject.Initiate(GetAnchorLabel(anchor), _textToSpeechManager);
            _translateObjects.Add(labelObject);
        }
    }

    /// <summary>
    /// Changes the label language based on the provided desired language enum.
    /// Uses events to check when translation finishes.
    /// </summary>
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

    /// <summary>
    /// Handles sending REST API to Googles Translation API based on the text provided and original language and desired language enum.
    /// On finishing the translation, it invokes the translate event.
    /// </summary>
    public IEnumerator TranslateText(UnityEvent<string> translateEvent, string textToTranslate, TranslateLanguage originLanguage, TranslateLanguage desiredLanguage)
    {
        string url =  _translationApiUrl + GetApiKey();
        string jsonRequestBody = "{\"q\":\"" + textToTranslate + "\",\"source\":\"" + originLanguage + "\",\"target\":\"" + desiredLanguage + "\"}";

        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonRequestBody);

        UnityWebRequest request = new UnityWebRequest(url, UnityWebRequest.kHttpVerbPOST);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            translateEvent.Invoke(ParseTranslatedText(request.downloadHandler.text));
        }
        else
        {
            Debug.LogError("Translation request failed: " + request.error);
        }
    }

    /// <summary>
    /// Returns enum of currently selected language.
    /// </summary>
    public TranslateLanguage GetCurrentLanguage()
    {
        return _currentLanguage;
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
                ChangeLanguage(TranslateLanguage.es);
                break;
            }
            case "English":
            {
                ChangeLanguage(TranslateLanguage.en);
                break;
            }
            case "French":
            {
                ChangeLanguage(TranslateLanguage.fr);
                break;
            }
            case "Hungarian":
            {
                ChangeLanguage(TranslateLanguage.hu);
                break;
            }
            case "Croatian":
            {
                ChangeLanguage(TranslateLanguage.hr);
                break;
            }
        }
    }

    /// <summary>
    /// Gets the proper label from the MRUK Anchor.
    /// </summary>
    private string GetAnchorLabel(MRUKAnchor anchor)
    {
        var text = anchor.GetLabelsAsEnum().ToString();
        if (text.Contains("ART"))
        {
            text = "PICTURE";
        }
        else
        {
            text = text.Split('_')[0];
        }

        return text;
    }

    /// <summary>
    /// Handles Google Translation API response.
    /// Returns the translation in string format.
    /// </summary>
    private string ParseTranslatedText(string jsonResponse)
    {
        JObject json = JObject.Parse(jsonResponse);
        string translatedText = json["data"]["translations"][0]["translatedText"].Value<string>();
        return translatedText;
    }

    private string GetApiKey()
    {
        return Resources.Load<TextAsset>("Security/APIKey").ToString();
    }
}
