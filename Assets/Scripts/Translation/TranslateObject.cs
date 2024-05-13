using System;
using System.Collections;
using Meta.XR.MRUtilityKit;
using NaughtyAttributes;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class TranslateObject : MonoBehaviour
{
    public string labelName;
    [SerializeField] private TextMeshProUGUI _textLabel;
    [SerializeField] private Button _button;
    [SerializeField] private Material _highlightMaterial;
    
    
    private string _name = "";
    private Renderer _renderer;
    private Transform _parentObject;
    private bool _isTouchingObject;
    private Material _defaultMaterial;


    

    


    public void Update()
    {
        if (!_renderer || !_parentObject)
        {
            SetMaterialParams();
        }
        
        if (_isTouchingObject)
        {
           SetMaterial(_renderer, _highlightMaterial );
        }
        else if (!_isTouchingObject && _renderer)
        {
            SetMaterial(_renderer, _defaultMaterial);
        }
    }
    
    

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

    

    public void SetMaterial(Renderer renderer, Material mat)
    {
        renderer.material = mat;
    }

    private IEnumerator SetMaterialChangeParamsRoutine(float duration)
    {
        yield return new WaitForSeconds(duration);
        SetMaterialParams();
        
    }
    
    

    private void SetMaterialParams()
    {
        _parentObject = GetComponentInParent<MRUKAnchor>().transform;
        _renderer = _parentObject.GetComponentInChildren<Renderer>();
        _defaultMaterial = _renderer.material;
    }

    public void ObjectSelectionBoolFlag()
    {
        _isTouchingObject = true;
        StartCoroutine(SetBoolFlagOff(2f, _isTouchingObject));
    }
    private IEnumerator SetBoolFlagOff(float duration, bool boolToSetOff)
    {
        yield return new WaitForSeconds(duration);
        boolToSetOff = false;

    }
    
}
