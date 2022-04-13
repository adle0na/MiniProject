using System;
using System.Collections;
using DG.Tweening;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.AI;

namespace _Test.LDG.Script
{
    public class Enemy : MonoBehaviour
    {
        private static readonly int AttackTrigger = Animator.StringToHash("AttackTrigger");
        private static readonly int DeadTrigger = Animator.StringToHash("DeadTrigger");
        private static readonly int IsRun = Animator.StringToHash("IsRun");

        [SerializeField] private LayerMask attackLayer;
        [SerializeField] private EnemyClass enemyClass;
        [SerializeField] private NavMeshAgent agent;
        [SerializeField] private Animator anim;
        [SerializeField] private EnemyAnimAttack animAttack;
        [SerializeField] private Transform firePoint;
        [SerializeField] private Transform target;

        private bool isAttack = false;
        private IDisposable moveCallBack;

        private void Awake()
        {
            enemyClass = Instantiate(enemyClass);
            enemyClass.Initialize();

            agent.speed = enemyClass.Speed;

            Observable.FromCoroutine(AnimEndChecker)
                .Subscribe(
                    _ => Debug.Log("AnimCheck"),
                    Initialized);
        }

        private void OnDestroy()
        {
            enemyClass.OnDeaded -= OnDead;
            animAttack.OnAttackAnim -= enemyClass.EnemyType switch
            {
                EnemyType.Melee => MeleeAttack,
                EnemyType.Explosion => MeleeAttack,
                EnemyType.Projectile => ProjectileAttack
            };
        }

        private IEnumerator AnimEndChecker()
        {
            while (!anim.IsInTransition(0))
                yield return null;
        }

        private void Initialized()
        {
            GameObject obj = GameObject.FindWithTag("Player");
            if(obj == null) { OnDead(); return; }
            
            target = obj.transform;

            enemyClass.OnDeaded += OnDead;

            switch (enemyClass.EnemyType)
            {
                case EnemyType.Melee:
                    
                    animAttack.OnAttackAnim += MeleeAttack;
                    
                    Observable.FromCoroutine(MeleeEnemyRoutine)
                        .Subscribe()
                        .AddTo(gameObject);
                    
                    break;
                case EnemyType.Explosion:
                    
                    animAttack.OnAttackAnim += ExplosionAttack;
                    
                    Observable.FromCoroutine(ExplosionEnemyRoutine)
                        .Subscribe()
                        .AddTo(gameObject);
                    
                    break;
                case EnemyType.Projectile:
                    
                    animAttack.OnAttackAnim += ProjectileAttack;
                    
                    Observable.FromCoroutine(ProjectileEnemyRoutine)
                        .Subscribe()
                        .AddTo(gameObject);
                    
                    break;
                case EnemyType.Boss:
                    
                    break;
                default: throw new ArgumentOutOfRangeException();
            }
        }

        private IEnumerator MeleeEnemyRoutine()
        {
            while (!enemyClass.IsDead)
            {
                yield return null;
                
                if (isAttack) { continue; }

                if (AttackRadius() < enemyClass.AttackRadius) 
                    StartAttackMotion();
                else
                    GoTo(target.position);
            }
        }
        
        private IEnumerator ProjectileEnemyRoutine()
        {
            while (!enemyClass.IsDead)
            {
                yield return null;
                
                if (isAttack) { continue; }
                
                if (AttackRadius() < enemyClass.AttackRadius)
                    StartAttackMotion();
                else
                    GoTo(target.position);
                
            }
        }
        
        private IEnumerator ExplosionEnemyRoutine()
        {
            while (!enemyClass.IsDead)
            {
                yield return null;
                
                if (isAttack) { continue; }
                
                if (AttackRadius() < enemyClass.AttackRadius)
                    StartAttackMotion();
                else
                    GoTo(target.position);
                
            }
        }

        private void StartAttackMotion()
        {
            transform.LookAt(target.position);
            
            isAttack = true;

            anim.SetTrigger(AttackTrigger);

            agent.ResetPath();
            
            Observable.Timer(TimeSpan.FromSeconds(enemyClass.AttackDelay))
                .Subscribe(_ => isAttack = false);
        }

        private void GoTo(Vector3 pos)
        {
            moveCallBack?.Dispose();

            anim.SetBool(IsRun, true);
            
            agent.SetDestination(pos);

            moveCallBack = this.UpdateAsObservable()
                .Select(_ => agent.remainingDistance)
                .Where(x => x < agent.stoppingDistance)
                .Subscribe(_=> Stop())
                .AddTo(gameObject);
        }
        
        private void Stop()
        {
            moveCallBack?.Dispose();
            
            anim.SetBool(IsRun, false);
            
            agent.ResetPath();
        }

        private void MeleeAttack()
        {
            foreach (var collider in Physics.OverlapSphere(transform.position, enemyClass.AttackRadius, attackLayer))
            {
                Debug.Log($"{collider.name}에게 {enemyClass.AttackPower}피해를 입혔다");
            }
        }

        private void ProjectileAttack()
        {
            GameObject obj = Instantiate(enemyClass.Projectile.prefab, firePoint.position, transform.rotation);
            obj.GetComponent<Rigidbody>().velocity = transform.forward * enemyClass.Projectile.speed;
        }

        private void ExplosionAttack()
        {
            foreach (var collider in Physics.OverlapSphere(transform.position, enemyClass.AttackRadius, attackLayer))
            {
                Debug.Log($"{collider.name}에게 {enemyClass.AttackPower}피해를 입혔다");
            }
        }
        
        private float AttackRadius() => Vector3.Distance(transform.position, target.position);

        private void OnDead()
        { 
            anim.SetTrigger(DeadTrigger);
        }

        private void OnDrawGizmos()
        {
            if(!isAttack) { return; }
            
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, enemyClass.AttackRadius);
        }
    }
}