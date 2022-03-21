using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [SerializeField] private Weapon currentWeapon;

    private bool isReload;
    private float currentAttackRate;

    private RaycastHit hitInfo;
    [SerializeField] private Camera theCamera;
    [SerializeField] private GameObject hitEffectPrefab;

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
        if(currentWeapon.gunCurrentBullet > 0)
        {
            currentWeapon.animator.SetTrigger("Shot");
            currentWeapon.gunCurrentBullet--;
            currentAttackRate = currentWeapon.attackRate;
            Hit();
        }
        else
        {
            StartCoroutine(Reload());
        }
    }

    private void Hit()
    {
        if (Physics.Raycast(theCamera.transform.position, theCamera.transform.forward, out hitInfo, currentWeapon.attackRange))
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
            if (currentWeapon.gunCurrentBullet == currentWeapon.gunReloadBullet) return;
            StartCoroutine(Reload());
        }
    }

    private IEnumerator Reload()
    {
        if (currentWeapon.gunCarryBullet > 0)
        {
            isReload = true;
            currentWeapon.animator.SetBool("isReload", isReload);

            currentWeapon.gunCarryBullet += currentWeapon.gunCurrentBullet;
            currentWeapon.gunCurrentBullet = 0;

            yield return new WaitForSeconds(currentWeapon.reloadDelay);

            if (currentWeapon.gunCarryBullet >= currentWeapon.gunReloadBullet)
            {
                currentWeapon.gunCurrentBullet = currentWeapon.gunReloadBullet;
                currentWeapon.gunCarryBullet -= currentWeapon.gunReloadBullet;
            }
            else
            {
                currentWeapon.gunCurrentBullet = currentWeapon.gunCarryBullet;
                currentWeapon.gunCarryBullet = 0;
            }
            isReload = false;
            currentWeapon.animator.SetBool("isReload", isReload);
        }
    }
}
