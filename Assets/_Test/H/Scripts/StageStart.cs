using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _Test.LDG.Script;
using UnityEngine;

public class StageStart : MonoBehaviour
{
    
    public int enemyCount;
    
    public EnemySpawnPoint[] enemySpawnPoints;

    private bool playerInstage = false;

    [SerializeField] private GameObject[] portalVisible;

    private bool stageMovable = false;

    public bool isClear = false;
    
    // 플레이어 감지 적 스폰 
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
                StartCoroutine(EnemyCountCheck());
            }

        }
    }
    
    // 적 처치시 수 감소
    public void EnemyDead()
    {
        enemyCount--;
        StartCoroutine(EnemyCountCheck());
    }
    
    // 모든 적 처치시 포탈 활성화
    IEnumerator EnemyCountCheck()
    {
        if (enemyCount == 0 && playerInstage == true)
        {
            Debug.Log("적을 전부 처치");
            isClear = true;
            foreach (var _portalVisible in portalVisible)
            {
                _portalVisible.SetActive(true);
                stageMovable = true;
            }
            Debug.Log("포탈 활성화");
            // 보상 조건 넣으시면 됩니당
        }
        yield break;
    }

    IEnumerator ClearCheck()
    {
        if (isClear == false)
        {
            Debug.Log("클리어 확인");
            yield break;
        }
    }
}