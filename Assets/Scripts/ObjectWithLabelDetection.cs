using System;
using System.Collections;
using System.Collections.Generic;
using Meta.XR.MRUtilityKit;
using TMPro;
using UnityEngine;

public class ObjectWithLabelDetection : MonoBehaviour
{
    /*[Header("Debugger")]
    public TMP_Text debuggerText2;*/
    
    private AppManager _appManager;
    [SerializeField]private float radius = 0.1f; // Adjust this radius according to your needs
    private float maxDistance = 0.02f; // Adjust this distance according to your needs
    private Vector3 center;
    
    
    
    // Start is called before the first frame update
    void Start()
    {
        _appManager = AppManager.Instance;
        }

    // Update is called once per frame
    private void Update()
    {
        //var k = transform.position;
        center = transform.position;
        // Define the sphere cast parameters
        Vector3 direction = Vector3.forward; // Adjust this direction according to your needs
         

        // Perform the sphere cast
        RaycastHit [] hits = Physics.SphereCastAll(center, radius, direction, maxDistance);
        foreach (RaycastHit hit in hits)
        {
            Debug.Log($"Found {hit.collider.gameObject.name}");

            // Get the parent object
            var parentObject = hit.collider.gameObject.GetComponentInParent<MRUKAnchor>();
            if (!parentObject) continue;

            // Find label object in MRUK anchor
            var labelObject = parentObject.GetComponentInChildren<TranslateObject>();
            labelObject.ObjectSelectionBoolFlag();
            if (!labelObject) continue;

            _appManager.SpeakTTS(labelObject.labelName);
            Debug.Log($"SPOKEN WORDS: {this.gameObject.name} {labelObject.labelName}");


        }
    }
    
    /*private void OnCollisionEnter(Collision other)
    {
        Debug.Log("DETECTED");
        //Get parent object
        var parentObject = other.gameObject.GetComponentInParent<MRUKAnchor>();
        if (!parentObject) return;
        
        //Find label object in MRUK anchor object
        var labelObject = parentObject.GetComponentInChildren<TranslateObject>();
        if (!labelObject) return;
        _appManager.SpeakTTS(labelObject.labelName);
        
        

        
    }*/
    
    private void OnDrawGizmosSelected()
    {
        // Draw the sphere cast gizmo
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(center, radius);
        
    }
}
