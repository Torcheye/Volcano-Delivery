using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public TMP_Text moneyText;
    public Slider supplyBar;
    public static GameManager Instance;
    public List<BoxSpawner> boxSpawners;
    public CanvasGroup gameOver;

    private int _money;
    private Camera _mainCam;
    private float _supply;
    private float _consumeSpeed;
    private AudioSource _source;

    private void Awake()
    {
        Instance = this;
        _mainCam = Camera.main;
        _source = GetComponent<AudioSource>();
    }

    private void Start()
    {
        _money = 10;
        _supply = .5f;
        _consumeSpeed = .008f;
        StartCoroutine(CameraZoom());
        StartCoroutine(Activate());
        StartCoroutine(Consume());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();
    }

    public void AddSupply(BoxColor color)
    {
        if (_supply <= 1f)
            _supply += .007f * (int) color + 0.01f;
    }

    private IEnumerator Consume()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            if (_supply > 0)
            {
                _supply -= _consumeSpeed / 10f;
                supplyBar.value = _supply;
                _consumeSpeed += .0015f;
                if (BoxSpawner.SpawnInterval > .3f)
                    BoxSpawner.SpawnInterval -= .02f;
            }
            else
            {
                GameOver();
            }
        }
    }

    private async void GameOver()
    {
        var tween = gameOver.DOFade(1, .5f);
        await tween.AsyncWaitForCompletion();
        Time.timeScale = 0;
    }
    
    private IEnumerator Activate()
    {
        boxSpawners[0].enabled = true;
        BoxSpawner.ColorSelectionMax = 1;
        yield return new WaitForSeconds(45);
        boxSpawners[1].enabled = true;
        BoxSpawner.ColorSelectionMax++;
        yield return new WaitForSeconds(45);
        boxSpawners[2].enabled = true;
        BoxSpawner.ColorSelectionMax++;
        yield return new WaitForSeconds(45);
        boxSpawners[3].enabled = true;
        BoxSpawner.ColorSelectionMax++;
        yield return new WaitForSeconds(45);
        boxSpawners[4].enabled = true;
    }

    private IEnumerator CameraZoom()
    {
        while (_mainCam.transform.position.y < 138)
        {
            yield return new WaitForSeconds(.1f);
            _mainCam.transform.position += Vector3.up * .04f;
        }
    }

    public bool UpgradeWorker(int level, Vector3 pos)
    {
        int price = 5 - level <= 0 ? 0 : 5 - level;
        if (_money < price)
        {
            return false;
        }

        _money -= price;
        moneyText.text = "$" + _money;
        ObjectPool.Pool.SpawnPop("-$" + price, pos);
        return true;
    }

    public void AddMoney(int amount, Vector3 pos)
    {
        _money += amount;
        moneyText.text = "$" + _money;
        ObjectPool.Pool.SpawnPop("😊+" + amount, pos);
        _source.PlayOneShot(_source.clip);
    }
}