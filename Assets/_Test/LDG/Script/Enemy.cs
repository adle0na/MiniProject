using System;
using System.Collections;
using DG.Tweening;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace _Test.LDG.Script
{
    public class Enemy : MonoBehaviour, IAttackAble
    {
        private static readonly int AttackTrigger = Animator.StringToHash("AttackTrigger");
        private static readonly int DeadTrigger = Animator.StringToHash("DeadTrigger");
        private static readonly int IsRun = Animator.StringToHash("IsRun");

        [SerializeField] private LayerMask attackLayer;
        [SerializeField] private EnemyClass enemyClass;
        [SerializeField] private NavMeshAgent agent;
        [SerializeField] private Animator anim;
        [SerializeField] private EnemyAnimEventer animEventer;
        [SerializeField] private Transform firePoint;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private Image healthImage;

        private Transform target;
        private bool isAttack = false;
        private IDisposable moveCallBack;

        private void Awake()
        {
            enemyClass = Instantiate(enemyClass);
            enemyClass.Initialize();

            agent.speed = enemyClass.Speed;
            
            animEventer.OnDeadAnim += DestroyObject;
            
            GameObject obj = GameObject.FindWithTag("Player");
            if (obj == null) { OnDead(); return; }

            target = obj.transform;
            
            Observable.FromCoroutine(AnimEndChecker)
                .Subscribe(
                    _ => Debug.Log("AnimCheck"),
                    Initialized)
                .AddTo(gameObject);
        }

        private void OnDestroy()
        {
            enemyClass.OnDeaded -= OnDead;
            animEventer.OnAttackAnim -= enemyClass.EnemyType switch
            {
                EnemyType.Melee => MeleeAttack,
                EnemyType.Explosion => MeleeAttack,
                EnemyType.Projectile => ProjectileAttack
            };
            animEventer.OnDeadAnim -= DestroyObject;
        }

        private IEnumerator AnimEndChecker()
        {
            while (!anim.IsInTransition(0))
                yield return null;
        }

        private void Initialized()
        {
            enemyClass.OnDeaded += OnDead;

            switch (enemyClass.EnemyType)
            {
                case EnemyType.Melee: animEventer.OnAttackAnim += MeleeAttack; break;
                case EnemyType.Explosion: animEventer.OnAttackAnim += ExplosionAttack; break;
                case EnemyType.Projectile: animEventer.OnAttackAnim += ProjectileAttack; break;
                case EnemyType.Boss: animEventer.OnAttackAnim += MeleeAttack; break;    // TEST
                default: throw new ArgumentOutOfRangeException();
            }

            Observable.FromCoroutine(EnemyRoutine)
                .Subscribe()
                .AddTo(gameObject);
        }
        
        private IEnumerator EnemyRoutine()
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
                .Subscribe(_ => Stop())
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
                if (collider.TryGetComponent<IAttackAble>(out IAttackAble attackAble))
                    AttackTarget(attackAble);
            }
        }

        private void ProjectileAttack()
        {
            GameObject obj = Instantiate(enemyClass.Projectile.prefab, firePoint.position, transform.rotation);

            ProjectileEventer eventer = obj.GetComponent<ProjectileEventer>();
            eventer.GetRigidBody().velocity = transform.forward * enemyClass.Projectile.speed;
            eventer.OnHitTarget += AttackTarget;
            eventer.OnDestroyProjectile += DisposeProjectileEvent;
        }

        private void ExplosionAttack()
        {
            foreach (var collider in Physics.OverlapSphere(transform.position, enemyClass.AttackRadius, attackLayer))
            {
                if (collider.TryGetComponent<IAttackAble>(out IAttackAble attackAble))
                    AttackTarget(attackAble);
            }
            OnDead();
            
        }

        private void OnDead()
        {
            agent.isStopped = true;
            anim.SetTrigger(DeadTrigger);
        }

        private void OnDrawGizmos()
        {
            if (!isAttack) { return; }

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, enemyClass.AttackRadius);
        }

        private void AttackTarget(IAttackAble attackAble)
        {
            attackAble.TakeDamage(enemyClass.AttackPower);
        }

        private void DisposeProjectileEvent(ProjectileEventer eventer)
        {
            eventer.OnHitTarget -= AttackTarget;
            eventer.OnDestroyProjectile -= DisposeProjectileEvent;
        }


        public void TakeDamage(int damage)
        {
            if(enemyClass.IsDead) { return; }

            enemyClass.HitHealth(damage);
            
            canvasGroup.DOFade(1, 0.1f)
                .OnComplete(() => canvasGroup.DOFade(0, 2f));

            healthImage.DOFillAmount((float) enemyClass.CurHealth / enemyClass.MaxHealth, 0.2f);
        }

        private void DestroyObject()
        {
            Debug.Log("사라짐");
        }

        private float AttackRadius() => Vector3.Distance(transform.position, target.position);

        public EnemyClass GetEnemyClass() => enemyClass;
    }
}