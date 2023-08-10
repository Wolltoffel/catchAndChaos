using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementScript : MonoBehaviour
{
    public bool IsSlideDone { get => coroutine == null; }

    private Transform obj;
    private Vector2 previousMovement;
    private Rigidbody rigidbody;
    private Coroutine coroutine;

    [SerializeField] private float lerpValue = 0.2f;
    [SerializeField] private float movementSpeed = 1f;
    [SerializeField] private float rotationSpeed = 2f;

    private void Awake()
    {
        obj = GetComponent<Transform>();

        if (!TryGetComponent<Rigidbody>(out rigidbody))
        {
            rigidbody = gameObject.AddComponent<Rigidbody>();
            rigidbody.useGravity = false;
            rigidbody.freezeRotation = true;
            rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
        }        
    }

    //private void FixedUpdate()
    //{
    //    MovePlayer(Input.GetAxis("K1 Horizontal"),Input.GetAxis("K1 Vertical"));
    //}

    public void MovePlayer(float xAxis, float yAxis, float speed = 1)
    {
        Vector2 axis = new Vector2(xAxis,yAxis);
        previousMovement = Vector2.Lerp(previousMovement,axis,lerpValue);
        Vector3 movementDir = (new Vector3(previousMovement.x, 0, previousMovement.y));
        rigidbody.velocity = movementDir * movementSpeed * Time.deltaTime * 1000 * speed;

        if (Vector3.Distance(movementDir,Vector3.zero) > 0.01)
        {
            Quaternion targetRotation = Quaternion.LookRotation(movementDir.normalized);
            float angleDifference = Quaternion.Angle(transform.rotation, targetRotation);
            if (angleDifference > 3)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, lerpValue * rotationSpeed * Time.deltaTime* speed);
            }
            else
            {
                transform.rotation = targetRotation;
            }
        }
    }

    public void DoSlide(GameObject vent, float slideDuration = 1)
    {
        coroutine = StartCoroutine(_DoSlide(vent, slideDuration));
    }
    public IEnumerator _DoSlide(GameObject vent, float slideDuration = 1)
    {
        float time = 0;
        Vector3 slideDir = GetSlideDir(vent);

        while (time < 0.1f)
        {
            Vector3 originPos = transform.position;
            Quaternion originRot = transform.rotation;

            Vector3 targetPos = vent.transform.position + slideDir * 0.5f;
            Quaternion targetRot = Quaternion.LookRotation(slideDir);

            transform.position = Vector3.Lerp(originPos, targetPos,time*10);
            transform.rotation = Quaternion.Slerp(originRot,targetRot,time*10);

            yield return null;
        }

        time = 0;

        while (time<slideDuration)
        {
            transform.Translate(slideDir * Time.deltaTime);

            time += Time.deltaTime * Time.timeScale;

            yield return null;
        }

        coroutine = null;
    }
    public Vector3 GetSlideDir(GameObject vent)
    {
        Vector3 ventPos = vent.transform.position;
        Vector3 ventDir = vent.transform.forward;
        Vector3 relativePos = Vector3.Normalize(transform.position - ventPos);
        float scalar = Vector3.Dot(relativePos, ventDir) > 0 ? 1 : -1;

        return ventDir * scalar;
    }
}
