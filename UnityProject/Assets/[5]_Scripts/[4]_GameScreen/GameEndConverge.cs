using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class GameEndConverge : MonoBehaviour
{
    [SerializeField] private Image image;
    private Material material;
    private Coroutine coroutine;

    public Vector2 pos1;
    public Vector2 pos2;

    private void Awake()
    {
        if (image == null)
            return;

        material = image.GetComponent<Image>().material;
        material.SetFloat("_Convergence", 0);
        material.SetVector("_Position_1", new Vector2(0.25f,0.25f));
        material.SetVector("_Position_2", new Vector2(0.75f, 0.75f));

        GetComponent<Canvas>().sortingOrder = -100;
    }

    public void ConvergeOn(Transform transform1, Transform transform2,float time = 1)
    {
        if (material == null)
            return;

        coroutine = StartCoroutine(_Converge(transform1, transform2, time));
    }

    private IEnumerator _Converge(Transform transform1, Transform transform2, float time = 1)
    {
        float progress = 0;

        while (time > 0)
        {
            progress += Time.deltaTime / time;

            pos1 = Camera.main.WorldToScreenPoint(transform1.position+ Vector3.up* 1.2f);
            pos2 = Camera.main.WorldToScreenPoint(transform2.position);

            material.SetVector("_Position_1", pos1);
            material.SetVector("_Position_2", pos2);
            material.SetFloat("_Convergence", progress);

            time -= Time.deltaTime;

            yield return null;
        }

        material.SetFloat("_Convergence", 1);

        coroutine = null;
    }
}
