﻿using System.Collections;
using System.Net;
using System.Security.Cryptography;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class Plushie : MonoBehaviour
{
    public bool IsThrowDone { get => coroutine == null; }

    private Coroutine coroutine;
    private Transform handAnchor;
    private Rigidbody _rigidbody;
    private Collider _collider;
    private bool isActive;

    private void Start()
    {
        if (!TryGetComponent(out _collider))
        {
            MeshCollider collider = gameObject.AddComponent<MeshCollider>();
            collider.convex = true;
            this._collider = collider;
        }

        if (!TryGetComponent(out _rigidbody))
        {
            _rigidbody = gameObject.AddComponent<Rigidbody>();
        }

        gameObject.layer = 6;
        //Debug.Log(gameObject.layer);
    }

    public void ThrowPlushie(Vector3 targetDir)
    {
        if (coroutine != null)
        {
            transform.position = handAnchor.position;
            transform.parent = handAnchor;
            StopCoroutine(coroutine);
        }

        _collider.enabled = true;

        Vector3 force = targetDir * 1000;
        _rigidbody.isKinematic = false;
        _rigidbody.AddForce(force, ForceMode.Impulse);

        Debug.Log("Plushie Thrown");
        isActive = true;
        transform.parent = null;
        coroutine = null;
    }

    public void AttachToTarget(Transform target)
    {
        this.handAnchor = target;
        _collider.enabled = false;
        _rigidbody.isKinematic = true;

        coroutine = StartCoroutine(_AttachToTarget(target));
    }

    private IEnumerator _AttachToTarget(Transform target, float duration = 0.1f)
    {
        float distance = Vector3.Distance(target.position,transform.position);
        float time = 0;
        Vector3 origin = transform.position;

        while (time < duration)
        {
            transform.position = Vector3.Lerp(origin,target.position, time);

            time += Time.deltaTime * Time.timeScale;

            yield return null;
        }

        transform.position = target.position;
        transform.parent = target;

        coroutine = null;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isActive)
        {
            Debug.Log(collision.gameObject.name);
            if (collision.gameObject.layer == 8)
            {
                Debug.Log("Child has been Hospitalized.");
                ChildData data = GameData.GetData<ChildData>("Child");
                data.stunned = true;
            }
        }
        isActive = false;
    }

}