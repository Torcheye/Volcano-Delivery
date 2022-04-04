using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool Pool;
    public int boxNumber, workerNumber;
    public GameObject workerPrefab;
    public List<GameObject> boxPrefabList;
    public List<Transform> factoryList;
    public GameObject popPrefab;

    private List<GameObject> _boxList;
    private List<GameObject> _workerList;
    private List<GameObject> _popList;

    private void Awake()
    {
        Pool = this;
        _boxList = new List<GameObject>();
        _workerList = new List<GameObject>();
        _popList = new List<GameObject>();
    }

    private void Start()
    {
        for (var i = 0; i < boxNumber; i++)
        {
            foreach (var t in boxPrefabList)
            {
                var box = Instantiate(t, Vector3.one * -200, Quaternion.identity);
                _boxList.Add(box);
                box.SetActive(false);
            }
        }

        for (var i = 0; i < workerNumber; i++)
        {
            var w = Instantiate(workerPrefab, Vector3.zero, Quaternion.identity);
            _workerList.Add(w);
            w.SetActive(false);

            var p = Instantiate(popPrefab, Vector3.zero, new Quaternion(0.707f, 0, 0, 0.707f));
            _popList.Add(p);
            p.SetActive(false);
        }
    }

    public void SpawnPop(string text, Vector3 pos)
    {
        GameObject pop = null;
        foreach (var p in _popList)
        {
            if (!p.activeInHierarchy)
                pop = p;
        }

        if (pop == null)
        {
            Debug.LogError("Running out of pops!");
            return;
        }

        pop.SetActive(true);
        pop.GetComponent<Popup>().SetText(text);
        pop.transform.position = pos;
    }

    public Vector3 GetFactoryPosition(BoxColor color) => factoryList[(int) color].position;

    public void SpawnWorker(Vector3 pos)
    {
        GameObject worker = null;
        foreach (var w in _workerList)
        {
            if (!w.activeInHierarchy)
                worker = w;
        }

        if (worker == null)
        {
            Debug.LogError("Running out of workers!");
            return;
        }

        worker.SetActive(true);
        worker.GetComponent<NavMeshAgent>().enabled = true;
        worker.transform.position = pos;
    }

    public GameObject SpawnBox(Vector3 pos, BoxColor color)
    {
        GameObject box = null;
        var targetTag = color.ToString();
        foreach (var b in _boxList)
        {
            if (!b.activeInHierarchy && b.CompareTag(targetTag))
                box = b;
        }

        if (box == null)
        {
            Debug.LogError("Running out of boxes!");
            return null;
        }
        
        box.SetActive(true);
        box.transform.position = pos;
        return box;
    }

    public List<GameObject> GetActiveBoxList()
    {
        return _boxList.Where(o => o.activeInHierarchy).ToList();
    }
}