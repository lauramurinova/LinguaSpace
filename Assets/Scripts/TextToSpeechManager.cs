using System;
using GoogleTextToSpeech.Scripts;
using GoogleTextToSpeech.Scripts.Data;
using UnityEngine;

public class TextToSpeechManager : MonoBehaviour
{
    [SerializeField] private AudioSource _speaker;
    [SerializeField] private TranslationManager _translationManager;
    [SerializeField] private TextToSpeechHandler _textToSpeechHandler;

    private const string TextToSpeechUrl = "https://texttospeech.googleapis.com/v1/text:synthesize?key=";

    public void Speak(string text)
    {
        Action<AudioClip> audioReceived = AudioClipReceived;
        Action<BadRequestData> errorReceived = ErrorReceived;
        _textToSpeechHandler.GetSpeechAudioFromGoogle(text, _translationManager.GetCurrentLanguage().ToString(), audioReceived, errorReceived);
    }

    private void ErrorReceived(BadRequestData badRequestData)
    {
        Debug.Log($"Error {badRequestData.error.code} : {badRequestData.error.message}");
    }

    private void AudioClipReceived(AudioClip clip)
    {
        _speaker.Stop();
        _speaker.clip = clip;
        _speaker.Play();
    }
}
