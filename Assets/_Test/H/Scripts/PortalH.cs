using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class PortalH : MonoBehaviour
{
    
    StageStart _stageStart;
    
    [SerializeField] private Transform spawnPoint;

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.TryGetComponent<CharacterController>(out CharacterController controller))
        {
            // 플레이어 이동
            controller.enabled = false;
            collision.gameObject.transform.position = spawnPoint.position;
            controller.enabled = true;
        }
    }
}
