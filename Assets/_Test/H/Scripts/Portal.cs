using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class Portal : MonoBehaviour
{
    [SerializeField] GameObject player = null;
    [SerializeField] Transform playerTransform = null;
    
    private StageStart _StageStart;

    void Start()
    {
        _StageStart = FindObjectOfType<StageStart>();
    }
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (_StageStart.inStage == false)
            {
                Debug.Log("플레이어 이동");
                _StageStart.inStage = true;

                collision.transform.position = new Vector3(50, 0, -0);
            }
        }


    }
}
