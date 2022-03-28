using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Old_Weapon : MonoBehaviour
{
    public string currentEquiped;
    
    public float attackRange;
    public float attackRate;
    public float attackDamage;

    public float gunAttackDelay;
    public float gunAttackOffDelay;

    public float reloadDelay;

    public int gunCurrentBullet;
    public int gunCarryBullet;
    public int gunMaxBullet;
    public int gunReloadBullet;

    public Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
}
