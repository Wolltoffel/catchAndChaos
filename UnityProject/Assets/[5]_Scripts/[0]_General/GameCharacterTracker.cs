using UnityEngine;

public class GameCharacterTracker : MonoBehaviour
{
    private Transform parent;
    private Transform child;
    private float minOffset = 10f;

    public void Setup(Transform parent, Transform child)
    {
        this.parent = parent;
        this.child = child;
    }

    private void LateUpdate()
    {
        Vector3 midpoint = (parent.position + child.position) / 2f;
        float distance = Vector3.Distance(parent.position, child.position);
        Vector3 targetPosition = midpoint - transform.forward * (distance / 2f + minOffset);
        transform.position = targetPosition;
    }
}