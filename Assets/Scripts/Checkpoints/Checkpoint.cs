using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Policies;
using Unity.VisualScripting;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public string type = "Checkpoint";
    private bool wentThrough = false;
    private CheckpointsManager manager;

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.GetComponent<MoveToGoalAgent>() != null)
        {
            manager.AgentThroughCheckpoint(this);
        }
    }

    public void SetCheckpointsManager(CheckpointsManager manager)
    {
        this.manager = manager;
    }

    public void SetWentThrough(bool wentThrough)
    {
        this.wentThrough = wentThrough;
    }

    public bool GetWentThrough()
    {
        return wentThrough;
    }
}
