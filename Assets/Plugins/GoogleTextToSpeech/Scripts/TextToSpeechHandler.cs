using System;
using GoogleTextToSpeech.Scripts.Data;
using UnityEngine;
using Input = GoogleTextToSpeech.Scripts.Data.Input;

namespace GoogleTextToSpeech.Scripts
{
    public class TextToSpeechHandler : MonoBehaviour
    {
        private Action<string> _actionRequestReceived;
        private Action<BadRequestData> _errorReceived;
        private Action<AudioClip> _audioClipReceived;

        private RequestService _requestService;
        private static AudioConverter _audioConverter;

        private string _googleTextToSpeechUrl = "https://texttospeech.googleapis.com/v1/text:synthesize";

        public void GetSpeechAudioFromGoogle(string textToConvert, string voice, Action<AudioClip> audioClipReceived,
            Action<BadRequestData> errorReceived)
        {
            _actionRequestReceived += (requestData => RequestReceived(requestData, audioClipReceived));

            if (_requestService == null)
                _requestService = gameObject.AddComponent<RequestService>();

            if (_audioConverter == null)
                _audioConverter = gameObject.AddComponent<AudioConverter>();

            var dataToSend = new DataToSend
            {
                input =
                    new Input()
                    {
                        text = textToConvert
                    },
                voice =
                    new Voice()
                    {
                        languageCode = voice,
                    },
                audioConfig =
                    new AudioConfig()
                    {
                        audioEncoding = "MP3",
                        pitch = 1f,
                        speakingRate = 1f
                    }
            };

            RequestService.SendDataToGoogle(_googleTextToSpeechUrl, dataToSend, GetApiKey(), _actionRequestReceived,
                errorReceived);
        }
        
        private static void RequestReceived(string requestData, Action<AudioClip> audioClipReceived)
        {
            var audioData = JsonUtility.FromJson<AudioData>(requestData);
            AudioConverter.SaveTextToMp3(audioData);
            _audioConverter.LoadClipFromMp3(audioClipReceived);
        }

        private string GetApiKey()
        {
            return Resources.Load<TextAsset>("Security/APIKey").ToString();
        }
    }
}