using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class PortalH : MonoBehaviour
{
    StageStart _stageStart;

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (collision.GetComponent<PlayerController>().gameObject)
            {
                if (_stageStart.inStage == true)
                {
                    Debug.Log("플레이어 이동 실행");

                }
            }
        }
    }
}
