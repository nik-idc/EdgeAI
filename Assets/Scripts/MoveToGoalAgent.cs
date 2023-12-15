using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class MoveToGoalAgent : Agent
{
    public CheckpointsManager manager;
    public float shortHeight;
    public float tallHeight;
    public float moveSpeed;

    private bool inAir = false;
    private Vector3 startPos = new();
    private float dragVal = 0;
    private Rigidbody rb;

    private void MoveStraight()
    {
        rb.velocity = new Vector3(moveSpeed, rb.velocity.y, rb.velocity.z);
    }

    private void MoveBack()
    {
        rb.velocity = new Vector3(-moveSpeed, rb.velocity.y, rb.velocity.z);
    }

    private void TurnLeft()
    {
        rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, -moveSpeed);
    }

    private void TurnRight()
    {
        rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, moveSpeed);
    }

    private void Jump()
    {
        if (!inAir)
        {
            inAir = true;
            rb.AddForce(Vector3.up * shortHeight, ForceMode.Impulse);
        }
    }

    private void TallJump()
    {
        if (!inAir)
        {
            inAir = true;
            rb.AddForce(Vector3.up * tallHeight, ForceMode.Impulse);
        }
    }

    private void Regen()
    {
        inAir = false; // No matter when the agent failed we put it back to the ground
        transform.position = startPos; // Put agent back to its initial position
        rb.velocity = new Vector3(0, 0, 0);
        rb.angularVelocity = new Vector3(0, 0, 0);
    }

    private void Start()
    {
        manager.OnCorrectCheckpoint += this.OnCorrectCheckpoint;
        manager.OnWrongCheckpoint += this.OnWrongCheckpoint;

        startPos = transform.position;

        rb = GetComponent<Rigidbody>();
        rb.velocity = new Vector3(0, 0, 0);
        rb.angularVelocity = new Vector3(0, 0, 0);
        dragVal = rb.drag;
    }

    public override void OnEpisodeBegin()
    {
        Regen();
    }

    private void OnCorrectCheckpoint(object sender, Checkpoint checkpoint)
    {
        switch(checkpoint.type)
        {
            case "Checkpoint":
                AddReward(0.02f);
                break;
            case "Vault":
                AddReward(0.035f);
                break;
            case "Fence":
                AddReward(0.05f);
                break;
            case "Elevation":
                AddReward(0.05f);
                break;
            case "Jump":
                AddReward(0.04f);
                break;
            case "Obstacle":
                AddReward(-0.033f);
                break;
        }
    }

    private void OnWrongCheckpoint(object sender, Checkpoint checkpoint)
    {
        switch(checkpoint.type)
        {
            case "Checkpoint":
                AddReward(-0.002f);
                break;
            case "Vault":
                AddReward(-0.0035f);
                break;
            case "Fence":
                AddReward(-0.005f);
                break;
            case "Elevation":
                AddReward(-0.005f);
                break;
            case "Jump":
                AddReward(-0.004f);
                break;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Goal"))
        {
            Debug.Log("Reached the goal yay :)");
            SetReward(10f);
            EndEpisode();
        }
        if (collision.gameObject.CompareTag("Fall Detection"))
        {
            Debug.Log("Fell :(");
            SetReward(-1f);
            EndEpisode();
        }
        if (collision.gameObject.CompareTag("Ground"))
        {
            inAir = false;
        }
    }

    // private void FixedUpdate()
    // {
    //     if (inAir)
    //     {
    //         rb.drag = 0;
    //     }
    //     else
    //     {
    //         rb.drag = dragVal;
    //     }

    //     if (Input.GetKey(KeyCode.D))
    //     {
    //         MoveStraight();
    //     }
    //     if (Input.GetKey(KeyCode.A))
    //     {
    //         MoveBack();
    //     }
    //     if (Input.GetKey(KeyCode.W))
    //     {
    //         TurnLeft();
    //     }
    //     if (Input.GetKey(KeyCode.S))
    //     {
    //         TurnRight();
    //     }

    //     if (Input.GetKeyDown(KeyCode.F))
    //     {
    //         Jump();
    //     }
    //     if (Input.GetKeyDown(KeyCode.G))
    //     {
    //         TallJump();
    //     }
    // }

    // Execute actions based on the agent's decisions
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        // First actions branch: move left/straight/right
        switch (actionBuffers.DiscreteActions[0])
        {
            case 0:
                AddReward(-0.05f);
                TurnLeft();
                break;
            case 1:
                AddReward(0.1f);
                MoveStraight();
                break;
            case 2:
                AddReward(-0.05f);
                TurnRight();
                break;
            case 3:
                AddReward(-0.33f);
                break;
        }
        // Second actions branch: don't jump, jump, jump taller
        switch (actionBuffers.DiscreteActions[0])
        {
            case 0:
                break;
            case 1:
                AddReward(-0.005f);
                Jump();
                break;
            case 2:
                AddReward(-0.01f);
                TallJump();
                break;
        }
    }
}
