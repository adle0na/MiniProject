using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _Test.LDG.Script;
using UnityEngine;

public class StageStart : MonoBehaviour
{
    [HideInInspector]public int enemyCount;
    
    public EnemySpawnPoint[] enemySpawnPoints;

    [HideInInspector]public bool playerInstage = false;

    public GameObject[] portalVisible;

    [HideInInspector]public bool isClear = false;

    // 플레이어 감지 적 스폰 (
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.TryGetComponent<CharacterController>(out CharacterController controller))
        {
            playerInstage = true;
            if (isClear == false)
            {
                foreach (var _enemySpawnPoint in enemySpawnPoints)
                {
                    enemyCount++;
                    _enemySpawnPoint.StageStartSpawn(this);
                }
            }
            StartCoroutine(EnemyCountCheck());
        }
    }
    
    // 적 처치시 수 감소
    public void EnemyDead()
    {
        enemyCount--;
        StartCoroutine(EnemyCountCheck());
    }

    private void Update()
    {
        StartCoroutine(PlayerCheck());
    }

    // 모든 적 처치시 포탈 활성화
    IEnumerator EnemyCountCheck()
    {
        if (enemyCount == 0 && playerInstage == true)
        {
            isClear = true;
            foreach (var _portalVisible in portalVisible)
            {
                _portalVisible.SetActive(true);
            }
        }
        yield break;
    }
    private IEnumerator PlayerCheck()
    {
        if(playerInstage == false)
        {
            foreach (var _portalVisible in portalVisible)
            {
                _portalVisible.SetActive(false);
            }
            yield break;
        }
        else
        {
            yield return new WaitForSeconds(5);
        }
    }

}