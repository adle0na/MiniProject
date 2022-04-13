using System.Collections;
using System.Collections.Generic;
using _Test.LDG.Script;
using UnityEngine;

public class WeaponMeleeCollider : MonoBehaviour
{
    // [SerializeField] private ImpactMemoryPool impactMemoryPool;
    [SerializeField] private Transform knifeTransform;

    private new Collider collider;
    private int damage;

    private void Awake()
    {
        collider = GetComponent<Collider>();
        collider.enabled = false;
    }

    public void StartCollider(int damage)
    {
        this.damage = damage;
        collider.enabled = true;

        StartCoroutine(DisablebyTime(0.1f));
    }

    private IEnumerator DisablebyTime(float time)
    {
        yield return new WaitForSeconds(time);
        collider.enabled = false;
    }

    // 칼 콜라이더와 다른게 닿았을 때 이벤트. 필요에 따라 수정
    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent<IAttackAble>(out IAttackAble attackAble))
            attackAble.TakeDamage(damage);
    }
}
