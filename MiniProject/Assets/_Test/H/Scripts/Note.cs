using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Note : MonoBehaviour
{
    // 노트 이동 및 속도 관리
    #region Note
    // 노트 이동속도
    public float noteSpeed = 400;

    UnityEngine.UI.Image noteImage;

    void Start()
    {
        noteImage = GetComponent<UnityEngine.UI.Image>();
    }

    void Update()
    {   
        // 태그가 LNote일경우 우측 이동 RNote일경우 좌측 이동
        if (CompareTag("LNote"))
        {
            transform.localPosition += Vector3.right * noteSpeed * Time.deltaTime; 
        }
        else if(CompareTag("RNote"))
        {
            transform.localPosition += Vector3.left * noteSpeed * Time.deltaTime; 
        }
        
    }

    public void HideNote()
    {
        noteImage.enabled = false;
    }
    #endregion
}
