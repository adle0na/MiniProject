using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CenterFrame : MonoBehaviour
{
    // 처음 시작하고 노트가 파괴되면 음악 재생
    #region CenterFrame
        
    // 오디오소스로 startBGM 사용 선언
    AudioSource stageBGM;
    
    // 불변수 음악 시작 상태를 꺼짐으로 선언
    bool musicStart = false;

    // Start is called before the first frame update
    private void Start()
    {
        // 오디오 컴포넌트 사용
        stageBGM = GetComponent<AudioSource>();
        
    }

    // 충돌함수
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 조건문 ( 음악 시작 상태가 꺼져있는 경우 )
        if(!musicStart)
        {
            // 조건문 ( LNote 혹은 RNote와 충돌한 경우)
            if(collision.CompareTag("LNote") || collision.CompareTag("RNote"))
            {
                // BGM 재생
                stageBGM.Play();

                // 음악 시작 상태 켜짐으로
                musicStart = true;
            }

        }
        
    }

    #endregion
}
