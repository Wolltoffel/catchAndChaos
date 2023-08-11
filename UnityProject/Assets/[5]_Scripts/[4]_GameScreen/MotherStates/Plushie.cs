using System.Collections;
using UnityEngine;

public class Plushie : MonoBehaviour
{
    public bool IsThrowDone { get => coroutine == null; }

    private Coroutine coroutine;
    private Transform target;

    public void ThrowPlushie()
    {
        if (coroutine != null)
        {
            transform.position = target.position;
            transform.parent = target;
        }

        StopCoroutine(coroutine);
        coroutine = StartCoroutine(_ThrowPlushie());
    }

    private IEnumerator _ThrowPlushie()
    {
        Debug.Log("Plushie Thrown");
        transform.parent = null;
        yield return null;
    }

    public void AttachToTarget(Transform target)
    {
        this.target = target;
        coroutine = StartCoroutine(_AttachToTarget(target));
    }

    private IEnumerator _AttachToTarget(Transform target, float duration = 0.1f)
    {
        float distance = Vector3.Distance(target.position,transform.position);
        float time = 0;
        Vector3 origin = transform.position;

        while (time < duration)
        {
            transform.position = Vector3.Lerp(origin,target.position,time);

            time += Time.deltaTime * Time.timeScale;

            yield return null;
        }

        transform.position = target.position;
        transform.parent = target;
    }

}