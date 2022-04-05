using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHUD : MonoBehaviour
{
    [Header("Components")] [SerializeField]
    private WeaponBase weapon; // 현재 정보가 출력되는 무기

    [Header("Components")] [SerializeField]
    private Status status;

    [Header("Weapon Base")] [SerializeField]
    private TextMeshProUGUI textWeaponName; // 무기 이름
    [SerializeField] private Image imageWeaponIcon; // 무기 아이콘
    [SerializeField] private Image imageWeaponAmmoIcon; // 사용한 만큼 비는 무기 아이콘
    [SerializeField] private Sprite[] spriteWeaponIcons; // 무기 아이콘에 사용되는 sprite 배열

    [Header("Ammo")] 
    [SerializeField] private Image imageAmmo;
    [SerializeField] private TextMeshProUGUI textAmmo; // 현재/최대 탄 수 출력 text

    [Header("Magazine")] [SerializeField] 
    private GameObject magazineUIPrefab; // 탄창 UI 프리펩
    [SerializeField] private Transform magazineParent; // 탄창 UI가 배치되는 Panel
    [SerializeField] private int maxMagazineCount; // 처음 생성하는 최대 탄창 수

    private List<GameObject> _magazineList; // 탄창 UI 리스트
    
    [Header("HP & BloodScreen UI")] [SerializeField]
    private TextMeshProUGUI textHp; // 플레이어의 체력을 출력하는 text
    [SerializeField] private Image imageBloodScreen; // 플레이어가 공격받았을 때 화면에 표시되는 image
    [SerializeField] private AnimationCurve curveBloodScreen;

    private void Awake()
    {
        status.onHPEvent.AddListener(UpdateHPHUD);
    }

    public void SetupAllWeapons(WeaponBase[] weapons)
    {
        SetupMagazine();
        
        // 사용 가능한 모든 무기의 이번트 등록
        for (int i = 0; i < weapons.Length; i++)
        {
            weapons[i].onAmmoEvent.AddListener(UpdateAmmoHUD);
            weapons[i].onMagazineEvent.AddListener(UpdateMagazineHUD);
        }
    }

    public void SwitchingWeapon(WeaponBase newWeapon)
    {
        weapon = newWeapon;
        
        SetupWeapon();
    }

    private void SetupWeapon()
    {
        textWeaponName.text = weapon.WeaponName.ToString();
        imageWeaponIcon.sprite = spriteWeaponIcons[(int) weapon.weaponType];
        imageWeaponAmmoIcon.sprite = spriteWeaponIcons[(int) weapon.weaponType];
    }

    private void UpdateAmmoHUD(int currentAmmo, int maxAmmo)
    {
        float fillAmount = maxAmmo != 0 ? (float) currentAmmo / maxAmmo : 1;

        imageAmmo.DOFillAmount(fillAmount, 0.3f);

        textAmmo.DOText($"<size=40>{currentAmmo}/</size>{maxAmmo}", 0.3f, true, ScrambleMode.All);
    }

    private void SetupMagazine()
    {
        // weapon에 등록되어 있는 최대 탄창 개수만큼 Image Icon을 생성
        // magazineParent 오브젝트의 자식으로 등록 후 모두 비활성화/리스트에 저장
        
        ColorUtility.TryParseHtmlString("#00F0A5", out Color color);
        
        _magazineList = new List<GameObject>();
        for (int i = 0; i < maxMagazineCount; ++i)
        {
            GameObject clone = Instantiate(magazineUIPrefab);
            clone.transform.SetParent(magazineParent);
            clone.SetActive(false);
            
            _magazineList.Add(clone);
        }
        
        // weapon에 등록되어 있는 현재 탄창 개수만큼 오브젝트 활성화
        // for (int i = 0; i < weapon.CurrentMagazine; ++i)
        // {
        //     _magazineList[i].GetComponent<Image>().color = color;
        //     _magazineList[i].SetActive(true);
        // }
    }

    private void UpdateMagazineHUD(int currentMagazine)
    {
        ColorUtility.TryParseHtmlString("#00F0A5", out Color color);
        
        
        // 전부 비활성화하고, currentMagazine 개수만큼 활성화
        for (int i = 0; i < _magazineList.Count; ++i)
        {
            var j = i;
            _magazineList[i].GetComponent<Image>().DOColor(Color.clear, 0.3f)
                .OnComplete(() => _magazineList[j].SetActive(false));
            // _magazineList[i].SetActive(false);
        }

        for (int i = 0; i < currentMagazine; ++i)
        {
            var j = i;
            _magazineList[i].GetComponent<Image>().DOColor(color, 0.3f)
                .OnComplete(() => _magazineList[j].SetActive(true));
        }
    }
    
    private void UpdateHPHUD(int previous, int current)
    {
        textHp.text = $"HP {current}";
        
        // 체력이 증가했을 때는 화면에 빨간색 이미지를 출력하지 않도록 return
        if (previous <= current) return;

        if (previous - current > 0)
        {
            StopCoroutine(OnBloodScreen());
            StartCoroutine(OnBloodScreen());
        }
    }

    private IEnumerator OnBloodScreen()
    {
        float percent = 0;
        while (percent < 1)
        {
            percent += Time.deltaTime;

            Color color = imageBloodScreen.color;
            color.a = Mathf.Lerp(1, 0, curveBloodScreen.Evaluate(percent));
            imageBloodScreen.color = color;

            yield return null;
        }
    }
}
