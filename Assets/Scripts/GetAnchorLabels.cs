using System;
using TMPro;
using UnityEngine;

public class GetAnchorLabels : MonoBehaviour
{
    private OVRSceneManager _ovrSceneManager;
    private OVRSceneRoom _sceneRoom;
    private OVRScenePlane[] _objects;

    private void Awake()
    {
        _ovrSceneManager = FindObjectOfType<OVRSceneManager>();
        _ovrSceneManager.SceneModelLoadedSuccessfully += SceneLoaded;
    }

    private void SceneLoaded()
    {
        _sceneRoom = FindObjectOfType<OVRSceneRoom>();
        _objects = _sceneRoom.Walls;
        foreach (var wall in _objects)
        {
            Debug.Log("HERE WALL " + wall.name);
            wall.gameObject.AddComponent<TextMeshPro>().text = "Wall";
        }
    }
}
