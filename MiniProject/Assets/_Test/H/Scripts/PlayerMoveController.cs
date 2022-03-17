using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveController : MonoBehaviour
{
    // ������
    #region variables

    // �ӵ�����
    [SerializeField]
    private float walkSpeed;

    [SerializeField]
    private float runSpeed;
    private float applySpeed;

    // �ΰ���
    [SerializeField]
    private float lookSensitivity;

    // ī�޶� ����
    [SerializeField]
    private float cameraRotationLimit;
    private float currentCameraRotationX = 0f;

    // ���� ����
    private bool isRun = false;
    private bool isCrouch = false;
    private bool isGround = true;

    // �������� �ӵ�
    [SerializeField]
    private float crouchSpeed;

    // �ɴ� �ӵ� ����
    [SerializeField]
    private float crouchPosY;
    private float originPosY;

    private float applyCrouchPosY;

    // �����ϴ� ��
    [SerializeField]
    private float jumpForce;


    // ��� ������Ʈ
    [SerializeField]
    private Camera theCamera;
    private Rigidbody playerRb;
    private CapsuleCollider capsuleCollider;

    #endregion

    // �Լ���
    #region Functions
    void Start()
    {
        capsuleCollider = GetComponent<CapsuleCollider>();
        // ���� �� ��� ����
        playerRb = GetComponent<Rigidbody>();
        // �ӵ��� ��ȯ ���
        applySpeed = walkSpeed;
        originPosY = theCamera.transform.localPosition.y;
        applyCrouchPosY = originPosY;
    }

    void Update()
    {
        // �Է°� �Լ� ȣ��
        IsGround();
        TryJump();
        TryRun(); // �ٴ��� Ȯ���� ���� �ݵ�� Move()�Լ����� ���� ����
        TryCrouch();
        Move();
        CameraRotation();
        CharactorRotation();
    }

    // �޸����� Ȯ�� �Լ�
    private void TryRun()
    {
        // LeftShiftŰ �Է½�
        if (Input.GetKey(KeyCode.LeftShift))
        {
            // �޸��� �Լ� ����
            Running();
        }
        // LeftShiftŰ �ߴܽ�
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            // �޸��� ���� �Լ� ����
            RunningCancle();
        }
    }

    // �ɱ� ���� �Լ�
    private void TryCrouch()
    {   // LeftControlŰ�� ������ ���� ������
        if (Input.GetKeyDown(KeyCode.LeftControl) && isGround)
        {
            // ���̱� ����
            Crouch();

        }
    }

    // ���̱� �Լ�
    private void Crouch()
    {
        // Toggle ���
        isCrouch = !isCrouch; // true�� false�� false�� true��

        // ���ǹ� ���λ��¶��
        if (isCrouch)
        {
            // �̵��ӵ��� �þ߸� ���ΰ�����
            applySpeed = crouchSpeed;
            applyCrouchPosY = crouchPosY;
        }
        else // ���� ���°� �ƴ϶��
        {
            // �Ϲ� �̵��ӵ��� ���� �þ߷�
            applySpeed = walkSpeed;
            applyCrouchPosY = originPosY;
        }
        // ���̱� �ڷ�ƾ �Լ� ����
        StartCoroutine(CrouchCoroutine());

    }

    // �ε巯�� ���̱�
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

    // ���� Ȯ��
    private void IsGround()
    {
        isGround = Physics.Raycast(transform.position, Vector3.down, capsuleCollider.bounds.extents.y + 0.1f);
    }
    // ���� Ű �Է¹޾����� ����
    private void TryJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGround)
        {
            Jump();
        }
    }
    // ���� �Լ�
    private void Jump()
    {
        if (isCrouch)
            Crouch();

        playerRb.velocity = transform.up * jumpForce;
    }

    // �޸��� �Լ�
    private void Running()
    {
        if (isCrouch)
            Crouch();
        // �޸��� ���� On
        isRun = true;
        // �ӵ��� = �޸��� �ӵ�
        applySpeed = runSpeed;
    }

    // �޸��� �ߴ� �Լ�
    private void RunningCancle()
    {
        // �޸��� ���� Off
        isRun = false;
        // �ӵ��� = �ȴ� �ӵ�
        applySpeed = walkSpeed;
    }


    private void Move()
    {
        // �� �� ����Ű �Է°��� ����
        float _moveDirX = Input.GetAxisRaw("Horizontal");
        // �� �� ����Ű �Է°��� ����
        float _moveDirZ = Input.GetAxisRaw("Vertical");

        // �Է¹��� ���� ���� ���Ͱ� ����
        Vector3 _moveHorizontal = transform.right * _moveDirX;
        Vector3 _moveVertical = transform.forward * _moveDirZ;

        // ���Ͱ��� ������ normalize�� 1�� ����ȭ
        Vector3 _velocity = (_moveHorizontal + _moveVertical).normalized * applySpeed;
        // �÷��̾��� ���� ��ü ������ ����
        playerRb.MovePosition(transform.position + _velocity * Time.deltaTime);
    }

    // ī�޶� ȸ���� �Լ� (����)
    private void CameraRotation()
    {
        // ���콺�� ���Ʒ��� �����Ҽ� �ִ� ȸ����X �Է� ���� ����
        float _xRotation = Input.GetAxisRaw("Mouse Y");
        // ī�޶� ������ �޾ƿ� ���� * ����
        float _cameraRotationX = _xRotation * lookSensitivity;
        // ���� ��� ������ ī�޶� ȸ������ �� �� (������ �ݴ��̱� ������)
        currentCameraRotationX -= _cameraRotationX;
        // ���� ��� ������ ��°����� �ּ� �ִ� ������ ����
        currentCameraRotationX = Mathf.Clamp(currentCameraRotationX, -cameraRotationLimit, cameraRotationLimit);

        // ���ݱ��� ����� ���� ����Ͽ� ���� Vector�� X�ุ ����
        theCamera.transform.localEulerAngles = new Vector3(currentCameraRotationX, 0f, 0f);
    }

    // ĳ���� ȸ���� �Լ� (�¿�)
    private void CharactorRotation()
    {
        // ���콺�� �¿� ���� �Է¹���
        float _yRotation = Input.GetAxisRaw("Mouse X");
        // ĳ������ ������ �޾ƿ� Y�ప * ����
        Vector3 _charactorRotationY = new Vector3(0f, _yRotation, 0f) * lookSensitivity;
        // ���� ������� ĳ������ ��ü ���� ȸ�� ( ����� Quaternion���Ϸ� ������ )
        playerRb.MoveRotation(playerRb.rotation * Quaternion.Euler(_charactorRotationY));
    }

    #endregion

}