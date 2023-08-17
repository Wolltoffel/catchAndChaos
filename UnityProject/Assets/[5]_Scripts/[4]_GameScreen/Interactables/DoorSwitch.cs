using System.Collections;
using UnityEngine;


public class DoorSwitch : MonoBehaviour
{
    [SerializeField] private GameObject leftDoor, rightDoor;
    [SerializeField] private float doorOpenLength = 0.4f;
    private bool isOpen;
    private Vector3 leftDoorOrigin, rightDoorOrigin;
    private Coroutine coroutine;

    private void Start()
    {
        leftDoorOrigin = leftDoor.transform.position;
        rightDoorOrigin = rightDoor.transform.position;
    }

    public void Toggle()
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }

        Vector3 lTarget = leftDoorOrigin;
        Vector3 rTarget = rightDoorOrigin;

        if (!isOpen)
        {
            lTarget = leftDoorOrigin + Vector3.left * 1;
            rTarget = rightDoorOrigin + Vector3.right * 1;
        }

        coroutine = StartCoroutine(ToggleDoor(leftDoor.transform.position,lTarget,rightDoor.transform.position,rTarget));
    }

    private IEnumerator ToggleDoor(Vector3 lStart, Vector3 lTarget, Vector3 rStart, Vector3 rTarget)
    {
        float time = 0;
        while (time<doorOpenLength)
        {
            leftDoor.transform.position = Vector3.Lerp(lStart,lTarget,time/doorOpenLength);
            rightDoor.transform.position = Vector3.Lerp(rStart,rTarget,time/doorOpenLength);
            time += Time.deltaTime * Time.timeScale;
            yield return null;
        }

        leftDoor.transform.position = lTarget;
        rightDoor.transform.position = rTarget;

        coroutine = null;
    }
}