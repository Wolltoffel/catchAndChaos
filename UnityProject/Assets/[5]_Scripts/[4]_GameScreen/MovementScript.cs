using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementScript : MonoBehaviour
{
    [SerializeField] private float movementSpeed = 1f;
    [SerializeField] private float rotationSpeed = 2f;
    private Transform obj;
    private Vector2 previousMovement;
    private Rigidbody rigidbody;

    [SerializeField] private float lerpValue = 0.2f;

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
}
