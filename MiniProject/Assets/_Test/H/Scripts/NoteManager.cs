using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NoteManager : MonoBehaviour
{   // 노트 생성 및 사라지는 효과, 파괴 관리
    #region NoteManager
        // 변수들
        #region variables

    // 수정가능한 int형 변수 bpm 0으로 선언
    public int bpm = 0;

    // float보다 오차가 적은 double형으로 진행시간 변수 선언
    double currentTime = 0d;

    // 노트 

    // 노트 이미지
    Image noteImage;
    
    // 노트 컬러
    Color clearColor;


    // 생성 위치 선언
    [SerializeField] Transform l_NoteAppear = null; // 왼쪽
    [SerializeField] Transform r_NoteAppear = null; // 오른쪽


    // 번갈아 가면서 생성할 노트 List로 선언
    [SerializeField] List<GameObject> l_Note = new List<GameObject> ();

    [SerializeField] List<GameObject> r_Note = new List<GameObject> ();

    // 번갈아 가면서 생성하기 위한 전역변수
    private int bpmLength;

    TimingManager theTimingManager;

    #endregion

        // 기능 함수들
        #region Functions

    void Start()
    {
        // 노트 이미지, 컬러 사용 선언
        noteImage = GetComponent<Image>();
        clearColor = new Color(1, 1, 1, 0);
        theTimingManager = GetComponent<TimingManager>();
    }
    void Update()
    {
        // 진행 시간을 절대시간만큼 대입연산
        currentTime += Time.deltaTime;

        // 진행 시간이 60을 bpm으로 나눈 값보다 같거나 클경우 
        if(currentTime >= 60d / bpm) // bpm이 120일 경우 2초마다 생성
        {
            // bpm동작 틱마다 증감
            bpmLength++;

            // 번갈아 가면서 소환
            Instantiate(l_Note[bpmLength%2], l_NoteAppear.position, Quaternion.identity,this.transform);

            Instantiate(r_Note[bpmLength%2], r_NoteAppear.position, Quaternion.identity,this.transform);

            theTimingManager.boxNoteList.Add(l_Note[bpmLength%2]);

            // 진행시간에 60을 bpm으로 나눈 값만큼 제거 (오차 제거)
            currentTime -= 60d / bpm;
        }

    }

    // 트리거 충돌종료시
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 태그가 LNote거나 RNote일 경우
        if(collision.CompareTag("LNote") || collision.CompareTag("RNote") )
        {
            theTimingManager.boxNoteList.Remove(l_Note[bpmLength%2]);

            // 파괴
            Destroy(collision.gameObject);
        }

    }

    // 트리거 충돌 진행시
    /*
    private void OnTriggerStay2D(Collider2D collision)
    {
        // 생성된 노트에 noteImage 지정 선언
        noteImage = collision.gameObject.GetComponent<Image>();

        // 태그가 LNote거나 RNote일 경우
        if(collision.CompareTag("LNote") || collision.CompareTag("RNote") )
        {   
            // 이미지를 흐릿하게
            noteImage.color = Color.Lerp(noteImage.color, clearColor, 0.3f);
        }
    }
    */

    #endregion
    #endregion
}
