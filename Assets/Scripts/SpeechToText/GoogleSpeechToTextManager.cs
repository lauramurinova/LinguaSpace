using System;
using System.Collections;
using Newtonsoft.Json.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.UI;

public class GoogleSpeechToTextManager : SpeechToTextManager
{
    [Header("Translation")]
    [SerializeField] private int _recordingDuration = 2;
    
    [Header("UI")]
    [SerializeField] private GameObject _voiceButtonBorder;
    [SerializeField] private TextMeshProUGUI _translatedTextUI;

    // Google Speech To Text url
    private const string SpeechToTextUrl = "https://speech.googleapis.com/v1/speech:recognize?key=";
    
    // make sure each coroutine runs once
    private Coroutine _recordingCoroutine;
    private Coroutine _speechRecognitionCoroutine;

    /// <summary>
    /// Starts speech recognition.
    /// </summary>
    public override void StartRecording(string textToRecognize)
    {
        if (_recordingCoroutine != null) return;
        
        _recordingCoroutine = StartCoroutine(RecordAndRecognizeSpeech(textToRecognize));
    }

    /// <summary>
    /// Handles activating the microphone and starting the speech recognition (through sending REST API to Google)
    /// </summary>
    private IEnumerator RecordAndRecognizeSpeech(string textToRecognize)
    {
        var audioClip = ActivateMicrophone();

        yield return new WaitForSeconds(_recordingDuration);

        DeactivateMicrophone();
        
        Action<string> action = (recognizedText) =>
        {
            HandleSpeechRecognitionResponse(textToRecognize, recognizedText);
        };

        if (_speechRecognitionCoroutine == null)
        {
            StartCoroutine(RecognizeSpeech(audioClip, action));
        }

        _recordingCoroutine = null;
    }

    private void HandleSpeechRecognitionResponse(string textToRecognize, string recognizedText)
    {
        _translatedTextUI.text = recognizedText;
        
        if(textToRecognize == "") return;
        
        // user got it right
        if (textToRecognize.ToLower() == recognizedText.ToLower())
        {
            AppManager.Instance.GivePositiveFeedbackToUser();
        }
        // user didnt get it right
        else
        {
            AppManager.Instance.GiveNegativeFeedbackToUser();
        }
    }

    /// <summary>
    /// Activates the microphone for listening.
    /// </summary>
    private AudioClip ActivateMicrophone()
    {
        _voiceButtonBorder.SetActive(true);
        return Microphone.Start(null, false, _recordingDuration, 44100);
    }

    /// <summary>
    /// Deactivates the microphone.
    /// </summary>
    private void DeactivateMicrophone()
    {
        Microphone.End(null);
        _voiceButtonBorder.SetActive(false);
    }

    /// <summary>
    /// Handles sending REST API to Googles Speech to Text with the recorder audioClip.
    /// On finishing the translation, it invokes the onComplete action.
    /// </summary>
    private IEnumerator RecognizeSpeech(AudioClip audioClip, Action<string> onComplete)
    {
        string jsonRequestBody = "{ \"config\": { \"encoding\": \"LINEAR16\", \"sampleRateHertz\": 44100, \"languageCode\": \""
                                 + AppManager.Instance.GetCurrentLanguage() + "\", \"enableWordTimeOffsets\": false }, \"audio\": { \"content\": \""
                                 + ConvertAudioClipToString(audioClip) + "\" } }";

        
        UnityWebRequest request = UnityWebRequest.PostWwwForm(SpeechToTextUrl + GetApiKey(), "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonRequestBody);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            onComplete?.Invoke(ParseTranscript(request.downloadHandler.text));
        }
        else
        {
            Debug.LogError("Speech recognition request failed: " + request.error);
        }

        _speechRecognitionCoroutine = null;
    }
    
    /// <summary>
    /// Handles Google Speech to Text API Response.
    /// Returns the translation in string format.
    /// </summary>
    private string ParseTranscript(string jsonResponse)
    {
        JObject json = JObject.Parse(jsonResponse);
        string transcript = json["results"][0]["alternatives"][0]["transcript"].Value<string>();
        return transcript;
    }
    
    /// <summary>
    /// Converts provided audioClip in string (Base64) string.
    /// </summary>
    private string ConvertAudioClipToString(AudioClip audioClip)
    {
        return Convert.ToBase64String(AudioClipToBytes(audioClip));
    }
    
    /// <summary>
    /// Returns provided audioClip in byte array.
    /// </summary>
    private byte[] AudioClipToBytes(AudioClip audioClip)
    {
        float[] samples = new float[audioClip.samples];
        audioClip.GetData(samples, 0);

        byte[] byteArray = new byte[samples.Length * 2];
        int rescaleFactor = 32767;

        for (int i = 0; i < samples.Length; i++)
        {
            short value = (short)(samples[i] * rescaleFactor);
            byteArray[i * 2] = (byte)value;
            byteArray[i * 2 + 1] = (byte)(value >> 8);
        }

        return byteArray;
    }
    
    private string GetApiKey()
    {
        return Resources.Load<TextAsset>("Security/APIKey").ToString();
    }
}
