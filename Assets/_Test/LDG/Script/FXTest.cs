using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FXTest : MonoBehaviour
{
    [SerializeField] private GameObject explosionFX;
    
    
    
    private void OnCollisionEnter(Collision collision)
    {
        explosionFX.SetActive(true);
        explosionFX.transform.parent = null;
        Destroy(explosionFX, 1);
        Destroy(gameObject);
    }
}
