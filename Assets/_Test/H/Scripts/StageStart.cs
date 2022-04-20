using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _Test.LDG.Script;
using UnityEngine;

public class StageStart : MonoBehaviour
{
    
    [SerializeField] private string stagenumber;
    
    private int enemyCount = 0;
    
    public EnemySpawnPoint[] enemySpawnPoints;

    // 플레이어 감지 적 스폰 
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.TryGetComponent<CharacterController>(out CharacterController controller))
        {
            foreach (var _enemySpawnPoint in enemySpawnPoints)
            { 
                enemyCount++;
                _enemySpawnPoint.StageStartSpawn(this);
            }
            StartCoroutine(EnemyCountCheck());
        }
    }
    
    // 적 처치시 수 감소
    public void EnemyDead()
    {
        enemyCount--;
    }
    
    // 모든 적 처치시 포탈 활성화
    IEnumerator EnemyCountCheck()
    {
        if (enemyCount == 0)
        {
            GameObject.Find("Portal" + stagenumber).transform.Find("Portal").gameObject.SetActive(true);
        }
        yield break;
    }

}