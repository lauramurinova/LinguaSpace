using System;
using Oculus.Voice;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WitSpeechToTextManager : SpeechToTextManager
{
    [SerializeField] private AppVoiceExperience _appVoice;
    [SerializeField] private TextMeshProUGUI _voiceText;
    [SerializeField] private Button _voiceButton;

    private void Awake()
    {
        _appVoice.VoiceEvents.OnFullTranscription.AddListener((transcription) =>
        {
            _voiceText.text = transcription;
            _appVoice.Deactivate();
            _voiceButton.GetComponent<Image>().color = Color.red;
        });
    }

    public override void StartRecording()
    {
        _appVoice.Activate();
        _voiceButton.GetComponent<Image>().color = Color.green;
    }
}