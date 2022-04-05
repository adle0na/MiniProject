using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class WeaponShotGun : WeaponBase
{
    [Header("Shot Gun Settings")] [SerializeField]
    private int shotGunHitCount = 15;

    [Header("Spawn Points")]
    // private Transform casingSpawnPoint; // 탄피 생성 위치
    [SerializeField]
    private Transform bulletSpawnPoint; // 총알 생성 위치

    [Header("Audio Clips")] [SerializeField]
    private AudioClip audioClipTakeOutWeapon; // 무기 장착 사운드

    [SerializeField] private AudioClip audioClipFire; // 공격 사운드
    [SerializeField] private AudioClip audioClipReload; // 재장전 사운드

    private Camera _mainCamera; // 광선 발사

    private void Awake()
    {
        // 기반 클래스의 초기화를 위한 Setup() 메소드 호출
        base.SetUp();

        // casingMemoryPool = GetComponent<CasingMemoryPool>();
        // impactMemoryPool = GetComponent<ImpactMemoryPool>();
        _mainCamera = Camera.main;

        // 처음 탄창 수는 최대로 설정
        weaponSetting.currentMagazine = weaponSetting.maxMagazine;
        // 처음 탄 수는 최대로 설정
        weaponSetting.currentAmmo = weaponSetting.maxAmmo;
    }

    private void OnEnable()
    {
        // // 무기 장착 사운드 재생
        PlaySound(audioClipTakeOutWeapon);
        // // 총구 이펙트 오브젝트 비활성화
        // muzzleFlashEffect.SetActive(false);

        // 무기가 활성화될 때 해당 무기의 탄창 정보를 갱신한다
        onMagazineEvent.Invoke(weaponSetting.currentMagazine);
        // 무기가 활성화될 때 해당 무기의 탄 수 정보를 갱신한다
        onAmmoEvent.Invoke(weaponSetting.currentAmmo, weaponSetting.maxAmmo);

        ResetVariables();
    }

    public override void StartWeaponAction(int type = 0)
    {
        // 재장전 중일 때는 무기 액션을 할 수 없다
        if (isReload) return;

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
        if (isReload || weaponSetting.currentMagazine <= 0 ||
            weaponSetting.currentAmmo == weaponSetting.maxAmmo) return;

        // 무기 액션 도중에 'R'키를 눌러 재장전을 시도하면 무기 액션 종료 후 재장전
        StopWeaponAction();

        StartCoroutine(OnReload());
    }

    public void OnAttack()
    {
        if (Time.time - lastAttackTime > weaponSetting.attackRate)
        {
            // 뛰고있을 때는 공격할 수 없다.
            if (animator.MoveSpeed > 0.5f)
            {
                return;
            }

            // 공격주기가 되어가 공격할 수 있도록 하기 위해 현재 시간 저장
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

    private IEnumerator OnReload()
    {
        isReload = true;

        // 재장전 애니메이션, 사운드 재생
        animator.OnReload();
        PlaySound(audioClipReload);
        yield return new WaitForSeconds(weaponSetting.reloadTime);

        isReload = false;

        // 현재 탄창 수를 1 감소시키고, 바뀐 탄창 정보를 Text UI에 업데이트
        weaponSetting.currentMagazine--;
        onMagazineEvent.Invoke(weaponSetting.currentMagazine);

        // 현재 탄 수를 최대로 설정하고, 바뀐 탄 수 정보를 Text UI에 업데이트
        weaponSetting.currentAmmo = weaponSetting.maxAmmo;
        onAmmoEvent.Invoke(weaponSetting.currentAmmo, weaponSetting.maxAmmo);

        yield return null;
    }

    private void TwoStepRaycast()
    {
        // 이전 코드. 산탄총을 위해 잠시 주석처리.
        // Ray ray;
        // RaycastHit hit;
        // Vector3 targetPoint = Vector3.zero;
        //
        // // 화면의 중앙 좌표 (Aim 기준으로 Raycast 연산)
        // ray = _mainCamera.ViewportPointToRay(Vector2.one * 0.5f);
        // // 공격 사거리(attackDistance) 안에 부딪히는 오브젝트가 있으면 targetPoint는 광선에 부딪힌 위치
        // if (Physics.Raycast(ray, out hit, weaponSetting.attackDistance))
        // {
        //     targetPoint = hit.point;
        // }
        // // 공격 사거리 안에 부딪히는 오브젝트가 없으면 targetPoint는 최대 사거리 위치
        // else
        // {
        //     targetPoint = ray.origin + ray.direction * weaponSetting.attackDistance;
        // }
        //
        // Debug.DrawRay(ray.origin, ray.direction * weaponSetting.attackDistance, Color.red);
        //
        // // 첫 번째 Raycast 연산으로 얻어진 targetPoint를 목표 지점으로 설정하고,
        // // 총구를 시작지점으로 하여 Raycast 연산
        // Vector3 attackDirection = (targetPoint - bulletSpawnPoint.position).normalized;
        // // if (Physics.Raycast(bulletSpawnPoint.position, attackDirection, out hit, weaponSetting.attackDistance))
        // // {
        // //     impactMemoryPool.SpawnImpact(hit);
        // // }
        // Debug.DrawRay(bulletSpawnPoint.position, attackDirection * weaponSetting.attackDistance, Color.blue);

        RaycastHit hit;
        List<Ray> rays = new List<Ray>();
        Vector3 targetPoint = Vector3.zero;

        for (int i = 0; i < shotGunHitCount; i++)
        {
            rays.Add(_mainCamera.ViewportPointToRay(new Vector2(Random.Range(0.45f, 0.55f), Random.Range(0.45f, 0.55f))));
        }

        foreach (Ray ray in rays)
        {
            // Do the thing 코드 작성중
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
            
            Vector3 attackDirection = (targetPoint - bulletSpawnPoint.position).normalized;
            if (Physics.Raycast(bulletSpawnPoint.position, attackDirection, out hit, weaponSetting.attackDistance))
            {
                GameObject obj = Instantiate(hitEffect, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(obj, 2);
            }
            // Debug.DrawRay(bulletSpawnPoint.position, attackDirection * weaponSetting.attackDistance, Color.blue);
        }
    }

    private void ResetVariables()
    {
        isReload = false;
        isAttack = false;
    }
}