using UnityEngine;
using UnityEngine.UIElements;

public class GameCharacterTracker : MonoBehaviour
{
    private Camera dummyCamera;
    private Transform parent;
    private Transform child;
    public float minZoom = 25f;
    public float maxZoom = 70f;
    public float maxDistance = 42;
    public float minDistance = 10;

    public void Setup(Transform parent, Transform child)
    {
        this.parent = parent;
        this.child = child;
    }

    private void LateUpdate()
    {
        float worldDistance = Vector3.Distance(parent.position, child.position);
        float distanceToMidpoint = (worldDistance * 0.5f) / Mathf.Tan(Mathf.Deg2Rad * 15);
        Vector3 midpoint = (parent.position + child.position) / 2f ;

        float value = (worldDistance - minDistance) / (maxDistance - minDistance);
        float distance = ((maxZoom - minZoom)*value) + minZoom;

        Vector3 targetPosition = midpoint - transform.forward * Mathf.Clamp(distance, minZoom, maxZoom);
        transform.position = targetPosition;
    }

    /*Vector3 parentScreenPoint = Camera.main.WorldToScreenPoint(parent.position);
        Vector3 childScreenPoint = Camera.main.WorldToScreenPoint(child.position);
        float screenDistance = Vector2.Distance(parentScreenPoint, childScreenPoint);
        float worldDistance = Vector3.Distance(parent.position,child.position);
        Vector3 distanceDir = new Vector3(parent.position.x - child.position.x,0, parent.position.z - child.position.z);


        float distanceDot = Mathf.Abs(distanceDir.normalized.z);
        distanceDot /= 4;
        float aspectRatio = (float) Screen.width / Screen.height;

        float t = aspectRatio * distanceDot;


        //float distanceMultiplier = screenDistance / 1.6f;
        //worldDistance *= distanceMultiplier;

        Debug.Log(distanceDot);

        float distanceToMidpoint = (worldDistance * 0.5f) / Mathf.Tan(Mathf.Deg2Rad * 15) * (t);
        Vector3 midpoint = (parent.position + child.position) / 2f + Vector3.up;

        Vector3 targetPosition = midpoint - transform.forward * Mathf.Clamp(distanceToMidpoint,minOffset,maxOffset);
        transform.position = targetPosition;*/
}