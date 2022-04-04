using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Title : MonoBehaviour
{
    private bool _clicked;

    private void Update()
    {
        if (!_clicked)
        {
            if (Input.GetMouseButtonDown(0))
            {
                _clicked = true;
                StartCoroutine(OnFade());
            }
        }
    }

    private IEnumerator OnFade()
    {
        yield return new WaitForSeconds(5);
        Fade();
    }

    private async void Fade()
    {
        var tween = GetComponent<CanvasGroup>().DOFade(0, 1);
        await tween.AsyncWaitForCompletion();
        gameObject.SetActive(false);
    }
}