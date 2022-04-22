using System;
using UnityEngine;

namespace _Test.LDG.Script
{
    [CreateAssetMenu(fileName = "NewEnemyData", menuName = "GameData/EnemyData")]
    public class EnemyClass : ScriptableObject
    {
        [Serializable]
        public class ProjectilePrefab
        {
            public GameObject prefab;
            public float speed;
            public float destroyTime;
        }

        public void Initialize()
        {
            curHealth = maxHealth;
        }
    
        [SerializeField] private EnemyType enemyType = EnemyType.Melee;
        [Tooltip("최대 체력")] [SerializeField] private int maxHealth = 100;
        [Tooltip("공격력")] [SerializeField] private int attackPower = 1;
        [Tooltip("공격 대기시간")] [SerializeField] private float attackDelay = 1;
        [Tooltip("공격 범위")] [SerializeField] private float attackRadius = 3;
        [Tooltip("탐지 범위")] [SerializeField] private float detectRadius = 6;
        [Tooltip("이동 속도")] [SerializeField] private float speed = 3;
        [Tooltip("드랍 아이템")] [SerializeField] private GameObject[] dropItems = new GameObject[0];
        [Tooltip("발사체 정보")] [SerializeField] private ProjectilePrefab projectile = new ProjectilePrefab();
        
        [Tooltip("공격 효과1 정보")] [SerializeField] private ProjectilePrefab attackFx1 = new ProjectilePrefab();
        [Tooltip("공격 효과1 정보")] [SerializeField] private ProjectilePrefab attackFx2 = new ProjectilePrefab();
        [Tooltip("공격 효과1 정보")] [SerializeField] private ProjectilePrefab attackFx3 = new ProjectilePrefab();
    
        private int curHealth = 100;
        private bool isDead = false;
        
        public event Action OnDeaded;

        public void HitHealth(float damage)
        {
            if(isDead) { return; }
            
            curHealth -= Mathf.RoundToInt(damage);
            if (curHealth > 0) { return; }
            
            curHealth = 0;
            isDead = true;
            OnDeaded?.Invoke();
        }

        public void UpgradeEnemy(float value)
        {
            int toIntValue = (int) value;
            curHealth = maxHealth *= toIntValue;
            attackPower *= (int) toIntValue;
        }

        public EnemyType EnemyType => enemyType;
        public int MaxHealth => maxHealth;
        public int AttackPower => attackPower;
        public float AttackDelay => attackDelay;
        public float AttackRadius => attackRadius;
        public float DetectRadius => detectRadius;
        public float Speed => speed;
        public GameObject[] DropItems => dropItems;
        public ProjectilePrefab Projectile => projectile;
        public ProjectilePrefab AttackFx1 => attackFx1;
        public ProjectilePrefab AttackFx2 => attackFx2;
        public ProjectilePrefab AttackFx3 => attackFx3;
        public int CurHealth => curHealth;
        public bool IsDead => isDead;

    }

    public enum EnemyType
    {
        Melee,
        Projectile,
        Explosion,
        Speed,
        MeleeG,
        ProjectileG,
        ExplosionG,
        SpeedG,
        Boss
    }
}