using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Old_WeaponController : MonoBehaviour
{
    [FormerlySerializedAs("currentWeapon")] [SerializeField] private Old_Weapon currentOldWeapon;

    private bool isReload;
    private float currentAttackRate;

    private RaycastHit hitInfo;
    [SerializeField] private Camera theCamera;
    [SerializeField] private GameObject hitEffectPrefab;
    private static readonly int Shot = Animator.StringToHash("Shot");
    private static readonly int IsReload = Animator.StringToHash("isReload");

    private void Update()
    {
        FireRateCalc();
        TryFire();
        TryReload();
    }

    private void FireRateCalc()
    {
        if (currentAttackRate > 0)
        {
            currentAttackRate -= Time.deltaTime;
        }
    }

    private void TryFire()
    {
        if (Input.GetMouseButtonDown(0) && !isReload && currentAttackRate <= 0)
        {
            Fire();
        }
    }

    private void Fire()
    {
        if(currentOldWeapon.gunCurrentBullet > 0)
        {
            currentOldWeapon.animator.SetTrigger(Shot);
            currentOldWeapon.gunCurrentBullet--;
            currentAttackRate = currentOldWeapon.attackRate;
            Hit();
        }
        else
        {
            StartCoroutine(Reload());
        }
    }

    private void Hit()
    {
        if (Physics.Raycast(theCamera.transform.position, theCamera.transform.forward, out hitInfo, currentOldWeapon.attackRange))
        {
            Debug.Log(hitInfo);
            GameObject clone = Instantiate(hitEffectPrefab, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));
            Destroy(clone, 2f);
        }
    }

    private void TryReload()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (currentOldWeapon.gunCurrentBullet == currentOldWeapon.gunReloadBullet) return;
            StartCoroutine(Reload());
        }
    }

    private IEnumerator Reload()
    {
        if (currentOldWeapon.gunCarryBullet > 0)
        {
            isReload = true;
            currentOldWeapon.animator.SetBool(IsReload, isReload);

            currentOldWeapon.gunCarryBullet += currentOldWeapon.gunCurrentBullet;
            currentOldWeapon.gunCurrentBullet = 0;

            yield return new WaitForSeconds(currentOldWeapon.reloadDelay);

            if (currentOldWeapon.gunCarryBullet >= currentOldWeapon.gunReloadBullet)
            {
                currentOldWeapon.gunCurrentBullet = currentOldWeapon.gunReloadBullet;
                currentOldWeapon.gunCarryBullet -= currentOldWeapon.gunReloadBullet;
            }
            else
            {
                currentOldWeapon.gunCurrentBullet = currentOldWeapon.gunCarryBullet;
                currentOldWeapon.gunCarryBullet = 0;
            }
            isReload = false;
            currentOldWeapon.animator.SetBool(IsReload, isReload);
        }
    }
}
