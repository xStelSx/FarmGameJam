using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class CameraMovement : MonoBehaviour
{
    [SerializeField, Min(0.1f)]
    private float speed = 1f;
    [SerializeField]
    private float rotationSpeed = 15;
 
    [SerializeField]
    private float movementTime = 0.1f;
    [SerializeField]
    private Vector3 zoomAmount, zoomLimitClose, zoomLimitFar;

    [SerializeField]
    CinemachineVirtualCamera cameraReference;

    CinemachineTransposer cameraTransposer;
    private Vector3 newZoom;

    private Quaternion targetRotation;

    Vector2 input;

    [SerializeField]
    public int constraintXMax = 5, constraintXMin = -5, constraintZMax = 5, constraintZMin = -5;

    private void Start()
    {
        cameraTransposer = cameraReference.GetCinemachineComponent<CinemachineTransposer>();
        targetRotation = transform.rotation;
        newZoom = cameraTransposer.m_FollowOffset;

    }

    void Update()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        int rotationDirection = 0;
        if (scroll < 0)
            rotationDirection = -7;
        if (scroll > 0)
            rotationDirection = 7;
        targetRotation = transform.rotation * Quaternion.Euler(Vector3.up * rotationDirection * rotationSpeed);

        transform.position += (transform.forward * input.y + transform.right * input.x) * speed * Time.deltaTime;

        transform.position = new Vector3(
            Mathf.Clamp(transform.position.x, constraintXMin, constraintXMax),
            transform.position.y,
            Mathf.Clamp(transform.position.z, constraintZMin, constraintZMax));

        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime / movementTime);
        cameraTransposer.m_FollowOffset = Vector3.Lerp(cameraTransposer.m_FollowOffset, newZoom, Time.deltaTime / movementTime);

    }

    private Vector3 ClampVector(Vector3 newZoom, Vector3 zoomLimitClose, Vector3 zoomLimitFar)
    {
        newZoom.x = Mathf.Clamp(newZoom.x, zoomLimitClose.x, zoomLimitFar.x);
        newZoom.y = Mathf.Clamp(newZoom.y, zoomLimitClose.y, zoomLimitFar.y);
        newZoom.z = Mathf.Clamp(newZoom.z, zoomLimitClose.z, zoomLimitFar.z);
        return newZoom;
    }

}
