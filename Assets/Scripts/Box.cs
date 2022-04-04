using System;
using System.Collections;
using UnityEngine;

public class Box : MonoBehaviour
{
    public bool HasBeenPicked { get; set; }

    private Transform _handlingTransform;
    private bool _isHandling;

    private void Update()
    {
        if (_isHandling)
        {
            transform.position = _handlingTransform.position;
        }
    }

    public void PickUp(Transform handlingTransform)
    {
        _handlingTransform = handlingTransform;
        _isHandling = true;
        GetComponent<Rigidbody>().isKinematic = true;
    }

    public void Release()
    {
        GetComponent<Rigidbody>().isKinematic = false;
        _isHandling = false;
        StartCoroutine(StartConsume());
    }

    private IEnumerator StartConsume()
    {
        yield return new WaitForSeconds(1);
        GameManager.Instance.AddMoney(10, transform.position);
        var color = tag switch
        {
            "Red" => BoxColor.Red,
            "Blue" => BoxColor.Blue,
            "Purple" => BoxColor.Purple,
            "Cyan" => BoxColor.Cyan,
            _ => throw new ArgumentOutOfRangeException(nameof(tag), tag, null)
        };
        GameManager.Instance.AddSupply(color);
        HasBeenPicked = false;
        gameObject.SetActive(false);
    }
}