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
    private float radius = 0.52f; // Adjust this radius according to your needs
    private float maxDistance = 1f; // Adjust this distance according to your needs
    
    
    
    // Start is called before the first frame update
    void Start()
    {
        _appManager = AppManager.Instance;
    }

    // Update is called once per frame
    /*private void Update()
    {
        // Define the sphere cast parameters
        Vector3 center = transform.position;
        Vector3 direction = Vector3.zero; // Adjust this direction according to your needs
         

        // Perform the sphere cast
        RaycastHit hit;
        if (Physics.SphereCast(center, radius, direction, out hit, maxDistance))
        {
            // Get the parent object
            var parentObject = hit.collider.gameObject.GetComponentInParent<MRUKAnchor>();
            if (parentObject != null)
            {
                // Find label object in MRUK anchor
                var labelObject = parentObject.GetComponentInChildren<TranslateObject>();
                if (labelObject != null)
                {
                    _appManager.SpeakTTS(labelObject.labelName);
                    Debug.Log($"SPOKEN WORDS: {labelObject.labelName}");
                }
            }
        }
    }*/
    
    private void OnCollisionEnter(Collision other)
    {
        //Get parent object
        var parentObject = other.gameObject.GetComponentInParent<MRUKAnchor>();
        if (!parentObject) return;
        
        //Find label object in MRUK anchor object
        var labelObject = parentObject.GetComponentInChildren<TranslateObject>();
        if (!labelObject) return;
        _appManager.SpeakTTS(labelObject.labelName);
        

        
    }
    
    private void OnDrawGizmosSelected()
    {
        // Define the sphere cast parameters
        Vector3 center = transform.position;
        float radius = 0.5f; // Adjust this radius according to your needs
        Vector3 direction = Vector3.down; // Adjust this direction according to your needs
        float maxDistance = 1f; // Adjust this distance according to your needs

        // Draw the sphere cast gizmo
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(center, radius);
    }
}
