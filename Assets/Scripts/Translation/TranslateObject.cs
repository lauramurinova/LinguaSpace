using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TranslateObject : MonoBehaviour
{
    public string labelName;
    [SerializeField] private TextMeshProUGUI _textLabel;
    [SerializeField] private Button _speakButton;
    [SerializeField] private Button _listenButton;
    [SerializeField] private Button _reloadWordsButton;
    
    [SerializeField] private Button[] _adjectiveButtons;
    
    
    private string _name = "";

    
    public void Initiate(string name)
    {
        _name = name;
        _textLabel.text = name;

        _speakButton.onClick.AddListener(delegate { AppManager.Instance.SpeakTTS(name);});
        _listenButton.onClick.AddListener(delegate { AppManager.Instance.ListenSTT(name);});

        foreach (var btn in _adjectiveButtons)
        {
            btn.onClick.AddListener(delegate { AppManager.Instance.SpeakTTS(btn.GetComponentInChildren<TextMeshProUGUI>().text);});
        }
        

        labelName = name;
        _button.onClick.AddListener(delegate { AppManager.Instance.SpeakTTS(name);});

        transform.LookAt(Camera.main.transform);
    }

    public void ChangeLabel(string name)
    {
        _name = name;
        _textLabel.text = name;

        _speakButton.onClick.RemoveAllListeners();
        _listenButton.onClick.RemoveAllListeners();
        _speakButton.onClick.AddListener(delegate { AppManager.Instance.SpeakTTS(name);});
        _listenButton.onClick.AddListener(delegate { AppManager.Instance.ListenSTT(name);});
        
        foreach (var btn in _adjectiveButtons)
        {
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(delegate { AppManager.Instance.SpeakTTS(btn.GetComponentInChildren<TextMeshProUGUI>().text);});
        }
        labelName = name;
        _button.onClick.RemoveAllListeners();
        _button.onClick.AddListener(delegate { AppManager.Instance.SpeakTTS(name);});

    }

    public string GetLabel()
    {
        return _name;
    }
}
