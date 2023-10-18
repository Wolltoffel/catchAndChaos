using System.Collections;
using UnityEngine;


public class DoorSwitch : MonoBehaviour
{
    [SerializeField] private GameObject leftDoor, rightDoor;
    [SerializeField] private float doorOpenLength = 0.4f;
    private bool isOpen;
    private Vector3 leftDoorOrigin, rightDoorOrigin;
    private Coroutine coroutine;
    private Coroutine openDoor;

    private void Start()
    {
        leftDoorOrigin = leftDoor.transform.position;
        rightDoorOrigin = rightDoor.transform.position;
        Toggle();
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
            if (openDoor != null)
            {
                StopCoroutine(openDoor);
            }
            lTarget = leftDoorOrigin - transform.right * 1;
            rTarget = rightDoorOrigin + transform.right * 1;
        }
        else
        {
            if (openDoor != null)
            {
                StopCoroutine(openDoor);
            }
            openDoor = StartCoroutine(OpenDoorAfter(10));
        }

        coroutine = StartCoroutine(ToggleDoor(leftDoor.transform.position,lTarget,rightDoor.transform.position,rTarget));

        isOpen = !isOpen;
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

    private IEnumerator OpenDoorAfter(float openAfter)
    {
        yield return new WaitForSeconds(openAfter);

        Toggle();
    }
}