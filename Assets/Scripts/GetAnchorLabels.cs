using System;
using Meta.XR.MRUtilityKit;
using TMPro;
using UnityEngine;

public class GetAnchorLabels : MonoBehaviour
{
    [SerializeField] private GameObject _labelPrefab;
    
    private OVRSceneManager _ovrSceneManager;
    private OVRSceneRoom _sceneRoom;

    private MRUK _mruk;
    private MRUKRoom _mrukRoom;

    public void LoadLabels()
    {
        _mruk = GetComponent<MRUK>();
        _mrukRoom = _mruk.GetCurrentRoom();
            
        foreach (var anchor in _mrukRoom.Anchors)
        {
            var label = Instantiate(_labelPrefab, anchor.transform);
            var text = anchor.GetLabelsAsEnum().ToString();
            var parts = text.Split('_');
            text = parts[0];
            label.GetComponentInChildren<TextMeshProUGUI>().text = text;
            label.transform.LookAt(Camera.main.transform);
        }
            
        // OVRSemanticClassification[] allClassifications = FindObjectsOfType<OVRSemanticClassification>();
        //
        // foreach (var classification in allClassifications)
        // {
        //     var label = Instantiate(_labelPrefab, classification.transform);
        //     label.GetComponentInChildren<TextMeshProUGUI>().text = classification.Labels[0];
        //     label.transform.LookAt(Camera.main.transform);
        // }
    }
}
