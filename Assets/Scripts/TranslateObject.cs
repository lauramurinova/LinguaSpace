using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TranslateObject : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _textLabel;
    [SerializeField] private Button _button;
    
    private string _name = "";
    private TranslationManager _translationManager;

    public void Initiate(string name, TranslationManager translationManager)
    {
        _translationManager = translationManager;
        _name = name;
        _textLabel.text = name;
        _button.onClick.AddListener(delegate { _translationManager.Speak(name);});
        transform.LookAt(Camera.main.transform);
    }

    public void ChangeLabel(string name)
    {
        _name = name;
        _textLabel.text = name;
        _button.onClick.RemoveAllListeners();
        _button.onClick.AddListener(delegate { _translationManager.Speak(name);});
    }

    public string GetLabel()
    {
        return _name;
    }
}
