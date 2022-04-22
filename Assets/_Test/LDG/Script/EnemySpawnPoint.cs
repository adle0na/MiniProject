using System;
using System.Collections;
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
        [SerializeField] private GameObject meleeGEnemyPrefab;
        [SerializeField] private GameObject projectileGEnemyPrefab;
        [SerializeField] private GameObject explosionGEnemyPrefab;
        [SerializeField] private GameObject speedGEnemyPrefab;
        [SerializeField] private GameObject middleBossEnemyPrefab;
        [SerializeField] private GameObject bossEnemyPrefab;
        
        [Range(0.5f,2)] [SerializeField] private float upgradeStat = 1;

        private StageStart _stageStart;
        
        private Enemy enemy = null;
        
        public void StageStartSpawn(StageStart stageStart = default)
        {
            _stageStart = stageStart;
            GameObject obj = null;
            
            switch (spawnEnemyType)
            {
                case EnemyType.Melee: obj = Instantiate(meleeEnemyPrefab); break;
                case EnemyType.Speed: obj = Instantiate(speedEnemyPrefab); break;
                case EnemyType.Projectile: obj = Instantiate(projectileEnemyPrefab); break;
                case EnemyType.Explosion: obj = Instantiate(explosionEnemyPrefab); break;
                case EnemyType.MeleeG: obj = Instantiate(meleeGEnemyPrefab); break;
                case EnemyType.SpeedG: obj = Instantiate(speedGEnemyPrefab); break;
                case EnemyType.ProjectileG: obj = Instantiate(projectileGEnemyPrefab); break;
                case EnemyType.ExplosionG: obj = Instantiate(explosionGEnemyPrefab); break;
                case EnemyType.MiddleBoss: obj = Instantiate(middleBossEnemyPrefab); break;
                case EnemyType.Boss: obj = Instantiate(bossEnemyPrefab); break;
            }

            if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit,Mathf.Infinity))
            {
                enemy = obj.GetComponent<Enemy>();
                enemy.GetEnemyClass().UpgradeEnemy(upgradeStat);
                enemy.GetEnemyClass().OnDeaded += OnDeadEnemy;
                
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

        private void OnDeadEnemy()
        {
            if(_stageStart != null)
                _stageStart.EnemyDead();
            enemy.GetEnemyClass().OnDeaded -= OnDeadEnemy;
        }
    }
}