using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class Portal : MonoBehaviour
{
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

                if (collision.TryGetComponent<CharacterController>(out CharacterController controller))
                {
                    controller.enabled = false;

                    Vector3 playerPos = controller.transform.position;
                    Vector3 portalPos = transform.position;
                    playerPos.y = 0;
                    portalPos.y = 0;

                    controller.transform.position += (portalPos - playerPos).normalized * 10;

                    controller.enabled = true;
                    //player.transform.position = targetPos.position;
                }
            }
        }


    }
}
