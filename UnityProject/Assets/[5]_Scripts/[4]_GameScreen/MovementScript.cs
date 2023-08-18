using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MovementScript : MonoBehaviour
{
    public bool IsCoroutineDone { get => coroutine == null; }

    private CharacterController characterController;
    private Coroutine coroutine;
    private Rigidbody rigidbody;
    private Vector2 previousMovement;

    [SerializeField] private float lerpValue = 0.2f;
    [SerializeField] private float movementSpeed = 10f;
    [SerializeField] private float rotationSpeed = 50f;
    [SerializeField] private float pushForce = 5f;

    private void Awake()
    {
        if (!TryGetComponent(out characterController))
        {
            characterController = gameObject.AddComponent<CharacterController>();
            characterController.slopeLimit = characterController.stepOffset = 0;

        }

        if (!TryGetComponent(out rigidbody))
        {
            rigidbody = gameObject.AddComponent<Rigidbody>();
            rigidbody.useGravity = false;
            rigidbody.constraints = RigidbodyConstraints.FreezePositionY;
            rigidbody.freezeRotation = true;
            rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
        }
    }

    public void MovePlayer(float xAxis, float yAxis, float speed = 1)
    {
        float yValue = transform.position.y;
        Vector2 axis = new Vector2(xAxis, yAxis);
        previousMovement = Vector2.Lerp(previousMovement, axis, lerpValue);
        Vector3 movementDir = (new Vector3(previousMovement.x, 0, previousMovement.y));
        Vector3 movement = movementDir * movementSpeed * speed / 10 * Time.deltaTime * Time.timeScale * 500;
        characterController.Move(movement);

        if (movement.magnitude >= 0.001)
        {
            Quaternion targetRotation = Quaternion.LookRotation(movementDir.normalized);
            float angleDifference = Quaternion.Angle(transform.rotation, targetRotation);
            if (Mathf.Abs(angleDifference) > 4)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, lerpValue * rotationSpeed * Time.deltaTime * speed);
            }
            else
            {
                transform.rotation = targetRotation;
            }
        }
        transform.position = new Vector3(transform.position.x, yValue, transform.position.z);
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody body = hit.collider.attachedRigidbody;

        // no rigidbody
        if (body == null || body.isKinematic)
        {
            return;
        }

        // We dont want to push objects below us
        if (hit.moveDirection.y < -0.3)
        {
            return;
        }

        Vector3 pushDir = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);

        //body.velocity = pushDir * pushForce * hit.moveLength / (1 + body.mass/50);
        body.AddForce(pushDir * pushForce * hit.moveLength / (1 + body.mass/50) , ForceMode.Impulse);
    }

    #region Slide
    public void DoSlide(GameObject vent, float slideDuration = 0.4f)
    {
        coroutine = StartCoroutine(_DoSlide(vent, slideDuration));
    }
    private IEnumerator _DoSlide(GameObject vent, float slideDuration)
    {
        //characterController.enabled = false;
        //rigidbody.isKinematic = true;

        float time = 0;
        Vector3 slideDir = GetSlideDir(vent);

        Vector3 originPos = transform.position;
        Quaternion originRot = transform.rotation;

        Vector3 targetPos = new Vector3(vent.transform.position.x, transform.position.y, vent.transform.position.z) + -slideDir * 1f;
        Quaternion targetRot = Quaternion.LookRotation(slideDir);

        while (time < 0.1f)
        {
            transform.position = Vector3.Lerp(originPos, targetPos, time * 10);
            transform.rotation = Quaternion.Slerp(originRot, targetRot, time * 10);

            time += Time.deltaTime * Time.timeScale;
            yield return null;
        }

        transform.position = targetPos;
        transform.rotation = targetRot;
        time = 0;

        while (time < slideDuration)
        {
            transform.Translate(slideDir * Time.timeScale * 500 * Time.deltaTime * movementSpeed / 10, Space.World);
            time += Time.deltaTime * Time.timeScale;

            yield return null;
        }

        //characterController.enabled = true;
        //rigidbody.isKinematic = false;

        coroutine = null;
    }

    private Vector3 GetSlideDir(GameObject vent)
    {
        Vector3 ventPos = vent.transform.position;
        Vector3 ventDir = vent.transform.forward;
        Vector3 relativePos = Vector3.Normalize(transform.position - ventPos);
        float scalar = Vector3.Dot(relativePos, ventDir) > 0 ? -1 : 1;
        ventDir = new Vector3(ventDir.x,0,ventDir.z).normalized;

        return ventDir * scalar;
    }
    #endregion

    #region Catch
    public void DoCatch()
    {
        coroutine = StartCoroutine(_DoCatch());
    }
    private IEnumerator _DoCatch(float catchDuration = 1f)
    {
        float time = 0;
        Vector3 catchDir = transform.forward;

        while (time < catchDuration)
        {
            float multiplicator = -Mathf.Pow((time+0.5f), 8) + 1;
            Vector3 movement = catchDir * (Mathf.Clamp01(multiplicator)) / 48;
            characterController.Move(movement * Time.timeScale);

            time += Time.deltaTime;

            yield return null;
        }

        coroutine = null;
    }

    public void StopCatch()
    {
        StopCoroutine(coroutine);
        coroutine = null;
    }
    #endregion
}

