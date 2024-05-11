using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TranslateObject : MonoBehaviour
{
    public string labelName;
    [SerializeField] private TextMeshProUGUI _textLabel;
    [SerializeField] private Button _button;
    
    
    private string _name = "";

    
    public void Initiate(string name)
    {
        _name = name;
        _textLabel.text = name;
        labelName = name;
        _button.onClick.AddListener(delegate { AppManager.Instance.SpeakTTS(name);});
        transform.LookAt(Camera.main.transform);
    }

    public void ChangeLabel(string name)
    {
        _name = name;
        _textLabel.text = name;
        labelName = name;
        _button.onClick.RemoveAllListeners();
        _button.onClick.AddListener(delegate { AppManager.Instance.SpeakTTS(name);});
    }

    public string GetLabel()
    {
        return _name;
    }
}
