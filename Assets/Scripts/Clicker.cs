using UnityEngine;

public class Clicker : MonoBehaviour
{
    public AudioClip spawn;

    private Camera _mainCam;
    private AudioSource _source;

    private void Awake()
    {
        _mainCam = Camera.main;
        _source = GetComponent<AudioSource>();
    }

    private void Update()
    {
        var ray = _mainCam.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out var hit))
        {
            if (hit.transform.CompareTag("Ground") && Input.GetMouseButtonDown(0))
            {
                var pos = hit.point + Vector3.up * 3;
                if (GameManager.Instance.UpgradeWorker(0, pos))
                {
                    ObjectPool.Pool.SpawnWorker(pos);
                    _source.PlayOneShot(spawn);
                }
            }
        }
    }
}