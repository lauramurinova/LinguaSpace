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

    [Header("UI")] 
    [SerializeField] private Color _microphoneDisabledColor = Color.red;
    [SerializeField] private Color _microphoneEnabledColor = Color.green;
    [SerializeField] private TextMeshProUGUI _listeningText;

    private void Awake()
    {
        _appVoice.VoiceEvents.OnFullTranscription.AddListener((transcription) =>
        {
            _voiceText.text = transcription;
            _appVoice.Deactivate();
            _voiceButton.GetComponent<Image>().color = _microphoneDisabledColor;
            _listeningText.gameObject.SetActive(false);
        });
    }

    public override void StartRecording()
    {
        _appVoice.Activate();
        _voiceButton.GetComponent<Image>().color = _microphoneEnabledColor;
        _listeningText.gameObject.SetActive(true);
    }
}
