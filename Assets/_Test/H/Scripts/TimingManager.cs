using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimingManager : MonoBehaviour
{
    // 판정 범위 안에 있는지 판독할 생성되는 노트들
    public List<GameObject> boxNoteList = new List<GameObject>();

    [SerializeField] Transform Center = null;
    
    // 여러가지 판정을 사용해야 해서 배열 선언
    [SerializeField] RectTransform[] timingRect = null;

    // RectTransform배열 삽입할 변수 선언
    Vector2[] timingBoxs = null;

    void Start()
    {   
        // 타이밍 박스 배열의 길이만큼 사용 선언
        timingBoxs = new Vector2[timingRect.Length];

        // 반복문 ( 인덱스 i 값을 타이밍 배열의 길이 전까지 증가)
        for(int i = 0; i < timingRect.Length; i++ )
        {
            // 
            timingBoxs[i].Set(Center.localPosition.x - timingRect[i].rect.width,
                              Center.localPosition.x + timingRect[i].rect.width);
        }
    }

    public void CheckTiming() 
    {
        for(int i = 0; i < boxNoteList.Count; i++)
        {
            float t_notePosX = boxNoteList[i].transform.localPosition.x;

            for(int x = 0; x < timingBoxs.Length; x++)
            {
                if(timingBoxs[x].x <= t_notePosX && t_notePosX <= timingBoxs[x].y)
                {
                    boxNoteList.RemoveAt(i);
                    Debug.Log("Hit" + x);
                    return;
                }
            }
        
        }

        Debug.Log("Miss");
    }
}
