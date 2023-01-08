using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    float lastTimeMoving = 0;
    CheckpointController checkpointController;

    DrivingScript driveScript;

    private void Start()
    {
        driveScript = GetComponent<DrivingScript>();
        checkpointController = driveScript.rb.GetComponent<CheckpointController>();
    }

    private void Update()
    {
        float acc = Input.GetAxis("Vertical");
        float brake = Input.GetAxis("Jump");
        float steer = Input.GetAxis("Horizontal");

        bool nitro = Input.GetKeyDown(KeyCode.LeftShift);

        if(driveScript.rb.velocity.magnitude > 1 || !RaceController.RacePending)
        {
            lastTimeMoving = Time.time;
        }

        if(Time.time > lastTimeMoving + 3 || driveScript.rb.transform.position.y < 14)
        {
            driveScript.rb.transform.position = checkpointController.lastCheckpoint.position;
            driveScript.rb.transform.rotation = checkpointController.lastCheckpoint.rotation;

            driveScript.rb.velocity = Vector3.zero;
            driveScript.rb.angularVelocity = Vector3.zero;

            lastTimeMoving = Time.time;

            driveScript.rb.gameObject.layer = 6;
            Invoke(nameof(ResetLayer), 3);
        }

        if (RaceController.RacePending != true)
        {
            acc = 0;
        }
        else
        {
            driveScript.NitroBoost(nitro);
        }

        driveScript.Drive(acc, brake, steer);
    }

    void ResetLayer()
    {
        driveScript.rb.gameObject.layer = 0;
    }
}
