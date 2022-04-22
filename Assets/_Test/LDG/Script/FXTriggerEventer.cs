using System;
using _Test.LDG.Script;
using UnityEngine;

namespace _Test.H.Scripts
{
    public class FXTriggerEventer : MonoBehaviour
    {
        [SerializeField] private int attackPower = 10;
        
        private void OnTriggerEnter(Collider other)
        {
            if(!other.CompareTag("Player")) { return; }
            
            if (other.TryGetComponent<IAttackAble>(out IAttackAble attackAble))
            {
                attackAble.TakeDamage(attackPower);
            }
        }
    }
}