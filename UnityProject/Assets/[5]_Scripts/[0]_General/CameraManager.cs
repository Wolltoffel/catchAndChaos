using System.Collections;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private Transform gamePosition;
    [SerializeField] private float minimumZoom;
    [SerializeField] private float minOffset;


    private Transform parent;
    private Transform child;

    private Coroutine coroutine;

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
        Transform anchor = new GameObject().transform;
        var tracker = anchor.gameObject.AddComponent<GameCharacterTracker>();
        tracker.Setup(parent, child);
        anchor.parent = transform;
        anchor.localPosition = Vector3.zero;
        anchor.localRotation = Quaternion.identity;
        anchor.parent = null;
        coroutine = StartCoroutine(_TrackPlayers(anchor));
    }
    public void EndTrackPlayers()
    {
        StopCoroutine(coroutine);
        coroutine = null;
    }

    private IEnumerator _TrackPlayers(Transform anchor)
    {
        while (true)
        {
            if (parent == null || child == null)
            {
                Debug.Log("Lost targets");
                yield break;
            }

            transform.position = Vector3.Lerp(transform.position, anchor.position, 0.05f);

            yield return null;
        }
    }
}


