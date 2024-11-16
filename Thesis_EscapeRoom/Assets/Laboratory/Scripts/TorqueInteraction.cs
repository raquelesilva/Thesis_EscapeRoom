using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TorqueInteraction : MonoBehaviour
{
    public float torqueAmount = 10f; // Amount of torque to apply
    private Rigidbody rb;

    Vector3 initialLocalPosition;
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Freeze rotation around X and Y axes
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY;
        initialLocalPosition = this.transform.localPosition;
    }

    private void Update()
    {
        rb.transform.localEulerAngles = new(0, 0, rb.transform.localEulerAngles.z);
        rb.transform.localPosition = initialLocalPosition;
    }

    public void ApplyTorque()
    {
        Vector3 torque = Vector3.forward * torqueAmount;
        rb.AddTorque(torque);
    }
}