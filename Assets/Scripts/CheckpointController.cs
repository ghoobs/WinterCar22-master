using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointController : MonoBehaviour
{
    public int checkpoint = -1;
    public Transform lastCheckpoint;
    public int lap = 0;

    public int checkpointCount;
    int nextCheckpoint = 0;


    void Start()
    {
        GameObject[] checkpointObjects = GameObject.FindGameObjectsWithTag("Checkpoint");
        checkpointCount = checkpointObjects.Length;
        for(int i=0; i<checkpointCount; i++)
        {
            if(checkpointObjects[i].name == "0")
            {
                lastCheckpoint = checkpointObjects[i].transform;
                break;
            }
        }
    }


    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Checkpoint"))
        {
            int thisCheckpoint = int.Parse(other.name);
            if(thisCheckpoint == nextCheckpoint)
            {
                checkpoint = thisCheckpoint;
                lastCheckpoint = other.transform;
                if(checkpoint == 0)
                {
                    lap++;
                }
                nextCheckpoint++;
                nextCheckpoint = nextCheckpoint % checkpointCount;
            }
        }

    }
}
