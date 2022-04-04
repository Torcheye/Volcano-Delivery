using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class Popup : MonoBehaviour
{
    public void SetText(string text)
    {
        GetComponentInChildren<TMP_Text>().text = text;
        StartCoroutine(Move());
    }

    private IEnumerator Move()
    {
        for (int i = 0; i < 40; i++)
        {
            transform.Translate(0, .03f, 0);
            yield return new WaitForSeconds(.01f);
        }
        gameObject.SetActive(false);
    }
    
    
}