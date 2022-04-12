using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class WeaponHandGun : WeaponBase
{
    // 발사시 총구에서 muzzle effect를 내기 위한 변수. 필요시 해제 후 이펙트 넣기
    // [Header("Fire Effects")] [SerializeField]
    // private GameObject muzzleFlashEffect; // 총구 이펙트 on/off
    
    [Header("Spawn Points")] [SerializeField]
    private Transform casingSpawnPoint; // 탄피 생성 위치
    [SerializeField] private Transform bulletSpawnPoint; // 총알 생성 위치
    
    // 발사, 재장전 등 사운드 재생을 위한 변수. 필요시 해제 후 오디오 클립 할당
    [Header("Audio Clips")] [SerializeField]
    private AudioClip audioClipTakeOutWeapon; // 무기 장착 사운드
    [SerializeField] private AudioClip audioClipFire; // 공격 사운드
    [SerializeField] private AudioClip audioClipReload; // 재장전 사운드

    [Header("Aim UI")] [SerializeField] private Image imageAim; // default/aim 모드에 따라 Aim 이미지 활성/비활성

    private bool _isModeChange = false; // 모드 전환 여부 체크용
    private float defaultModeFOV = 60; // 기본모드에서의 카메라 FOV
    private float aimModeFOV = 30; // Aim모드에서의 카메라 FOW
    
    //private ImpactMemoryPool impactMemoryPool; // 공격 효과 생성 후 활성/비활성 관리
    private Camera _mainCamera; // 광선 발사

    private void Awake()
    {
        // 기반 클래스의 초기화를 위한 Setup() 메소드 호출
        base.SetUp();
        
        // casingMemoryPool = GetComponent<CasingMemoryPool>();
        // impactMemoryPool = GetComponent<ImpactMemoryPool>();
        _mainCamera = Camera.main;

        // 처음 탄 수는 최대로 설정
        weaponSetting.currentAmmo = weaponSetting.maxAmmo;
    }

    private void OnEnable()
    {
        // // 무기 장착 사운드 재생
        PlaySound(audioClipTakeOutWeapon);
        // // 총구 이펙트 오브젝트 비활성화
        // muzzleFlashEffect.SetActive(false);

        // 무기가 활성화될 때 해당 무기의 탄 수 정보를 갱신한다
        onAmmoEvent.Invoke(weaponSetting.currentAmmo, weaponSetting.maxAmmo);

        ResetVariables();
    }

    public override void StartWeaponAction(int type = 0)
    {
        // 재장전 중일 때는 무기 액션을 할 수 없다
        if (isReload) return;
        
        // 모드 전환중이면 무기 액션을 할 수 없다
        if (_isModeChange) return;
        
        // 마우스 왼쪽 클릭 (공격 시작)
        if (type == 0)
        {
            // 연속 공격
            if (weaponSetting.isAutomaticAttack == true)
            {
                isAttack = true;
                StartCoroutine("OnAttackLoop");
            }
            // 단발 공격
            else
            {
                OnAttack();
            }
        }
        // 마우스 오른쪽 클릭 (모드 전환)
        else
        {
            // 공격 중일 때에는 모드 전환을 할 수 없다
            if (isAttack) return;

            StartCoroutine(OnModeChange());
        }
    }

    public override void StopWeaponAction(int type = 0)
    {
        // 마우스 왼쪽 클릭 (공격 종료)
        if (type == 0)
        {
            isAttack = false;
            StopCoroutine("OnAttackLoop");
        }
    }

    public override void StartReload()
    {
        // 현재 재장전 중이면 재장전 불가능
        if (weaponSetting.currentAmmo == weaponSetting.maxAmmo) return;
        if (isReload) return;
        
        // 무기 액션 도중에 'R'키를 눌러 재장전을 시도하면 무기 액션 종료 후 재장전
        StopWeaponAction();

        StartCoroutine(OnReload());
    }

    private IEnumerator OnAttackLoop()
    {
        while (true)
        {
            OnAttack();

            yield return null;
        }
    }

    public void OnAttack()
    {
        if (Time.time - lastAttackTime > weaponSetting.attackRate)
        {
            // 공격주기가 되어야 공격할 수 있도록 하기 위해 현재 시간 저장
            lastAttackTime = Time.time;
            
            // 탄 수가 없으면 공격 불가능
            if (weaponSetting.currentAmmo <= 0)
            {
                return;
            }
            // 공격시 currenAmmo 1 감소
            weaponSetting.currentAmmo--;
            onAmmoEvent.Invoke(weaponSetting.currentAmmo, weaponSetting.maxAmmo);
            
            // 무기 애니메이션 재생 (모드에 따라 AimFire 혹은 Fire 애니메이션 재생)
            // animator.Play("Fire",-1,0);
            string animationName = animator.AimModeIs == true ? "Pistol_AimFire" : "Fire";
            animator.Play(animationName, -1, 0);
            
            // 자 드가자 zzzzzzz
            _mainCamera.DOShakeRotation(0.3f, 0.3f);
            // 총구 이펙트 재생
            //if(!animator.AimModeIs) StartCoroutine(OnMuzzleFlashEffect());
            // 공격 사운드 재생
            PlaySound(audioClipFire);
            // 탄피 생성
            //casingMemoryPool.SpawnCasing(casingSpawnPoint.position, transform.right);
            
            // 광선을 발사해 원하는 위치 공격(+Impact Effect)
            TwoStepRaycast();
        }
    }

    // private IEnumerator OnMuzzleFlashEffect()
    // {
    //     muzzleFlashEffect.SetActive(true);
    //
    //     yield return new WaitForSeconds(weaponSetting.attackRate * 0.3f);
    //     
    //     muzzleFlashEffect.SetActive(false);
    // }

    private IEnumerator OnReload()
    {
        isReload = true;

        // 재장전 애니메이션, 사운드 재생
        animator.OnReload();
        PlaySound(audioClipReload);
        yield return new WaitForSeconds(weaponSetting.reloadTime);

        isReload = false;

        // 현재 탄 수를 최대로 설정하고, 바뀐 탄 수 정보를 Text UI에 업데이트
        weaponSetting.currentAmmo = weaponSetting.maxAmmo;
        onAmmoEvent.Invoke(weaponSetting.currentAmmo, weaponSetting.maxAmmo);

        yield return null;
    }

    private void TwoStepRaycast()
    {
        Ray ray;
        RaycastHit hit;
        Vector3 targetPoint = Vector3.zero;
        
        // 화면의 중앙 좌표 (Aim 기준으로 Raycast 연산)
        ray = _mainCamera.ViewportPointToRay(Vector2.one * 0.5f);
        // 공격 사거리(attackDistance) 안에 부딪히는 오브젝트가 있으면 targetPoint는 광선에 부딪힌 위치
        if (Physics.Raycast(ray, out hit, weaponSetting.attackDistance))
        {
            targetPoint = hit.point;
        }
        // 공격 사거리 안에 부딪히는 오브젝트가 없으면 targetPoint는 최대 사거리 위치
        else
        {
            targetPoint = ray.origin + ray.direction * weaponSetting.attackDistance;
        }

        // 첫 번째 Raycast 연산으로 얻어진 targetPoint를 목표 지점으로 설정하고,
        // 총구를 시작지점으로 하여 Raycast 연산
        Vector3 attackDirection = (targetPoint - bulletSpawnPoint.position).normalized;
        if (Physics.Raycast(bulletSpawnPoint.position, attackDirection, out hit, weaponSetting.attackDistance))
        {
            GameObject obj = Instantiate(hitEffect, hit.point, Quaternion.LookRotation(hit.normal));
            Destroy(obj, 2);
        }
    }

    private IEnumerator OnModeChange()
    {
        float current = 0;
        float percent = 0;
        float time = 0.35f;

        animator.AimModeIs = !animator.AimModeIs;
        imageAim.enabled = !imageAim.enabled;

        float start = _mainCamera.fieldOfView;
        float end = animator.AimModeIs == true ? aimModeFOV : defaultModeFOV;

        _isModeChange = true;

        while (percent < 1)
        {
            current += Time.deltaTime;
            percent = current / time;
            
            // mode에 따라 카메라의 시야각을 변경
            _mainCamera.fieldOfView = Mathf.Lerp(start, end, percent);

            yield return null;
        }

        _isModeChange = false;
    }

    private void ResetVariables()
    {
        isReload = false;
        isAttack = false;
        _isModeChange = false;
    }
}
