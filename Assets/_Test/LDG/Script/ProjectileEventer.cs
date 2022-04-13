using System;
using _Test.LDG.Script;
using UnityEngine;

public class ProjectileEventer : MonoBehaviour
{
    [SerializeField] private string targetTag;
    [SerializeField] private Rigidbody rigid;
    [SerializeField] private float penetrate;
    
    public event Action<IAttackAble> OnHitTarget;
    public event Action<ProjectileEventer> OnDestroyProjectile;
    
    public Rigidbody GetRigidBody() => rigid;
    
    private void OnTriggerEnter(Collider other)
    {
        if(!other.CompareTag(targetTag)) { return; }

        if (!other.TryGetComponent<IAttackAble>(out IAttackAble attackAble)) return;
        
        OnHitTarget?.Invoke(attackAble);
        
        if (!(--penetrate < 0)) return;
        
        OnDestroyProjectile?.Invoke(this);
        Destroy(gameObject);
    }
}
