using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CheckpointsManager : MonoBehaviour
{
    public event EventHandler<Checkpoint> OnCorrectCheckpoint;
    public event EventHandler<Checkpoint> OnWrongCheckpoint;
    private void Awake()
    {
        // Loop through map objects
        foreach (Transform children in transform)
        {
            // Loop through objects children and set checkpoints
            foreach (Transform obj in children)
            {
                if (obj.GetComponent<Checkpoint>() != null)
                {
                    Checkpoint checkpoint = obj.GetComponent<Checkpoint>();
                    checkpoint.SetCheckpointsManager(this);
                }
            }
        }
    }

    public void AgentThroughCheckpoint(Checkpoint checkpoint)
    {
        if (!checkpoint.GetWentThrough())
        {
            OnCorrectCheckpoint.Invoke(this, checkpoint);
            checkpoint.SetWentThrough(true);
        }
        else
        {
            OnWrongCheckpoint.Invoke(this, checkpoint);
        }
    }
}