/*
class OldMovementScript : MonoBehaviour
{
    public bool IsCoroutineDone { get => coroutine == null; }

    private Vector2 previousMovement;
    private Rigidbody rigidbody;
    private Coroutine coroutine;
    private CharacterController characterController;

    [SerializeField] private float lerpValue = 0.2f;
    [SerializeField] private float movementSpeed = 10f;
    [SerializeField] private float rotationSpeed = 50f;

    private void Awake()
    {
        if (!TryGetComponent<Rigidbody>(out rigidbody))
        {
            rigidbody = gameObject.AddComponent<Rigidbody>();
            rigidbody.useGravity = false;
            rigidbody.constraints = RigidbodyConstraints.FreezePositionY;
            rigidbody.freezeRotation = true;
            rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
        }


    }

    public void MovePlayer(float xAxis, float yAxis, float speed = 1)
    {
        Vector2 axis = new Vector2(xAxis, yAxis);
        previousMovement = Vector2.Lerp(previousMovement, axis, lerpValue);
        Vector3 movementDir = (new Vector3(previousMovement.x, 0, previousMovement.y));

        rigidbody.velocity = Vector3.zero;
        if (axis != Vector2.zero)
            rigidbody.velocity = movementDir * movementSpeed * speed;



        if (movementDir != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(movementDir.normalized);
            float angleDifference = Quaternion.Angle(transform.rotation, targetRotation);
            if (Mathf.Abs(angleDifference) > 4)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, lerpValue * rotationSpeed * Time.deltaTime * speed);
            }
            else
            {
                transform.rotation = targetRotation;
            }
        }
    }

    #region Slide
    public void DoSlide(GameObject vent, float slideDuration = 1)
    {
        coroutine = StartCoroutine(_DoSlide(vent, slideDuration));
    }
    private IEnumerator _DoSlide(GameObject vent, float slideDuration = 1)
    {
        float time = 0;
        Vector3 slideDir = GetSlideDir(vent);

        Vector3 originPos = transform.position;
        Quaternion originRot = transform.rotation;

        Vector3 targetPos = vent.transform.position + slideDir * 0.5f;
        Quaternion targetRot = Quaternion.LookRotation(slideDir);

        while (time < 0.1f)
        {
            transform.position = Vector3.Lerp(originPos, targetPos, time * 10);
            transform.rotation = Quaternion.Slerp(originRot, targetRot, time * 10);

            yield return null;
        }

        time = 0;

        while (time < slideDuration)
        {
            transform.Translate(slideDir * Time.deltaTime);

            time += Time.deltaTime * Time.timeScale;

            yield return null;
        }

        coroutine = null;
    }
    private Vector3 GetSlideDir(GameObject vent)
    {
        Vector3 ventPos = vent.transform.position;
        Vector3 ventDir = vent.transform.forward;
        Vector3 relativePos = Vector3.Normalize(transform.position - ventPos);
        float scalar = Vector3.Dot(relativePos, ventDir) > 0 ? 1 : -1;

        return ventDir * scalar;
    }
    #endregion

    #region Catch
    public void DoCatch()
    {
        coroutine = StartCoroutine(_DoCatch());
    }
    private IEnumerator _DoCatch(float catchDuration = 1.5f)
    {
        float time = 0;
        Vector3 catchDir = transform.forward;

        while (time < catchDuration)
        {
            Vector3 movement = catchDir * (Mathf.Pow(time, 8) + 1);

            rigidbody.velocity = movement;

            yield return null;
        }

        coroutine = null;
    }
    #endregion
}
*/