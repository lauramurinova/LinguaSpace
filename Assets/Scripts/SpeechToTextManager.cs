using System;
using System.Collections;
using System.IO;
using Newtonsoft.Json.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class SpeechToTextManager : MonoBehaviour
{
    public int recordingDuration = 2;
    public AudioSource audioSource;
    public Button recordButton;
    [SerializeField] private TranslationManager _translationManager;
    [SerializeField] private TextMeshProUGUI _translatedText;

    private const string SpeechToTextUrl = "https://speech.googleapis.com/v1/speech:recognize?key=";
    private Coroutine recording = null;

    public void StartRecording()
    {
        if (recording != null) return;
        
        recording = StartCoroutine(RecordAndRecognizeSpeech());
    }

    IEnumerator RecordAndRecognizeSpeech()
    {
        recordButton.GetComponent<Image>().color = Color.green;;
        AudioClip recordedClip = Microphone.Start(null, false, recordingDuration, 44100);

        yield return new WaitForSeconds(recordingDuration);

        Microphone.End(null);
        recordButton.GetComponent<Image>().color = Color.red;
        Action<string> action = (recognizedText) =>
        {
            _translatedText.text = ParseTranscript(recognizedText);
            Debug.Log("Recognized text: " + recognizedText);
        };

        StartCoroutine(RecognizeSpeech(recordedClip, action));
    }
    
    private string ParseTranscript(string jsonResponse)
    {
        // Parse the JSON response
        JObject json = JObject.Parse(jsonResponse);

        // Extract the transcript from the JSON structure
        string transcript = json["results"][0]["alternatives"][0]["transcript"].Value<string>();

        // Return the transcript
        return transcript;
    }
    
    private byte[] AudioClipToBytes(AudioClip audioClip)
    {
        float[] samples = new float[audioClip.samples];
        audioClip.GetData(samples, 0);

        byte[] byteArray = new byte[samples.Length * 2];
        int rescaleFactor = 32767; // to convert float to Int16

        for (int i = 0; i < samples.Length; i++)
        {
            short value = (short)(samples[i] * rescaleFactor);
            byteArray[i * 2] = (byte)value;
            byteArray[i * 2 + 1] = (byte)(value >> 8);
        }

        return byteArray;
    }

    IEnumerator RecognizeSpeech(AudioClip audioClip, System.Action<string> onComplete)
    {
        string base64Audio = Convert.ToBase64String(AudioClipToBytes(audioClip));
        string jsonRequestBody = "{ \"config\": { \"encoding\": \"LINEAR16\", \"sampleRateHertz\": 44100, \"languageCode\": \"" + _translationManager.GetCurrentLanguage() + "\", \"enableWordTimeOffsets\": false }, \"audio\": { \"content\": \"" + base64Audio + "\" } }";

        
        UnityWebRequest request = UnityWebRequest.PostWwwForm(SpeechToTextUrl + GetApiKey(), "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonRequestBody);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string jsonResponse = request.downloadHandler.text;
            // Parse JSON response to get recognized text
            string recognizedText = ParseRecognizedText(jsonResponse);
            onComplete?.Invoke(recognizedText);
        }
        else
        {
            Debug.LogError("Speech recognition request failed: " + request.error);
            onComplete?.Invoke("");
        }
    }

    private string ParseRecognizedText(string jsonResponse)
    {
        // Parse JSON response to get recognized text
        // Implement your parsing logic here
        return jsonResponse;
    }
    
    private string GetApiKey()
    {
        return Resources.Load<TextAsset>("Security/APIKey").ToString();
    }
}
