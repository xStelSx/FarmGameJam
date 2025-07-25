using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationMap : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 50f;
    [SerializeField] private float smoothness = 5f;

    private Quaternion targetRotation;
    // Start is called before the first frame update
    void Start()
    {
        targetRotation = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        float mouseWheel = Input.GetAxis("Mouse ScrollWheel");

        if (mouseWheel != 0)
        {
            float rotationAmount = mouseWheel * rotationSpeed;
            targetRotation *= Quaternion.Euler(0f, rotationAmount, 0f);

        }

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            smoothness * Time.deltaTime
            );

    }
}
