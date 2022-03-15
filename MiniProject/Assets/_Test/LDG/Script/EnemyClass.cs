using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyData",menuName = "GameData/EnemyData")]
public class EnemyClass : ScriptableObject
{
    private EnemyClass _enemyClass;
    public EnemyClass EnemyClassBase
    {
        get
        {
            _enemyClass = this;
            return Instantiate(_enemyClass);
        }
    }
    
    public EnemyAttackType EnemyAttackType;
    public EnemyEnhanceType EnemyEnhanceType;

    [Tooltip("최대 체력")]
    public int maxHealth;
    [Tooltip("현재 체력")]
    internal int curHealth;
    [Tooltip("공격력")]
    public int damage;
    [Tooltip("공격 대기시간")]
    public float attackDelay;
    [Tooltip("공격 범위")]
    public float attackRadius;
    [Tooltip("이동 속도")]
    public float speed;
    [Tooltip("드랍 아이템")]
    public GameObject[] dropItems;

    public void SpecialEnemy(float value = 1.5f)
    {
        maxHealth = Mathf.RoundToInt(maxHealth * value);
        curHealth = Mathf.RoundToInt(curHealth * value);
        damage = Mathf.RoundToInt(damage * value);
    }
}

public enum EnemyAttackType
{
    Mlee,
    Projectile,
    Explosion
}
    
public enum EnemyEnhanceType
{
    Normal,
    Special,
    Boss
}
