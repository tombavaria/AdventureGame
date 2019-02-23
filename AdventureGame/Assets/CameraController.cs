using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{

    public Transform target;
    public float rotationDamping = 1f;
    public float heightDamping = 15f;
    public float heightAimMultiplier = 1f;

    Vector3 targetOffset = Vector3.zero;
    Vector3 offset;
    float heightDiff;

    // Use this for initialization
    void Start()
    {
        offset = target.position - transform.position;
        // Zero out the height offset
        offset = new Vector3(offset.x, 0f, offset.z);
        float targetHeight = target.GetComponent<CharacterController>().height;
        targetOffset = new Vector3(0f, targetHeight * heightAimMultiplier, 0f);
        heightDiff = transform.position.y - target.position.y;
    }

    // Update is called once per frame
    void Update()
    {

    }

    // LateUpdate is called after Update
    void LateUpdate()
    {
        float currentAngle = transform.eulerAngles.y;
        float desiredAngle = target.eulerAngles.y;
        float currentHeight = transform.position.y;
        float desiredHeight = target.position.y + heightDiff;
        float angle = Mathf.LerpAngle(currentAngle, desiredAngle, Time.deltaTime * rotationDamping);
        Quaternion rotation = Quaternion.Euler(0, angle, 0);
        transform.position = target.position - (rotation * offset);

        // Set height
        transform.position = new Vector3(transform.position.x, Mathf.Lerp(currentHeight, desiredHeight, Time.deltaTime * heightDamping), transform.position.z);

        // Look at target + offset
        transform.LookAt(target.position + targetOffset);
    }
}