using System;
using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] private float minWallDistance = 1f;

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
        Vector2 axis = OptimizeMovement(transform.position,new Vector2(xAxis, yAxis));
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

    private Vector2 OptimizeMovement(Vector3 position, Vector2 input)
    {
        float speedMag = input.magnitude;
        Vector3 moveDir = new Vector3(input.x, 0, input.y).normalized;

        RaycastHit hit;
        if (Physics.Linecast(position, position + moveDir, out hit))
        {
            if (hit.collider.gameObject.tag == "Door")
            {
                Vector3 dir = hit.collider.gameObject.transform.position - position;
                dir.y = 0;
                float mag = dir.magnitude;
                //Debug.Log(dir.magnitude);
                pos = transform.position + Vector3.up;
                loc = pos + dir;
                if (mag > 0.8f && mag < 2f)
                {
                    dir.Normalize();
                    return new Vector2(dir.x, dir.z) * speedMag;
                }

            }

        }

        Physics.Linecast(position + Vector3.up, position + Vector3.up + moveDir * 3, out hit, LayerMask.GetMask("Walls"));
        if (hit.collider != null && (hit.collider.gameObject.tag == "Walls") || hit.collider.gameObject.tag == "Door" || hit.collider.gameObject.tag == "Vent")
        {
            Vector3 dir = hit.point - position;
            Vector2 dir2 = new(dir.x, dir.y);
            Vector3 normal = hit.normal;
            Vector2 normal2 = new(normal.x, normal.z);

            float distance = dir2.magnitude * Vector2.Dot(dir2.normalized,normal2.normalized);
            distance = Mathf.Clamp01(distance - minWallDistance);
            float influence = 1 / (distance * distance);



        }

        //Physics.Linecast(position + Vector3.up, position + Vector3.up + moveDir * 4, out hit, LayerMask.GetMask("Walls"));
        //loc = position;
        //pos = position + moveDir * 4;
        //if (hit.collider != null && (hit.collider.gameObject.tag == "Walls" || hit.collider.gameObject.tag == "Door" || hit.collider.gameObject.tag == "Vent"))
        //{
        //    Vector3 dir = hit.point - position;
        //    Vector2 dir2 = new(dir.x, dir.y);
        //    Vector3 normal = hit.normal;
        //    Vector2 normal2 = new(normal.x, normal.z);

        //    Vector2 reflectedVector = Vector2.Reflect(dir2, normal2);

        //    Vector2 reflectedDirection = reflectedVector.normalized;

        //    float influence = 1 - Mathf.Max(Mathf.Min(hit.distance-0.75f, 1),0);

        //    Vector2 scaledInput = (input + reflectedDirection*influence);

        //    Debug.Log("Input"+input);
        //    Debug.Log("ReflectedInput" + reflectedVector);
        //    Debug.Log("Normal" + normal2);
        //    Debug.Log("Added" + scaledInput);
        //    Debug.Log("influence:" + influence);
        //    // Return the scaled and adjusted movement vector
        //    return scaledInput.magnitude < 0.2f ? Vector2.zero : scaledInput;
        //}
        //if (hit.collider.gameObject.tag == "Walls" || hit.collider.gameObject.tag == "Vent")
        //{

        //    Vector3 dir = hit.point - position;
        //    Vector3 normal = hit.normal;
        //    Vector2 normal2 = new(normal.x, normal.z);
        //    float dot = MathF.Abs(Vector3.Dot(dir, normal));

        //    //DEBUG
        //    //pos = position;
        //    //loc = pos + new Vector3(normal.x * dot + input.x, normal.y, normal.z * dot + input.y);

        //    Vector2 ret = (input + normal2 * dot);
        //    return ret.magnitude < 0.2 ? Vector2.zero : ret * speedMag;
        //}

        return input;

        //DirectionalWeights surrounding = Spacializer.CalculateSurroundings(position+ Vector3.up);
        //float xInput = input.x;
        //float yInput = input.y;

        //Debug.Log(xInput + " - "+ yInput);

        //if (xInput > 0)
        //{
        //    xInput = xInput * surrounding.east;
        //}
        //else if (xInput < 0)
        //{
        //    xInput = xInput * surrounding.west;
        //}
        //if (yInput > 0)
        //{
        //    yInput = yInput * surrounding.north;
        //}
        //else if (yInput < 0)
        //{
        //    yInput = yInput * surrounding.south;
        //}

        //Debug.Log(xInput + " - " + yInput);

        //return new Vector2(xInput,yInput).normalized;
    }

    Vector3 pos = Vector3.zero;
    Vector3 loc = Vector3.zero;

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(pos, loc);
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
        float yValue = transform.position.y;
        float time = 0;

        Vector3 catchDir = CalculateCatchDir();

        while (time < catchDuration)
        {
            float multiplicator = -Mathf.Pow((time+0.5f), 8) + 1;
            Vector3 movement = catchDir * (Mathf.Clamp01(multiplicator)) / 28;
            characterController.Move(movement * Time.timeScale);

            transform.position = new Vector3(transform.position.x, yValue, transform.position.z);

            time += Time.deltaTime;

            yield return null;
        }

        coroutine = null;
    }

    private Vector3 CalculateCatchDir()
    {
        Vector3 dir = CharacterInstantiator.GetActiveCharacter(Characters.Child).transform.position - transform.position;
        dir.Normalize();
        if (Vector3.Dot(dir,transform.forward) > 0)
        {
            return dir;
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
}

static class Spacializer
{
    static public DirectionalWeights CalculateSurroundings(Vector3 position)
    {
        DirectionalWeights directionalWeights = new DirectionalWeights();

        float checkDistance = 2;

        for (int i = 0; i < directionalWeights.directionCount; i++)
        {
            RaycastHit hit;
            bool hasHit = Physics.Linecast(position, position + new Vector3(directionalWeights.directions[i].y, 0, directionalWeights.directions[i].x) * checkDistance,out hit);
            directionalWeights.weights[i] = hasHit? hit.distance / checkDistance : 1;
        }
        return directionalWeights;
    }
}

class DirectionalWeights
{
    //public DirectionalWeights(int resolution)
    //{
    //    if (resolution < 4)
    //        resolution = 4;


    //}

    public float[] weights = new float[8];
    public Vector2[] directions = new Vector2[8]
    {
        new Vector2(1,0),
        new Vector2(1,1).normalized,
        new Vector2(0,1),
        new Vector2(-1,1).normalized,
        new Vector2(-1,0),
        new Vector2(-1,-1).normalized,
        new Vector2(0,-1),
        new Vector2(1,-1).normalized,
    };

    public float directionCount { get => weights.Length; }

    public float north { get => weights[0]; set => weights[0] = Mathf.Clamp01(value); }
    public float south { get => weights[4]; set => weights[4] = Mathf.Clamp01(value); }
    public float west { get => weights[6]; set => weights[6] = Mathf.Clamp01(value); }
    public float east { get => weights[2]; set => weights[2] = Mathf.Clamp01(value); }
    public float northWest { get => weights[7]; set => weights[7] = Mathf.Clamp01(value); }
    public float northEast { get => weights[1]; set => weights[1] = Mathf.Clamp01(value); }
    public float southEast { get => weights[3]; set => weights[3] = Mathf.Clamp01(value); }
    public float southWest { get => weights[5]; set => weights[5] = Mathf.Clamp01(value); }
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