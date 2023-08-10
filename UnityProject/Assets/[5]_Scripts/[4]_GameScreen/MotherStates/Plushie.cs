using System.Collections;
using UnityEngine;

public class Plushie : MonoBehaviour
{
    public bool IsThrowDone { get => coroutine == null; }

    private Coroutine coroutine;

    public void ThrowPlushie()
    {

    }

    public void AttachToTarget(Transform target)
    {
        coroutine = StartCoroutine(LerpToTarget(target));
    }

    private IEnumerator LerpToTarget(Transform target)
    {
        float distance = Vector3.Distance(target.position,transform.position);
        float time = 0;
        Vector3 origin = transform.position;

        while (time < 1)
        {
            transform.position = Vector3.Lerp(origin,target.position,time);

            time += Time.deltaTime * Time.timeScale;

            yield return null;
        }

        transform.position = target.position;
        transform.parent = target;
    }

}