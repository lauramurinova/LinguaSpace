using Meta.WitAi.TTS.Utilities;
using UnityEngine;

public class WitTextToSpeechManager : TextToSpeechManager
{
    [SerializeField] private TTSSpeaker _speaker;

    /// <summary>
    /// Initiates Text to Speech by WIT.AI and plays audio clip upon successful request.
    /// </summary>
    public override void Speak(string text)
    {
        _speaker.Speak(text);
    }
}
