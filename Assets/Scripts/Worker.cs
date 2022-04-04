using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Worker : MonoBehaviour
{
    private const int MaxLevel = 10;
    public Transform handlingPos;
    public int collisionForce;
    
    private enum State
    {
        SeekingBox,
        GoingToBox,
        DeliveringBox,
        Idle
    }
    private State _state;

    private NavMeshAgent _agent;
    private Animator _animator;
    private Box _box;
    private Outline _outline;
    private int _level;
    private Vector3 _target;
    
    private static readonly int Hold = Animator.StringToHash("Hold");

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
        _outline = GetComponent<Outline>();
    }

    private void OnEnable()
    {
        _state = State.SeekingBox;
        _level = 1;
        _agent.stoppingDistance = 2.2f;
    }

    private void Update()
    {
        switch (_state)
        {
            case State.SeekingBox:
                var box = FindNearestBox();
                if (box != null)
                {
                    box.GetComponent<Box>().HasBeenPicked = true;
                    _box = box.GetComponent<Box>();
                    _target = _box.transform.position;
                    _agent.SetDestination(_target);
                    _state = State.GoingToBox;
                    _agent.stoppingDistance = 2.2f;
                }
                break;
            case State.GoingToBox:
                _target = _box.transform.position;
                _agent.SetDestination(_target);
                if (IsAgentCompleted())
                    StartCoroutine(PickUpBox());
                break;
            case State.DeliveringBox:
                if (IsAgentCompleted())
                    StartCoroutine(ReleaseBox());
                break;
            default:
                return;
        }
    }

    private bool IsAgentCompleted()
    {
        var position = transform.position;
        var thisPos = new Vector3(position.x, 0, position.y);
        var targetPos = new Vector3(_target.x, 0, _target.y);
        return Vector3.Distance(thisPos, targetPos) <= _agent.stoppingDistance;
    }

    private GameObject FindNearestBox()
    {
        List<GameObject> list = ObjectPool.Pool.GetActiveBoxList();
        if (list.Count == 0)
        {
            Debug.Log("No active box remaining");
            return null;
        }

        GameObject target = null;
        float minDist = float.PositiveInfinity;
        foreach (var b in list)
        {
            if (b.GetComponent<Box>().HasBeenPicked)
                continue;
            float dist = Vector3.Distance(transform.position, b.transform.position);
            if (dist < minDist)
            {
                target = b;
                minDist = dist;
            }
        }

        return target;
    }

    private IEnumerator PickUpBox()
    {
        _state = State.Idle;
        _animator.SetBool(Hold, true);
        yield return new WaitForSeconds(1);
        _box.PickUp(handlingPos);
        _state = State.DeliveringBox;
        _agent.stoppingDistance = 3.5f;
        GetComponent<AudioSource>().Play();
        
        var color = _box.tag switch
        {
            "Red" => BoxColor.Red,
            "Blue" => BoxColor.Blue,
            "Purple" => BoxColor.Purple,
            "Cyan" => BoxColor.Cyan,
            _ => throw new ArgumentOutOfRangeException(nameof(tag), tag, null)
        };
        _target = ObjectPool.Pool.GetFactoryPosition(color);
        _agent.SetDestination(_target);
    }

    private IEnumerator ReleaseBox()
    {
        _animator.SetBool(Hold, false);
        _box.Release();
        _state = State.Idle;
        yield return new WaitForSeconds(1);
        _state = State.SeekingBox;
    }

    private void Grow()
    {
        if (_level < MaxLevel)
            _level++;
        else
            return;

        const float multiplier = 1.18f;
        _agent.speed *= multiplier;
        _agent.acceleration *= multiplier;
        _agent.avoidancePriority++;
        transform.localScale *= multiplier;
        _animator.SetFloat("Speed", _animator.GetFloat("Speed") * .9f);
    }

    private void OnMouseEnter()
    {
        _outline.enabled = true;
    }

    private void OnMouseUp()
    {
        if (GameManager.Instance.UpgradeWorker(_level, transform.position))
            Grow();
    }

    private void OnMouseExit()
    {
        _outline.enabled = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Worker"))
        {
            var force = (transform.position - collision.transform.position) * collisionForce * _level;
            collision.rigidbody.AddForce(force);
        }

        if (collision.transform.name.Equals("Lava"))
        {
            gameObject.SetActive(false);
        }
    }
}