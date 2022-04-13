using UnityEngine;

namespace _Test.LDG.Script
{
    public class EnemySpawnPoint : MonoBehaviour
    {
        [SerializeField] private EnemyType spawnEnemyType;
        [SerializeField] private GameObject meleeEnemyPrefab;
        [SerializeField] private GameObject projectileEnemyPrefab;
        [SerializeField] private GameObject explosionEnemyPrefab;
        [Range(0.5f,2)] [SerializeField] private float upgradeStat = 1;

        public void StageStartSpawn()
        {
            GameObject obj = null;
            
            switch (spawnEnemyType)
            {
                case EnemyType.Melee: obj = Instantiate(meleeEnemyPrefab); break;
                case EnemyType.Projectile: obj = Instantiate(projectileEnemyPrefab); break;
                case EnemyType.Explosion: obj = Instantiate(explosionEnemyPrefab); break;
            }

            obj.transform.position = transform.position;
            obj.GetComponent<Enemy>().GetEnemyClass().UpgradeEnemy(upgradeStat);

            foreach (var render in obj.transform.GetChild(0).GetComponentsInChildren<Renderer>())
            {
                foreach (var material in render.materials)
                {
                    material.color = Color.red / upgradeStat;
                }
            }
        }
    }
}