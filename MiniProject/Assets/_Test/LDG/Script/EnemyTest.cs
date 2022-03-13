using System;
using System.Collections;
using UnityEngine;

public class EnemyTest : MonoBehaviour
{
    public enum EnemyAttackType
    {
        Mlee,
        Projectile,
        Speed,
        Explosion,
    }
    
    public enum EnemyEnhanceType
    {
        Normal,
        Special,
        Boss
    }

    [Tooltip("공격 타입")]
    public EnemyAttackType AttackType;
    [Tooltip("강화 타입")]
    public EnemyEnhanceType EnemyType;
    
    private int health;
    private int damage;
    private float attackDelay;
    public float attackRaius = 2;
    private float speed;
    private bool isDropItem;

    private float enchantScale = 1;
    
    public GameObject[] dropItem;
    public Transform player;
    private Rigidbody rigid;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();

        TypeConfig();
        SetStatus(enchantScale);
    }

    private void Start()
    {
        StartCoroutine(StartAI());
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRaius);
    }

    private IEnumerator StartAI()
    {
        RaycastHit hit;
        int playerLayer = 1 << 3;
        Vector3 dir;

        while (health > 0)
        {
            dir = (player.position - transform.position).normalized;
            switch (AttackType)
            {
                case EnemyAttackType.Mlee:

                    if (Vector3.Distance(transform.position, player.position) < attackRaius)
                    {
                        Debug.Log("공격");
                        yield return new WaitForSeconds(attackDelay);
                    }
                    else
                    {
                        rigid.MovePosition(rigid.position + dir * speed * Time.fixedDeltaTime);
                    }
                    break;
                case EnemyAttackType.Explosion:
                    if (Vector3.Distance(transform.position, player.position) < attackRaius)
                    {
                        Debug.Log("발견");
                        yield return new WaitForSeconds(2);
                        Debug.Log("2초 지났고 터짐");
                        Destroy(gameObject);
                    }
                    else
                    {
                        rigid.MovePosition(rigid.position + dir * speed * Time.fixedDeltaTime);
                    }
                    break;
            }

            yield return new WaitForFixedUpdate();
        }
        if (isDropItem)
        {
            Debug.Log("아이템 떨굼");
        }
    }

    #region Private Method

    private void TypeConfig()
    {
        switch (EnemyType)
        {
            case EnemyEnhanceType.Normal:
                enchantScale = 1;
                break;
            case EnemyEnhanceType.Special:
                enchantScale = 1.5f;
                isDropItem = true;
                break;
            case EnemyEnhanceType.Boss:
                enchantScale = 100;
                isDropItem = true;
                break;
        }
        switch (AttackType)
        {
            case EnemyAttackType.Mlee:
                health = 20;
                damage = 10;
                attackDelay = 1;
                speed = 3;
                break;
            case EnemyAttackType.Projectile:
                health = 15;
                damage = 10;
                attackDelay = 2;
                speed = 4;
                break;
            case EnemyAttackType.Speed:
                health = 15;
                damage = 10;
                attackDelay = 1;
                speed = 6;
                break;
            case EnemyAttackType.Explosion:
                health = 10;
                damage = 20;
                speed = 5;
                break;
        }
    }
    private void SetStatus(float scale)
    {
        health = Mathf.RoundToInt(health * scale);
    }

    #endregion

    
}
