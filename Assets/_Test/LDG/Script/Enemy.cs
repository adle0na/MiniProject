using System;
using System.Collections;
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
        
        [SerializeField] private EnemyClass enemyClass;
        [SerializeField] private NavMeshAgent agent;
        [SerializeField] private Animator anim;
        
        [SerializeField] private Transform testTarget;

        [SerializeField] private bool isAttack = false;

        private IDisposable moveCallBack;

        private void Awake()
        {
            enemyClass = Instantiate(enemyClass);
            enemyClass.Initialize();
            enemyClass.OnDeaded += OnDead;
            
            Observable.FromCoroutine(AnimEndChecker)
                .Subscribe(
                    _ => Debug.Log("AnimCheck"),
                    Initialized);
        }
        
        private IEnumerator AnimEndChecker()
        {
            while (!anim.IsInTransition(0))
                yield return null;
        }

        private void Initialized()
        {
            switch (enemyClass.EnemyType)
            {
                case EnemyType.Melee:
                    Observable.FromCoroutine(MeleeEnemyRoutine)
                        .Subscribe()
                        .AddTo(gameObject);
                    break;
                case EnemyType.Explosion:
                    
                    break;
                case EnemyType.Projectile:
                    
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

                if(InAttackRadius())
                    StartAttackMotion();
                else
                    GoTo(testTarget.position);
            }
        }

        private void StartAttackMotion()
        {
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

        private bool InAttackRadius() => Vector3.Distance(transform.position, testTarget.position) < enemyClass.AttackRadius;

        private void OnDead()
        { 
            anim.SetTrigger(DeadTrigger);
        }
    }
}