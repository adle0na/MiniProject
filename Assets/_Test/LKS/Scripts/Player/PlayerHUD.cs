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
    private WeaponColtDoubleEagle weapon; // 현재 정보가 출력되는 무기

    [Header("Weapon Base")] [SerializeField]
    private TextMeshProUGUI textWeaponName; // 무기 이름
    [SerializeField] private Image imageWeaponIcon; // 무기 아이콘
    [SerializeField] private Sprite[] spriteWeaponIcons; // 무기 아이콘에 사용되는 sprite 배열

    [Header("Ammo")] 
    [SerializeField] private Image imageAmmo;
    [SerializeField] private TextMeshProUGUI textAmmo; // 현재/최대 탄 수 출력 text

    [Header("Magazine")] [SerializeField] 
    private GameObject magazineUIPrefab; // 탄창 UI 프리펩
    [SerializeField] private Transform magazineParent; // 탄창 UI가 배치되는 Panel

    private List<GameObject> _magazineList; // 탄창 UI 리스트

    private void Awake()
    {
        SetupWeapon();
        SetupMagazine();
        
        // 메소드가 등록되어 있는 이벤트 클래스(weapon.xx)의
        // Invoke() 메소드가 호출될 때 등록된 메소드(매개변수)가 실행된다
        weapon.onAmmoEvent.AddListener(UpdateAmmoHUD);
        weapon.onMagazineEvent.AddListener(UpdateMagazineHUD);
    }

    private void SetupWeapon()
    {
        textWeaponName.text = weapon.WeaponName.ToString();
        imageWeaponIcon.sprite = spriteWeaponIcons[(int) weapon.WeaponName];
    }

    private void UpdateAmmoHUD(int currentAmmo, int maxAmmo)
    {
        imageAmmo.DOFillAmount((float) currentAmmo / maxAmmo, 0.3f);

        textAmmo.DOText($"<size=40>{currentAmmo}/</size>{maxAmmo}", 0.3f, true, ScrambleMode.All);
    }

    private void SetupMagazine()
    {
        // weapon에 등록되어 있는 최대 탄창 개수만큼 Image Icon을 생성
        // magazineParent 오브젝트의 자식으로 등록 후 모두 비활성화/리스트에 저장
        
        ColorUtility.TryParseHtmlString("#00F0A5", out Color color);
        
        _magazineList = new List<GameObject>();
        for (int i = 0; i < weapon.MaxMagazine; ++i)
        {
            GameObject clone = Instantiate(magazineUIPrefab);
            clone.transform.SetParent(magazineParent);
            clone.SetActive(false);
            
            _magazineList.Add(clone);
        }
        
        // weapon에 등록되어 있는 현재 탄창 개수만큼 오브젝트 활성화
        for (int i = 0; i < weapon.CurrentMagazine; ++i)
        {
            _magazineList[i].GetComponent<Image>().color = color;
            _magazineList[i].SetActive(true);
        }
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
}
