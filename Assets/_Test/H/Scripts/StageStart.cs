using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageStart : MonoBehaviour
{
    [SerializeField] GameObject player = null;

    public bool inStage = false;

    void Awake()
    {
        inStage = false;
    }
    private void OnTriggerStay(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (collision.GetComponent<PlayerController>().gameObject)
            {
                if (inStage == false)
                {
                    Debug.Log("1스테이지 대기중");
                    inStage = true;
                }

            }

        }


    }
}