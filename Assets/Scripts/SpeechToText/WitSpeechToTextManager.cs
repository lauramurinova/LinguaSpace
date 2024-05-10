using System;
using Oculus.Voice;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class WitSpeechToTextManager : SpeechToTextManager
{
    [SerializeField] private AppVoiceExperience _appVoice;
    [SerializeField] private TextMeshProUGUI _voiceText;
    [SerializeField] private Button _voiceButton;

    [Header("UI")] 
    [SerializeField] private Color _microphoneDisabledColor = Color.red;
    [SerializeField] private Color _microphoneEnabledColor = Color.green;
    [SerializeField] private TextMeshProUGUI _listeningText;

    public override void StartRecording(string textToRecognize)
    {
        _appVoice.Activate();
        _voiceButton.GetComponent<Image>().color = _microphoneEnabledColor;
        _listeningText.gameObject.SetActive(true);
        _appVoice.VoiceEvents.OnFullTranscription.AddListener(transcription =>
        {
            HandleSpeechRecognitionResponse(textToRecognize, transcription);
            _appVoice.Deactivate();
            _voiceButton.GetComponent<Image>().color = _microphoneDisabledColor;
            _listeningText.gameObject.SetActive(false);
            _appVoice.VoiceEvents.OnFullTranscription.RemoveAllListeners();
        });
    }
    
    private void HandleSpeechRecognitionResponse(string textToRecognize, string recognizedText)
    {
        _voiceText.text = recognizedText;
        
        // user got it right
        if (textToRecognize == recognizedText)
        {
            AppManager.Instance.GivePositiveFeedbackToUser();
        }
        // user didnt get it right
        else
        {
            AppManager.Instance.GiveNegativeFeedbackToUser();
        }
    }
}
