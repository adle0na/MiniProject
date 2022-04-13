using System;
using UnityEngine;
using UnityEngine.AI;

namespace _Test.LDG.Script
{
    public class EnemySpawnPoint : MonoBehaviour
    {
        [SerializeField] private EnemyType spawnEnemyType;
        [SerializeField] private GameObject meleeEnemyPrefab;
        [SerializeField] private GameObject projectileEnemyPrefab;
        [SerializeField] private GameObject explosionEnemyPrefab;
        [SerializeField] private GameObject speedEnemyPrefab;
        [Range(0.5f,2)] [SerializeField] private float upgradeStat = 1;

        private void Start()
        {
            StageStartSpawn();
        }

        public void StageStartSpawn()
        {
            GameObject obj = null;
            
            switch (spawnEnemyType)
            {
                case EnemyType.Melee: obj = Instantiate(meleeEnemyPrefab); break;
                case EnemyType.Speed: obj = Instantiate(speedEnemyPrefab); break;
                case EnemyType.Projectile: obj = Instantiate(projectileEnemyPrefab); break;
                case EnemyType.Explosion: obj = Instantiate(explosionEnemyPrefab); break;
            }

            if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit,Mathf.Infinity))
            {
                obj.GetComponent<Enemy>().GetEnemyClass().UpgradeEnemy(upgradeStat);
                obj.GetComponent<NavMeshAgent>().Warp(hit.point);

                foreach (var render in obj.transform.GetChild(0).GetComponentsInChildren<Renderer>())
                {
                    foreach (var material in render.materials)
                    {
                        material.color = Color.red / upgradeStat;
                    }
                }
            }
            else
            {
                Debug.Log("바닥이 없자나!!!");
            }

        }
    }
}