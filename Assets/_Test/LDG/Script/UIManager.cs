using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class UIManager : MonoBehaviour
{
    private static UIManager instance;
    public static UIManager Instance => instance;

    private float test = 1;

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
        pos.y = Random.Range(pos.y - test, pos.y + test);
        
        GameObject obj = Instantiate(damageTextPrefab);
        obj.GetComponent<DamageText>().EnableText(pos, damage);
    }
    
}
