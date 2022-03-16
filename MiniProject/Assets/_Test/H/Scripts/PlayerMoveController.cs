using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveController : MonoBehaviour
{
    // 변수들
    #region variables

    // 속도변수
    [SerializeField]
    private float walkSpeed;

    [SerializeField]
    private float runSpeed;
    private float applySpeed;

    // 민감도
    [SerializeField]
    private float lookSensitivity;

    // 카메라 각도
    [SerializeField]
    private float cameraRotationLimit;
    private float currentCameraRotationX = 0f;

    // 상태 변수
    private bool isRun = false;
    private bool isCrouch = false;
    private bool isGround = true;

    // 앉은상태 속도
    [SerializeField]
    private float crouchSpeed;

    // 앉는 속도 변수
    [SerializeField]
    private float crouchPosY;
    private float originPosY;

    private float applyCrouchPosY;

    // 점프하는 힘
    [SerializeField]
    private float jumpForce;


    // 사용 컴포넌트
    [SerializeField]
    private Camera theCamera;
    private Rigidbody playerRb;
    private CapsuleCollider capsuleCollider;

    #endregion

    // 함수들
    #region Functions
    void Start()
    {
        capsuleCollider = GetComponent<CapsuleCollider>();
        // 물리 값 사용 선언
        playerRb = GetComponent<Rigidbody>();
        // 속도값 변환 출력
        applySpeed = walkSpeed;
        originPosY = theCamera.transform.localPosition.y;
        applyCrouchPosY = originPosY;
    }

    void Update()
    {
        // 입력값 함수 호출
        IsGround();
        TryJump();
        TryRun(); // 뛰는지 확인을 위해 반드시 Move()함수보다 위에 선언
        TryCrouch();
        Move();
        CameraRotation();
        CharactorRotation();
    }

    // 달리는지 확인 함수
    private void TryRun()
    {
        // LeftShift키 입력시
        if (Input.GetKey(KeyCode.LeftShift))
        {
            // 달리기 함수 실행
            Running();
        }
        // LeftShift키 중단시
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            // 달리기 종료 함수 실행
            RunningCancle();
        }
    }

    // 앉기 실행 함수
    private void TryCrouch()
    {   // LeftControl키가 눌리고 땅에 있을때
        if (Input.GetKeyDown(KeyCode.LeftControl) && isGround)
        {
            // 숙이기 실행
            Crouch();

        }
    }

    // 숙이기 함수
    private void Crouch()
    {
        // Toggle 기능
        isCrouch = !isCrouch; // true면 false로 false면 true로

        // 조건문 숙인상태라면
        if (isCrouch)
        {
            // 이동속도와 시야를 숙인값으로
            applySpeed = crouchSpeed;
            applyCrouchPosY = crouchPosY;
        }
        else // 숙인 상태가 아니라면
        {
            // 일반 이동속도와 원래 시야로
            applySpeed = walkSpeed;
            applyCrouchPosY = originPosY;
        }
        // 숙이기 코루틴 함수 실행
        StartCoroutine(CrouchCoroutine());

    }

    // 부드러운 숙이기
    IEnumerator CrouchCoroutine()
    {
        float _posY = theCamera.transform.localPosition.y;
        int count = 0;

        while (_posY != applyCrouchPosY)
        {
            count++;
            _posY = Mathf.Lerp(_posY, applyCrouchPosY, 0.2f);
            theCamera.transform.localPosition = new Vector3(0, _posY, 0);
            if (count > 15)
                break;
            yield return null;
        }
        theCamera.transform.localPosition = new Vector3(0, applyCrouchPosY, 0f);

    }

    // 착지 확인
    private void IsGround()
    {
        isGround = Physics.Raycast(transform.position, Vector3.down, capsuleCollider.bounds.extents.y + 0.1f);
    }
    // 점프 키 입력받았을때 실행
    private void TryJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGround)
        {
            Jump();
        }
    }
    // 점프 함수
    private void Jump()
    {
        if (isCrouch)
            Crouch();

        playerRb.velocity = transform.up * jumpForce;
    }

    // 달리기 함수
    private void Running()
    {
        if (isCrouch)
            Crouch();
        // 달리기 상태 On
        isRun = true;
        // 속도값 = 달리기 속도
        applySpeed = runSpeed;
    }

    // 달리기 중단 함수
    private void RunningCancle()
    {
        // 달리기 상태 Off
        isRun = false;
        // 속도값 = 걷는 속도
        applySpeed = walkSpeed;
    }


    private void Move()
    {
        // 좌 우 방향키 입력값을 받음
        float _moveDirX = Input.GetAxisRaw("Horizontal");
        // 앞 뒤 방향키 입력값을 받음
        float _moveDirZ = Input.GetAxisRaw("Vertical");

        // 입력받은 값에 따라 벡터값 조정
        Vector3 _moveHorizontal = transform.right * _moveDirX;
        Vector3 _moveVertical = transform.forward * _moveDirZ;

        // 벡터값의 기준을 normalize로 1로 정규화
        Vector3 _velocity = (_moveHorizontal + _moveVertical).normalized * applySpeed;
        // 플레이어의 물리 몸체 움직임 조정
        playerRb.MovePosition(transform.position + _velocity * Time.deltaTime);
    }

    // 카메라 회전값 함수 (상하)
    private void CameraRotation()
    {
        // 마우스로 위아래를 조정할수 있는 회전축X 입력 방향 받음
        float _xRotation = Input.GetAxisRaw("Mouse Y");
        // 카메라 각도는 받아온 방향 * 감도
        float _cameraRotationX = _xRotation * lookSensitivity;
        // 최종 출력 각도는 카메라 회전값을 뺀 값 (실제로 반대이기 때문에)
        currentCameraRotationX -= _cameraRotationX;
        // 최종 출력 각도는 출력각도와 최소 최대 각도로 구성
        currentCameraRotationX = Mathf.Clamp(currentCameraRotationX, -cameraRotationLimit, cameraRotationLimit);

        // 지금까지 계산한 값을 사용하여 실제 Vector값 X축만 조정
        theCamera.transform.localEulerAngles = new Vector3(currentCameraRotationX, 0f, 0f);
    }

    // 캐릭터 회전값 함수 (좌우)
    private void CharactorRotation()
    {
        // 마우스로 좌우 값을 입력받음
        float _yRotation = Input.GetAxisRaw("Mouse X");
        // 캐릭터의 각도는 받아온 Y축값 * 감도
        Vector3 _charactorRotationY = new Vector3(0f, _yRotation, 0f) * lookSensitivity;
        // 최종 출력으로 캐릭터의 몸체 각도 회전 ( 계산은 Quaternion오일러 값으로 )
        playerRb.MoveRotation(playerRb.rotation * Quaternion.Euler(_charactorRotationY));
    }

    #endregion

}