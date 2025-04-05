using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class PopulateTestCanvas : MonoBehaviour
{
    [SerializeField] private WordSuggesterHelper _helper;
    [SerializeField] private Button _button;
    [SerializeField] private InputField _input;

    // private bool _isFirstTime = true;
    // public void Start()
    // {
    //     _button.onClick.AddListener(onClick);
    // }
    //
    // public void Update()
    // {
    //     if (_isFirstTime)
    //     {
    //         _isFirstTime = false;
    //         _helper.Populate(_input.text, Languages.es);
    //     }
    // }
    //
    // public void onClick()
    // {
    //     _helper.Populate(_input.text, Languages.es);
    // }
}
