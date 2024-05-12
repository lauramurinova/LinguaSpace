using UnityEngine;

public abstract class SpeechToTextManager : MonoBehaviour
{
    public abstract void StartRecording(string textToRecognize);
}
