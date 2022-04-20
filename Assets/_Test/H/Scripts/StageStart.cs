using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _Test.LDG.Script;
using UnityEngine;

public class StageStart : MonoBehaviour
{
    public bool inStage;

    public int enemyCount;

    public string stagenumber;

    public EnemySpawnPoint[] enemySpawnPoints;
        
    void Awake()
    {
        inStage = false;

        // 디버깅용
        //StartCoroutine(EnemyCountM());
    }

    
    
    private void Update()
    {
        if (enemyCount == 0)
        {
            Debug.Log("포탈 활성화");
            GameObject.Find("Portal" + stagenumber).transform.Find("Portal").gameObject.SetActive(true);
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (collision.GetComponent<PlayerController>().gameObject)
            {
                if (inStage == false)
                {
                    Debug.Log("스테이지 진입");
                    inStage = true;
                    
                    if (inStage == true)
                    {
                        foreach (var _enemySpawnPoint in enemySpawnPoints)
                        {
                            _enemySpawnPoint.StageStartSpawn();
                        }
                    }
                }
            }
        }
    }
    
    // 디버깅용 
    /*
    IEnumerator EnemyCountM()
    {
        yield return new WaitForSeconds(5f);
        Debug.Log("적이 전부 처치됨");
        enemyCount--;
    }
    */
}