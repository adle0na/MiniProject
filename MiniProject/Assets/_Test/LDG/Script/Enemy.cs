using System;
using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private EnemyClass _enemyClass;

    public Transform player;
    private Rigidbody rigid;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        _enemyClass.curHealth = _enemyClass.maxHealth;
        
        StartCoroutine(StartAI());
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (_enemyClass != null) 
            Gizmos.DrawWireSphere(transform.position, _enemyClass.attackRadius);
    }

    private IEnumerator StartAI()
    {
        RaycastHit hit;
        int playerLayer = 1 << 3;
        Vector3 dir;
        
        if(_enemyClass.EnemyEnhanceType == EnemyEnhanceType.Special)
            _enemyClass.SpecialEnemy();

        while (_enemyClass.curHealth > 0)
        {
            dir = (player.position - transform.position).normalized;
            switch (_enemyClass.EnemyAttackType)
            {
                case EnemyAttackType.Mlee:

                    if (Vector3.Distance(transform.position, player.position) < _enemyClass.attackRadius)
                    {
                        Debug.Log("공격");
                        yield return new WaitForSeconds(_enemyClass.attackDelay);
                    }
                    else
                    {
                        rigid.MovePosition(rigid.position + dir * _enemyClass.speed * Time.fixedDeltaTime);
                    }

                    break;
                case EnemyAttackType.Explosion:
                    if (Vector3.Distance(transform.position, player.position) < _enemyClass.attackRadius)
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
                    break;
            }

            yield return new WaitForFixedUpdate();
        }

        foreach (var item in _enemyClass.dropItems)
        {
            Debug.Log($"{item.name} 아이템 드롭");
        }
    }

    #region Private Method
    

    #endregion
}