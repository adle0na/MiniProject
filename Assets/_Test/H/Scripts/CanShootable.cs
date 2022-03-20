using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanShootable : MonoBehaviour
{
    TimingManager theTimingManager;

    void Start()
    {
        theTimingManager = FindObjectOfType<TimingManager>();
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            theTimingManager.CheckTiming();
        }
        
    }
}
