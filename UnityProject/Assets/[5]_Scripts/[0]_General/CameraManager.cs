using System.Collections;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private Transform gamePosition;
    [SerializeField] private float minimumZoom;
    [SerializeField] private float minOffset;


    private Transform parent;
    private Transform child;
    private Transform anchor;

    private Coroutine coroutine;


    public void SetCameraAsMain()
    {
        Camera.SetupCurrent(GetComponent<Camera>());
        GetComponent<Camera>().depth = -1;
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
        if (anchor != null)
        {
            EndTrackPlayers();
        }
        anchor = new GameObject().transform;
        var tracker = anchor.gameObject.AddComponent<GameCharacterTracker>();
        tracker.Setup(parent, child);
        anchor.parent = transform;
        anchor.localPosition = Vector3.zero;
        anchor.localRotation = Quaternion.identity;
        anchor.parent = null;
        coroutine = StartCoroutine(_TrackPlayers());
    }
    public void EndTrackPlayers()
    {
        Destroy(anchor.gameObject);
        if (coroutine!= null)
        {
            StopCoroutine(coroutine);
            coroutine = null;
        }
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

            transform.position = Vector3.Lerp(transform.position, anchor.position, 0.05f);

            yield return null;
        }
    }
}


