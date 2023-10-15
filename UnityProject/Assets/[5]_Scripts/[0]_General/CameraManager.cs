using System.Collections;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [Header("Anchor Positions")]
    [SerializeField] private Transform gamePosition;

    [Header("Internal Data")]
    private Transform parent;
    private Transform child;

    private Coroutine coroutine;
    private Animator animator;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }

    public void SetCameraAsMain()
    {
        Camera camera = GetComponent<Camera>();
        Camera.SetupCurrent(camera);
        camera.depth = -1;
    }

    public void SetCameraPosition(Transform transform)
    {
        gameObject.transform.rotation = transform.rotation;
        gameObject.transform.position = transform.position;
    }

    public void GameCamera()
    {
        transform.position = gamePosition.position;
        transform.rotation = gamePosition.rotation;
    }

    public void TrackPlayers(Transform parent, Transform child)
    {
        this.parent = parent;
        this.child = child;

        coroutine = StartCoroutine(_TrackPlayers());
    }
    public void EndTrackPlayers()
    {
        if (coroutine!= null)
        {
            StopCoroutine(coroutine);
            coroutine = null;
        }
    }

    private Vector3 CalculateCurrentPosition(Transform parent, Transform child)
    {
        float minZoom = 25f;
        float maxZoom = 70f;
        float maxDistance = 42;
        float minDistance = 10;

        if (parent != null && child != null)
        {
            float worldDistance = Vector3.Distance(parent.position, child.position);
            float distanceToMidpoint = (worldDistance * 0.5f) / Mathf.Tan(Mathf.Deg2Rad * 15);
            Vector3 midpoint = (parent.position + child.position) / 2f;

            float value = (worldDistance - minDistance) / (maxDistance - minDistance);
            float distance = ((maxZoom - minZoom) * value) + minZoom;

            Vector3 targetPosition = midpoint - transform.forward * Mathf.Clamp(distance, minZoom, maxZoom);
            return targetPosition;
        }

        return Vector3.zero;
    }

    private IEnumerator _TrackPlayers()
    {
        while (true)
        {
            yield return new WaitForEndOfFrame();

            if (parent == null || child == null)
            {
                Debug.Log("Lost targets");
                yield break;
            }

            Vector3 target = CalculateCurrentPosition(parent, child);
            transform.position = Vector3.Lerp(transform.position, target, 0.05f);

            yield return null;
        }
    }

    public void ApplyCameraShake(CameraShakeType shakeType = CameraShakeType.Destroy)
    {
        switch (shakeType)
        {
            default:

                break;
        }

        animator.SetTrigger("DestroyTrigger");
    }

    public enum CameraShakeType
    {
        Destroy
    }
}



