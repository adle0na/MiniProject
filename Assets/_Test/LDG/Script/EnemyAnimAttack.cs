using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimAttack : MonoBehaviour
{
    public event Action OnAttackAnim;

    public void AttackEvent() => OnAttackAnim?.Invoke();
}
