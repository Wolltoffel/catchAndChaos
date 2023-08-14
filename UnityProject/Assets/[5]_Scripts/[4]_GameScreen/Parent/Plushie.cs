using System.Collections;
using System.Net;
using System.Security.Cryptography;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class Plushie : MonoBehaviour
{
    public bool IsThrowDone { get => coroutine == null; }

    private Coroutine coroutine;
    private Transform handAnchor;
    private Rigidbody rigidbody;
    private Collider collider;
    private bool isActive;

    private void Start()
    {
        if (!TryGetComponent(out collider))
        {
            MeshCollider collider = gameObject.AddComponent<MeshCollider>();
            collider.convex = true;
            this.collider = collider;
        }

        if (!TryGetComponent(out rigidbody))
        {
            rigidbody = gameObject.AddComponent<Rigidbody>();
        }

        gameObject.layer = 6;
        Debug.Log(gameObject.layer);
    }

    public void ThrowPlushie(Vector3 target)
    {
        if (coroutine != null)
        {
            transform.position = handAnchor.position;
            transform.parent = handAnchor;
            StopCoroutine(coroutine);
        }

        coroutine = StartCoroutine(_ThrowPlushie(target));
    }

    private IEnumerator _ThrowPlushie(Vector3 target)
    {
        Vector3 targetDir = (target - transform.position).normalized;
        Transform parent = CharacterInstantiator.GetActiveCharacter(Characters.Parent).transform;
        LayerMask obstacleLayer;
        obstacleLayer = ~6;
        targetDir = Vector3.Dot(targetDir, parent.forward) > 0.2f ? Physics.Linecast(parent.position, target, out RaycastHit hit, obstacleLayer) ? parent.forward : targetDir : parent.forward;
        targetDir = new Vector3(targetDir.x,0,targetDir.z).normalized;

        yield return new WaitForSeconds(0.2f);

        collider.enabled = true;

        Vector3 force = targetDir * 60;
        rigidbody.isKinematic = false;
        rigidbody.AddForce(force,ForceMode.Impulse);

        Debug.Log("Plushie Thrown");
        isActive = true;
        transform.parent = null;
        coroutine = null;
    }

    public void AttachToTarget(Transform target)
    {
        this.handAnchor = target;
        collider.enabled = false;
        rigidbody.isKinematic = true;

        coroutine = StartCoroutine(_AttachToTarget(target));
    }

    private IEnumerator _AttachToTarget(Transform target, float duration = 0.1f)
    {
        float distance = Vector3.Distance(target.position,transform.position);
        float time = 0;
        Vector3 origin = transform.position;

        while (time < duration)
        {
            transform.position = Vector3.Lerp(origin,target.position + Vector3.up, time);

            time += Time.deltaTime * Time.timeScale;

            yield return null;
        }

        transform.position = target.position + Vector3.up;
        transform.parent = target;

        coroutine = null;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isActive)
        {
            if (collision.gameObject.layer == 8)
            {
                Debug.Log("Child has been Hospitalized.");
                collision.gameObject.GetComponent<GameChild>();
                //handleCollision
            }
        }

        isActive = false;
    }

}