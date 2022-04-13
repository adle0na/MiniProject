using System.Collections;
using System.Collections.Generic;
using _Test.LDG.Script;
using UnityEngine;

public enum WeaponType { HandGun = 0, Rifle, ShotGun, Melee, Throw,  }

[System.Serializable]
public class AmmoEvent : UnityEngine.Events.UnityEvent<int, int> { }

public abstract class WeaponBase : MonoBehaviour
{
    [Header("Weapon Base")] [SerializeField]
    public WeaponType weaponType; // 무기 종류
    [SerializeField] protected GameObject hitEffect; // 조준 확인을 위한 피격 이펙트
    [SerializeField] protected WeaponSetting weaponSetting; // 무기 설정

    protected float lastAttackTime = 0; // 마지막 발사시간 체크용
    protected bool isReload = false; // 재장전 중인지 체크
    protected bool isAttack = false; // 공격 여부 체크
    protected AudioSource audioSource; // 사운드 재생 컴포넌트
    protected PlayerAnimatorController animator; // 애니메이션 재생 제어
    
    // 외부에서 이벤트 함수 등록을 할 수 있도록 public 선언
    [HideInInspector] public AmmoEvent onAmmoEvent = new AmmoEvent();

    // 외부에서 필요한 정보를 열람하기 위해 정의한 Get Property
    public PlayerAnimatorController Animator => animator;
    public WeaponName WeaponName => weaponSetting.weaponName;

    public abstract void StartWeaponAction(int type = 0);
    public abstract void StopWeaponAction(int type = 0);
    public abstract void StartReload();

    protected void PlaySound(AudioClip clip)
    {
        audioSource.Stop(); // 기존에 재생중인 사운드를 정지하고
        audioSource.clip = clip; // 새로운 사운드 clip으로 교체 후
        audioSource.Play(); // 사운드 재생
    }

    protected void WeaponAttack(IAttackAble attackAble)
    {
        attackAble.TakeDamage(weaponSetting.damage);
    }

    protected void SetUp()
    {
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<PlayerAnimatorController>();
    }
}
