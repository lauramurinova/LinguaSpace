using System;
using GoogleTextToSpeech.Scripts;
using GoogleTextToSpeech.Scripts.Data;
using UnityEngine;

public class TextToSpeechManager : MonoBehaviour
{
    [SerializeField] private AudioSource _speaker;
    [SerializeField] private TranslationManager _translationManager;
    [SerializeField] private TextToSpeechHandler _textToSpeechHandler;

    /// <summary>
    /// Initiates Text to Speech by Google Rest Api and plays audio clip upon successful request.
    /// </summary>
    public void Speak(string text)
    {
        Action<AudioClip> audioReceived = AudioClipReceived;
        Action<BadRequestData> errorReceived = ErrorReceived;
        _textToSpeechHandler.GetSpeechAudioFromGoogle(text, _translationManager.GetCurrentLanguage().ToString(), audioReceived, errorReceived);
    }

    /// <summary>
    /// Called on error during Text to Speech request.
    /// </summary>
    private void ErrorReceived(BadRequestData badRequestData)
    {
        Debug.Log($"Error {badRequestData.error.code} : {badRequestData.error.message}");
    }

    /// <summary>
    /// Called on successful Text to Speech request.
    /// </summary>
    private void AudioClipReceived(AudioClip clip)
    {
        _speaker.Stop();
        _speaker.clip = clip;
        _speaker.Play();
    }
}
