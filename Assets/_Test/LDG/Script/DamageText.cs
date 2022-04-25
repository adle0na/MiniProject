using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class DamageText : MonoBehaviour
{
    [SerializeField] private TMP_Text tmpText;

    private float textDuration = 2;

    private Sequence popText;
    
    private void OnValidate()
    {
        tmpText = GetComponent<TMP_Text>();
    }

    private void Awake()
    {
        popText = DOTween.Sequence();
        // popText.Append(transform.DOMove(transform.position + transform.up, textDuration)).Insert(0, tmpText.DOColor(new Color(1, 1, 1, 0), textDuration));
        popText.Append(transform.DOLocalMove(Vector3.up, textDuration).SetRelative())
            .Insert(.2f, tmpText.DOColor(new Color(1, 1, 1, 0), textDuration - .2f))
            .Insert(0, transform.DOScale(transform.localScale * 1.5f, 0.2f))
            .Insert(.2f, transform.DOScale(Vector3.zero, textDuration - 0.2f)
                //    .OnComplete(() => gameObject.SetActive(false)))
                .OnComplete(() => Destroy(gameObject))
                .Pause()
                .SetAutoKill(false));

    }
    
    public void EnableText(Vector3 pos,float damage)
    {
        transform.position = pos;
        tmpText.text = $"{damage}";
        popText.Restart();
    }
    
}
