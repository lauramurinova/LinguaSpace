using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Debugger : MonoBehaviour
{
    public TMP_Text debuggerText;
    public TMP_Text debuggerText2;

    private int noOfTimesActivated = 0;
    private int noOfTimesDeactivated = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision other)
    {
        //if (!other.gameObject.CompareTag("Player")) return;
        Debug.Log($" COLLIDED WITH {other.gameObject.name}");
    }
    
    
    public void DebugLog()
    {
        noOfTimesActivated++;
        debuggerText.text = $" Activated   {noOfTimesActivated}   times";
        Debug.Log($"ACTIVATED!!");
    }
    
    public void DebugLog2()
    {
        noOfTimesDeactivated++;
        debuggerText.text = $" Activated    {noOfTimesDeactivated}   times";
        Debug.Log($"NOT ACTIVATED!!");
    }
}
