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
    [SerializeField] private GameObject _voiceButtonBorder;

    public override void StartRecording(string textToRecognize)
    {
        _appVoice.Activate();
        _voiceButtonBorder.SetActive(true);
        _appVoice.VoiceEvents.OnFullTranscription.AddListener(transcription =>
        {
            HandleSpeechRecognitionResponse(textToRecognize, transcription);
            _appVoice.Deactivate();
            _voiceButtonBorder.SetActive(false);
            _appVoice.VoiceEvents.OnFullTranscription.RemoveAllListeners();
        });
    }
    
    private void HandleSpeechRecognitionResponse(string textToRecognize, string recognizedText)
    {
        _voiceText.text = AppManager.Instance.CapitalizeFirstLetter(recognizedText);
        
        if(textToRecognize == "") return;
        
        // user got it right
        if (textToRecognize.Replace("-", "").Replace(" ", "").ToLower() == recognizedText.Replace("-", "").Replace(" ", "").ToLower())
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
