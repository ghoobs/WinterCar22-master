using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DriveSupport : MonoBehaviour
{
    public float antiRoll = 5000;
    float lastTimeOK;

    Rigidbody rb;

    [Header("0 - lewe ko³o, 1 - prawe ko³o")]
    public WheelCollider[] frontWheels = new WheelCollider[2];
    public WheelCollider[] backWheels = new WheelCollider[2];

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if(transform.up.y > 0.5 || rb.velocity.magnitude > 1)
        {
            lastTimeOK = Time.time;
        }

        if(Time.time > lastTimeOK + 3)
        {
            TurnCarBackOn();
        }
    }

    private void FixedUpdate()
    {
        HoldWheelsOnGround(frontWheels);
        HoldWheelsOnGround(backWheels);
    }

    void TurnCarBackOn()
    {
        transform.position += Vector3.up * 2;
        transform.rotation = Quaternion.LookRotation(transform.forward);
    }

    void HoldWheelsOnGround(WheelCollider[] wheels)
    {
        float leftTravel = 1;
        float rightTravel = 1;

        WheelHit hit;
        bool groundedL = wheels[0].GetGroundHit(out hit);

        if (groundedL)
        {
            leftTravel =
                (-wheels[0].transform.InverseTransformPoint(hit.point).y -
                wheels[0].radius) / wheels[0].suspensionDistance;
        }

        bool groundedR = wheels[1].GetGroundHit(out hit);

        if (groundedR)
        {
            rightTravel =
                (-wheels[1].transform.InverseTransformPoint(hit.point).y -
                wheels[1].radius) / wheels[1].suspensionDistance;
        }

        float antiRollForce = (leftTravel - rightTravel) * antiRoll;

        if (groundedL)
        {
            rb.AddForceAtPosition(wheels[0].transform.up * -antiRollForce,
                wheels[0].transform.position);
        }

        if (groundedR)
        {
            rb.AddForceAtPosition(wheels[1].transform.up * antiRollForce,
                wheels[1].transform.position);
        }
    }
}
