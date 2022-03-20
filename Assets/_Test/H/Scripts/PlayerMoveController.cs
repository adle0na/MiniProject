using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using System;

public class PlayerMoveController : MonoBehaviour
{
    #region variables


    public float dodgeForce;
    public float jumpForce;
    private KeyCode m_FirstButtonPressed;
    private float m_TimeOfFirstButton;
    private bool m_Reset;

    private bool dodgePossible;

    [SerializeField]
    private float walkSpeed;

    [SerializeField]
    private float runSpeed;
    private float applySpeed;

    [SerializeField]
    private float lookSensitivity;

    [SerializeField]
    private float cameraRotationLimit;
    private float currentCameraRotationX = 0f;

    private bool isRun = false;
    private bool isCrouch = false;
    private bool isGround = true;

    [SerializeField]
    private float crouchSpeed;

    [SerializeField]
    private float crouchPosY;
    private float originPosY;

    private float applyCrouchPosY;

    [SerializeField]
    private Camera theCamera;
    private Rigidbody playerRb;
    private CapsuleCollider capsuleCollider;

    #endregion

    #region Functions
    void Start()
    {
        capsuleCollider = GetComponent<CapsuleCollider>();

        playerRb = GetComponent<Rigidbody>();

        applySpeed = walkSpeed;
        originPosY = theCamera.transform.localPosition.y;
        applyCrouchPosY = originPosY;

    }

    void Update()
    {

        IsGround();
        TryJump();
        TryRun(); 
        TryCrouch();
        Move();
        CameraRotation();
        CharactorRotation();

        if(checkDoubleTap(KeyCode.A) && dodgePossible)
        {
            dodge(KeyCode.A);
        }

        if(checkDoubleTap(KeyCode.W) && dodgePossible)
        {
            dodge(KeyCode.W);
        }

        if(checkDoubleTap(KeyCode.D) && dodgePossible)
        {
            dodge(KeyCode.A);
        }

        if(checkDoubleTap(KeyCode.S) && dodgePossible)
        {
            dodge(KeyCode.A);
        }

    }

    private void dodge(KeyCode directionKey)
    {
        float force = dodgeForce;

        if(!isGround)
        {
            force = force / 5;
        }

        Vector3 directionVector = findVectorForDirection(directionKey);
        playerRb.drag = 0f;

        playerRb.AddForce(new Vector3(0f, jumpForce / 2, 0f), ForceMode.Impulse);
        playerRb.AddForce(-new Vector3(directionVector.x * force, directionVector.y * force, directionVector.z * force), ForceMode.Impulse);
        dodgePossible = false;
    }

    private Vector3 findVectorForDirection(KeyCode directionKey)
    {
        if (directionKey == KeyCode.W)
        {
            return transform.forward;
        }

        if (directionKey == KeyCode.S)
        {
            return -transform.forward;
        }

        Vector3 up = new Vector3(0.0f, 1.0f, 0.0f);
        Vector3 left = Vector3.Cross(transform.forward.normalized, up.normalized);

        if(directionKey == KeyCode.A)
        {
            return left;
        }

        return -left;
    }


    private void TryRun()
    {

        if (Input.GetKey(KeyCode.LeftShift))
        {

            Running();
        }

        if (Input.GetKeyUp(KeyCode.LeftShift))
        {

            RunningCancle();
        }
    }

    private void TryCrouch()
    {  
        if (Input.GetKeyDown(KeyCode.LeftAlt) && isGround)
        {

            Crouch();

        }
    }

    private bool checkDoubleTap(KeyCode key)
    {
        if (Input.GetKeyDown(key) && m_FirstButtonPressed == key)
        {
            m_FirstButtonPressed = KeyCode.O;

            if (Time.time - m_TimeOfFirstButton < 0.5f)
            {
                return true;
            }
        }

        if(Input.GetKeyDown(key) && m_FirstButtonPressed != key)
        {
            m_FirstButtonPressed = key;
            m_TimeOfFirstButton = Time.time;
            return false;
        }

        return false;
    }


    private void Crouch()
    {

        isCrouch = !isCrouch;


        if (isCrouch)
        {

            applySpeed = crouchSpeed;
            applyCrouchPosY = crouchPosY;
        }
        else
        {
            applySpeed = walkSpeed;
            applyCrouchPosY = originPosY;
        }

        StartCoroutine(CrouchCoroutine());

    }
    

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

    private void IsGround()
    {
        isGround = Physics.Raycast(transform.position, Vector3.down, capsuleCollider.bounds.extents.y + 0.1f);
        dodgePossible = true;
    }

    private void TryJump()
    {
        if (Input.GetKeyDown(KeyCode.F) && isGround)
        {
            Jump();
        }
    }

    private void Jump()
    {
        if (isCrouch)
            Crouch();

        playerRb.velocity = transform.up * jumpForce;
    }


    private void Running()
    {
        if (isCrouch)
            Crouch();

        isRun = true;
 
        applySpeed = runSpeed;
    }


    private void RunningCancle()
    {

        isRun = false;

        applySpeed = walkSpeed;
    }


    private void Move()
    {
    
        float _moveDirX = Input.GetAxisRaw("Horizontal");

        float _moveDirZ = Input.GetAxisRaw("Vertical");

        Vector3 _moveHorizontal = transform.right * _moveDirX;
        Vector3 _moveVertical = transform.forward * _moveDirZ;

        Vector3 _velocity = (_moveHorizontal + _moveVertical).normalized * applySpeed;

        playerRb.MovePosition(transform.position + _velocity * Time.deltaTime);
    }

    private void CameraRotation()
    {

        float _xRotation = Input.GetAxisRaw("Mouse Y");

        float _cameraRotationX = _xRotation * lookSensitivity;

        currentCameraRotationX -= _cameraRotationX;
 
        currentCameraRotationX = Mathf.Clamp(currentCameraRotationX, -cameraRotationLimit, cameraRotationLimit);


        theCamera.transform.localEulerAngles = new Vector3(currentCameraRotationX, 0f, 0f);
    }

    private void CharactorRotation()
    {

        float _yRotation = Input.GetAxisRaw("Mouse X");

        Vector3 _charactorRotationY = new Vector3(0f, _yRotation, 0f) * lookSensitivity;

        playerRb.MoveRotation(playerRb.rotation * Quaternion.Euler(_charactorRotationY));
    }

    #endregion

}