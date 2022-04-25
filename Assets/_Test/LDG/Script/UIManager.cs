using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    private static UIManager instance;
    public static UIManager Instance => instance;

    public GameObject damageTextPrefab;
    
    private void Awake()
    {
        if(instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }


    public void PopDamageText(Vector3 pos, float damage)
    {
        GameObject obj = Instantiate(damageTextPrefab);
        obj.GetComponent<DamageText>().EnableText(pos, damage);
    }
    
}
