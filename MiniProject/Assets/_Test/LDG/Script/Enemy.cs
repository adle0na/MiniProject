using System;
using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private EnemyClass _enemyClass;
    private int playerLayer = 1 << 3;
    public Transform player;
    private Rigidbody rigid;
    private Vector3 dir;
    private bool isDeteted;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        _enemyClass.curHealth = _enemyClass.maxHealth;
        isDeteted = GetPlayerDetected();

        if (_enemyClass.EnemyEnhanceType == EnemyEnhanceType.Special)
        {
            _enemyClass.SpecialEnemy();
        }

        if (_enemyClass.EnemyEnhanceType == EnemyEnhanceType.Boss)
        {
        }
        else
            switch (_enemyClass.EnemyAttackType)
            {
                case EnemyAttackType.Mlee:
                    StartCoroutine(StartMleeAI());
                    break;
                case EnemyAttackType.Explosion:
                    StartCoroutine(StartExplosionAI());
                    break;
                case EnemyAttackType.Projectile:
                    StartCoroutine(StartProjectileAI());
                    break;
                default:
                    Debug.Log("설정 안했네?");
                    break;
            }
    }

    private void FixedUpdate()
    {
        dir = (player.position - transform.position).normalized;
        isDeteted = GetPlayerDetected();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (_enemyClass != null)
            Gizmos.DrawWireSphere(transform.position, _enemyClass.attackRadius);
    }


    #region Private Method

    private IEnumerator StartExplosionAI()
    {
        while (_enemyClass.curHealth > 0)
        {
            if (isDeteted)
            {
                Debug.Log("발견");
                yield return new WaitForSeconds(2);
                Debug.Log("2초 지났고 터짐");
                Destroy(gameObject);
            }
            else
            {
                rigid.MovePosition(rigid.position + dir * _enemyClass.speed * Time.fixedDeltaTime);
            }

            yield return new WaitForFixedUpdate();
        }

        OnEnemyDie();
        yield return null;
    }

    private IEnumerator StartMleeAI()
    {
        while (_enemyClass.curHealth > 0)
        {
            if (isDeteted)
            {
                Debug.Log("공격");
                yield return new WaitForSeconds(_enemyClass.attackDelay);
            }
            else
            {
                rigid.MovePosition(rigid.position + dir * _enemyClass.speed * Time.fixedDeltaTime);
            }

            yield return new WaitForFixedUpdate();
        }

        OnEnemyDie();
        yield return null;
    }

    private IEnumerator StartProjectileAI()
    {
        while (_enemyClass.curHealth > 0)
        {
            if (isDeteted)
            {
                rigid.MovePosition(rigid.position + -dir * _enemyClass.speed * Time.fixedDeltaTime);
            }
            else if (GetPlayerDistance() > _enemyClass.attackRadius * 1.5f)  
            {
                rigid.MovePosition(rigid.position + dir * _enemyClass.speed * Time.fixedDeltaTime);
            }
            else
            {
                Debug.Log("공격");
                GameObject obj = Instantiate(_enemyClass._projectile.prefab, transform.position,
                    Quaternion.LookRotation(dir));
                obj.GetComponent<Rigidbody>().velocity = dir * _enemyClass._projectile.speed;
                Destroy(obj, _enemyClass._projectile.destroyTime);
                yield return new WaitForSeconds(_enemyClass.attackDelay);
            }

            yield return new WaitForFixedUpdate();
        }

        OnEnemyDie();
        yield return null;
    }

    private float GetPlayerDistance() => Vector3.Distance(transform.position, player.position);
    private bool GetPlayerDetected() => GetPlayerDistance() < _enemyClass.attackRadius;

    private void OnEnemyDie()
    {
        foreach (var item in _enemyClass.dropItems)
        {
            Debug.Log($"{item.name} 아이템 드롭");
        }
    }

    #endregion
}