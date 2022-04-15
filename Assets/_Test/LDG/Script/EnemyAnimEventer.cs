using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimEventer : MonoBehaviour
{
    public event Action OnAttackAnim;
    public event Action OnDeadAnim;
    public void AttackEvent() => OnAttackAnim?.Invoke();
    public void DeadEvent() => OnDeadAnim?.Invoke();
}
