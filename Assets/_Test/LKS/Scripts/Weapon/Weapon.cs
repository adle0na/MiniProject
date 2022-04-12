using UnityEngine.Serialization;

public enum WeaponName { ColtDoubleEagle=0, DesertEagle, WinchesterRifle, DoubleBarreled, CombatKnife, Excalibur, HandGrenade, }

[System.Serializable]
public struct WeaponSetting
{
    public WeaponName weaponName; // 무기 이름
    public int damage; // 무기 공격력
    public float reloadTime; // 재장전에 걸리는 시간
    public int currentAmmo; // 현재 탄약 수
    public int maxAmmo; // 최대 탄약 수
    public float attackRate; // 공격 속도
    public float attackDistance; // 공격 사거리
    public bool isAutomaticAttack; // 연속 공격 여부
}