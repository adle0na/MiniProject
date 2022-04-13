using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponThrow : WeaponBase
{
    [Header("Audio Clips")] [SerializeField]
    private AudioClip audioClipFire; // 공격 사운드

    [Header("Grenade")] [SerializeField] private GameObject grenadePrefab; // 수류탄 프리팹
    [SerializeField] private Transform grenadeSpawnPoint; // 수류탄 생성 위치

    private void OnEnable()
    {
        // 무기가 활성화될 때 무기의 탄 수 정보를 갱신한다
        onAmmoEvent.Invoke(weaponSetting.currentAmmo, weaponSetting.maxAmmo);
    }
    
    private void Awake()
    {
        base.SetUp();

        // 처음 탄 수는 최대로 설정
        weaponSetting.currentAmmo = weaponSetting.maxAmmo;
    }

    public override void StartWeaponAction(int type = 0)
    {
        if (type == 0 && !isAttack && weaponSetting.currentAmmo > 0)
        {
            StartCoroutine(OnAttack());
        }
    }

    public override void StopWeaponAction(int type = 0)
    {
    }

    public override void StartReload()
    {
    }

    private IEnumerator OnAttack()
    {
        isAttack = true;
        
        //공격 애니메이션 재생
        animator.Play("Fire", -1, 0);
        // 공격 사운드 재생
        PlaySound(audioClipFire);

        yield return new WaitForEndOfFrame();

        while (true)
        {
            if (animator.CurrentAnimationIs("Movement"))
            {
                isAttack = false;
                
                yield break;
            }

            yield return null;
        }
    }
    
    // grenade 애니메이션 이벤트 함수
    public void SpawnGrenadeProjectile()
    {
        GameObject grenadeClone = Instantiate(grenadePrefab, grenadeSpawnPoint.position, Random.rotation);
        grenadeClone.GetComponent<WeaponGrenadeProjectile>().Setup(weaponSetting.damage, transform.parent.forward);

        weaponSetting.currentAmmo--;
        onAmmoEvent.Invoke(weaponSetting.currentAmmo, weaponSetting.maxAmmo);
    }
}
