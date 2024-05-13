using System;
using System.Collections;
using System.Collections.Generic;
using Meta.XR.MRUtilityKit;
using Newtonsoft.Json.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

public class TranslationManager : MonoBehaviour
{
    [Header("MRUK")]
    [SerializeField] private MRUK _mruk;
    [SerializeField] private MRUKAnchor.SceneLabels _sceneLabelsToShow;
    [SerializeField] private GameObject _labelPrefab;
    [SerializeField] private WordSuggesterHelper _wordSuggesterHelper;
    [SerializeField] private Transform _playerTrackingObj;

    [Header("UI")] 
    [SerializeField] private TextMeshProUGUI _lastSelectedWordText;
    [SerializeField] private GameObject _noSelectedWord;
    [SerializeField] private GameObject _selectedWord;

    private List<TranslateObject> _translateObjects = new List<TranslateObject>();
    private Vector3 _lastUsersPosition;
    private float _rotationTimer = 0f;

    private string _translationApiUrl = "https://translation.googleapis.com/language/translate/v2?key=";

    private void Start()
    {
        _lastUsersPosition = _playerTrackingObj.position;
    }

    void Update()
    {
        // rotate labels based on users position - smoothly
        if (Vector3.Distance(_lastUsersPosition, _playerTrackingObj.position) > 0.5f)
        {
            foreach (var translateObject in _translateObjects)
            {
                Vector3 directionToTarget = _playerTrackingObj.position - translateObject.transform.position;
                Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
                translateObject.transform.rotation = Quaternion.Lerp(translateObject.transform.rotation, targetRotation,
                    3f * Time.deltaTime);
            }

            if (_rotationTimer > 3f)
            {
                _lastUsersPosition = _playerTrackingObj.position;
                _rotationTimer = 0f;
            }

            _rotationTimer += Time.deltaTime;
        }
    }
    
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
            labelObject.Initiate(GetAnchorLabel(anchor), _wordSuggesterHelper);
            labelObject.selectedObject.AddListener(ChangeLastSelectedObject);
            _translateObjects.Add(labelObject);
        }
    }

    private void ChangeLastSelectedObject(TranslateObject translateObject)
    {
        _lastSelectedWordText.text = AppManager.Instance.CapitalizeFirstLetter(translateObject.GetLastSelectedWord());
        _noSelectedWord.SetActive(false);
        _selectedWord.SetActive(true);
    }

    public string GetSelectedObjectName()
    {
        return _lastSelectedWordText.text;
    }

    /// <summary>
    /// Changes the label language based on the provided desired language enum.
    /// Uses events to check when translation finishes.
    /// </summary>
    public void ChangeLabels(Languages desiredLanguage)
    {
        foreach (var translateObject in _translateObjects)
        {
            var translateEvent = new UnityEvent<string>();
            TranslateText(translateEvent, translateObject.GetLabel(), AppManager.Instance.GetCurrentLanguage(), desiredLanguage);
            translateEvent.AddListener(translatedText =>
            {
                translateObject.ChangeLabel(translatedText);
                translateEvent.RemoveAllListeners();
            });
        }
        
        var translateEvent2 = new UnityEvent<string>();
        TranslateText(translateEvent2, GetSelectedObjectName(), AppManager.Instance.GetCurrentLanguage(), desiredLanguage);
        translateEvent2.AddListener(translatedText =>
        {
            _lastSelectedWordText.text = AppManager.Instance.CapitalizeFirstLetter(translatedText);
        });
    }

    /// <summary>
    /// Translates the given text to a given desired language.
    /// Response is sent through Unity Event in a string.
    /// </summary>
    public void TranslateText(UnityEvent<string> translateEvent, string text, Languages originLanguage, Languages desiredLanguage)
    {
        StartCoroutine(TranslateTextCor(translateEvent, text, originLanguage, desiredLanguage));
    }

    /// <summary>
    /// Handles sending REST API to Googles Translation API based on the text provided and original language and desired language enum.
    /// On finishing the translation, it invokes the translate event.
    /// </summary>
    private IEnumerator TranslateTextCor(UnityEvent<string> translateEvent, string textToTranslate, Languages originLanguage, Languages desiredLanguage)
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
