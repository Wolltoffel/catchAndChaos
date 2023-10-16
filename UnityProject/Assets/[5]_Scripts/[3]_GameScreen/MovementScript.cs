using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Windows;
using static UnityEngine.Rendering.DebugUI;

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
    [SerializeField] Characters character = Characters.Child;
    private float minWallDistanceChild = 0.7f;
    private float minWallDistanceParent = 0.77f;
    private float catchDistance = 5;

    private void Awake()
    {
        catchDistance = GameData.GetData<ParentData>("Parent").catchDistance;

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
        body.AddForce(pushDir * pushForce * hit.moveLength / (1 + body.mass / 50), ForceMode.Impulse);
    }

    #region Movement
    public Vector2 MovePlayer(float xAxis, float yAxis, float speed = 1)
    {
        float yValue = transform.position.y;
        Vector2 axis = new Vector2(xAxis, yAxis);
        axis = axis.magnitude >= 1 ? axis.normalized : axis;
        axis = OptimizeMovement(transform.position, axis);
        axis = AssureMovement(transform.position, axis);

        previousMovement = Vector2.Lerp(previousMovement, axis, 1);
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

        return axis;
    }

    private Vector2 AssureMovement(Vector3 position, Vector2 input)
    {
        if (input.x == 1 || input.y == 1 || input.x == -1 || input.y == -1)
        {
            return OptimizeMovement(position, input);
        }

        return input;
    }

    private Vector2 OptimizeMovement(Vector3 position, Vector2 input)
    {
        float speedMag = input.magnitude;
        Vector3 moveDir = new Vector3(input.x, 0, input.y).normalized;

        //If Door
        RaycastHit hit;
        if (Physics.Linecast(position, position + moveDir, out hit))
        {
            if (hit.collider.gameObject.tag == "Door")
            {
                Vector3 dir = hit.collider.gameObject.transform.position - position;
                dir.y = 0;
                float mag = dir.magnitude;
                if (mag > 0.8f && mag < 2f)
                {
                    dir.Normalize();
                    return new Vector2(dir.x, dir.z) * speedMag;
                }
            }
        }

        //If Wall
        return CalculateOptimizedMovement(input, position); 

    }

    private Vector2 CalculateOptimizedMovement(Vector2 movement, Vector3 position)
    {
        Vector3 RaycastEnd = position + Vector3.up + Vector2ToVector3(movement.normalized) * 4;

        RaycastHit hit;
        Physics.Linecast(position + Vector3.up, position + Vector3.up + Vector2ToVector3(movement.normalized) * 4, out hit, LayerMask.GetMask("Walls"));
        if (hit.collider != null /* && (hit.collider.gameObject.tag == "Walls" || hit.collider.gameObject.tag == "Door" || hit.collider.gameObject.tag == "Vent")*/)
        {
            Vector2 ray = Vector3ToVector2(hit.point - position);
            Vector3 dir = hit.point - position;
            Vector2 dir2 = Vector3ToVector2(dir);
            Vector3 normal = hit.normal;
            Vector2 normal2 = Vector3ToVector2(normal).normalized;

            float distance = ray.magnitude * Vector2.Dot(ray.normalized, normal2.normalized);
            distance = Mathf.Abs(distance);
            distance = Mathf.Clamp01(distance - (minWallDistanceChild * (((int)character + 1)%2)) - (minWallDistanceParent * ((int)character)));
            float influence = 1 - distance;

            float moveMagnitude = movement.magnitude;
            Vector2 cardinalDir = GetCardinalDirection(dir2, normal2);
            Vector2 cardinal = cardinalDir * moveMagnitude;

            Vector2 moveToCardinal = cardinal - movement;
            Vector2 resultdir = (movement + moveToCardinal * influence).normalized;
            Vector2 result = resultdir * moveMagnitude;

            return result;
        }

        return movement;
    }

    private Vector2 GetCardinalDirection(Vector2 dir2, Vector2 normal2)
    {
        normal2 = -normal2;
        float dotProduct = Vector3.Dot(dir2, normal2);
        float magnitudeSquared = normal2.sqrMagnitude;

        Vector2 proj = (dotProduct / magnitudeSquared) * normal2;

        return (dir2 - proj).normalized;
    }
    #endregion

    #region Slide
    public void DoSlide(GameObject vent,float slideDuration = 0.4f)
    {
        coroutine = StartCoroutine(_DoSlide(vent, slideDuration));
    }
    private IEnumerator _DoSlide(GameObject vent,float slideDuration)
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
        ventDir = new Vector3(ventDir.x, 0, ventDir.z).normalized;

        return ventDir * scalar;
    }
    #endregion

    #region Catch
    public void DoCatch()
    {
        coroutine = StartCoroutine(_DoCatch());
    }
    private IEnumerator _DoCatch(float catchDuration = 0.6f)
    {
        float yValue = transform.position.y;
        float time = 0;

        Vector3 catchDir = CalculateCatchDir();
        transform.rotation = Quaternion.LookRotation(catchDir);

        while (time < catchDuration)
        {
            float multiplicator = -Mathf.Pow((time+ 0.5f), 8) + 1;
            Vector3 movement = catchDir * (Mathf.Clamp01(multiplicator)) * Time.deltaTime * 4 * (1/catchDuration);
            characterController.Move(movement * Time.timeScale);

            transform.position = new Vector3(transform.position.x, yValue, transform.position.z);

            time += Time.deltaTime;

            yield return null;
        }

        coroutine = null;
    }

    private Vector3 CalculateCatchDir()
    {
        Vector3 childPos = CharacterInstantiator.GetActiveCharacter(Characters.Child).transform.position;
        Vector3 dir = childPos - transform.position;
        float distance = dir.magnitude;
        dir.Normalize();

        if (distance <= catchDistance && Vector3.Dot(dir, transform.forward) > 0.4f)
        {
            RaycastHit hit;
            LayerMask obstacleLayer;
            obstacleLayer = ~6;
            if (!Physics.Linecast(transform.position, childPos, out hit, obstacleLayer))
            {
                return dir;
            }
        }

        return transform.forward;
    }

    public void StopCatch()
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }
        coroutine = null;
    }
    #endregion

    #region Conversion
    private Vector2 Vector3ToVector2(Vector3 value)
    {
        return new(value.x, value.z);
    }
    private Vector3 Vector2ToVector3(Vector2 value)
    {
        return new(value.x,0, value.y);
    }
    #endregion
}
