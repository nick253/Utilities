using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;
using MLAgents;

public class UtilityAgent : Agent
{
    //public float speed = 10;
    public bool contribute = true;
    public bool useVectorObs = true;
    Rigidbody rBody;

    // Speed of agent rotation.
    public float turnSpeed = 100;

    // Speed of agent movement.
    public float moveSpeed = 1f;
    public float speedLimit = 2f;
    public int health;

    [HideInInspector]
    public Rigidbody agentRb;
    Renderer AgentRenderer;
    private RayPerception rayPer;
    public int spawnLocation;
    public GameObject AgentSpawn;

    public GameObject ShootingAgent;

    public override void InitializeAgent()
    {
        base.InitializeAgent();
        agentRb = GetComponent<Rigidbody>();
        Monitor.verticalOffset = 1f;
        rayPer = GetComponent<RayPerception>();
        rBody = GetComponent<Rigidbody>();
        //speedLimit = moveSpeed * 1f;
    }

    public Transform Target;

    public override void AgentReset()
    {
        if (this.transform.localPosition.y < 0)
        {
            // If the Agent fell, zero its momentum
            this.rBody.angularVelocity = Vector3.zero;
            this.rBody.velocity = Vector3.zero;
            this.transform.localPosition = new Vector3(0, 0.5f, 0);
        }

        // Move the target to a new spot
        //The below code moves the Target to a random location near the center of the map 
        Target.localPosition = new Vector3(Random.value * 4 - 4, 0.5f, Random.value * 4 - 4);

        // the below code will reset the Agents locations/roation when it finds a reward.
        this.rBody.angularVelocity = Vector3.zero;
        this.rBody.velocity = Vector3.zero;
        this.transform.localPosition = AgentSpawn.transform.localPosition;//SpawnVector(spawnLocation);//Random.Range(0, 4));//)SpawnVector(4);

        health = 100;
        moveSpeed = 1f;

        //print("Reward Collector Reset");
    }

    //this code spawns the agent randomly at one of the four corners of the map.
    public Vector3 SpawnVector(int num)
    {
        if (num == 1)
        {
            return new Vector3(4, 0.1f, -3);
        }
        else if (num == 2)
        {
            return new Vector3(2, 0.1f, 4);
        }
        else if (num == 3)
        {
            return new Vector3(4, 0.1f, 4);
        }
        else if (num == 4)
        {
            return new Vector3(4, 0.1f, 0);
        }
        else
        {
            return new Vector3(4, 0.1f, 0);
        }

    }

    public void TakeDamage(int damage)
    {
        health = health - damage;
    }

    public void MoveAgent(float[] act)
    {
        var dirToGo = Vector3.zero;
        var rotateDir = Vector3.zero;

        var forwardAxis = (int)act[0];
        var rightAxis = (int)act[1];
        var rotateAxis = (int)act[2];

        switch (forwardAxis)
        {
            case 1:
                dirToGo = transform.forward;
                break;
            case 2:
                dirToGo = -transform.forward;
                break;
        }

        switch (rightAxis)
        {
            case 1:
                dirToGo = transform.right;
                break;
            case 2:
                dirToGo = -transform.right;
                break;
        }

        switch (rotateAxis)
        {
            case 1:
                rotateDir = -transform.up;
                break;
            case 2:
                rotateDir = transform.up;
                break;
        }

        if (agentRb.velocity.sqrMagnitude > 25f) // slow it down
        {
            agentRb.velocity *= 0.95f;
        }

        agentRb.AddForce(dirToGo * moveSpeed, ForceMode.VelocityChange);
        transform.Rotate(rotateDir, Time.fixedDeltaTime * turnSpeed);


    }



    //What would you need to calculate a solution to an analytical problem?
    public override void CollectObservations()
    {
        // Target and Agent positions
        if (useVectorObs)
        {
            //Here I am setting up the rays that will beam out from the agent. We must set the angles and distances we want the rays to face.
            const float rayDistance = 3f;
            float[] rayAngles = { 20f, 90f, 160f, 45f, 135f, 70f, 110f };
            float[] rayAngles1 = { 25f, 95f, 165f, 50f, 140f, 75f, 115f };
            //float[] rayAngles2 = {15f, 85f, 155f, 40f, 130f, 65f, 105f};
            string[] detectableObjects = { "reward", "wall", "wallOuter", "agent" };
            // These are the tags that the Agent will be looking for whenb detecting objects

            //Below we are adding Observation data the agent can use to learn how to win the game
            AddVectorObs(rayPer.Perceive(rayDistance, rayAngles, detectableObjects, 0f, 0f));
            AddVectorObs(rayPer.Perceive(rayDistance, rayAngles1, detectableObjects, 0f, .8f));
            //AddVectorObs(rayPer.Perceive(rayDistance, rayAngles2, detectableObjects, 0f, 10f));
            AddVectorObs(transform.InverseTransformDirection(agentRb.velocity));

            AddVectorObs(Target.localPosition);
            AddVectorObs(this.transform.localPosition);

            // Agent velocity
            AddVectorObs(rBody.velocity.x);
            AddVectorObs(rBody.velocity.z);
        }
    }


    public override void AgentAction(float[] vectorAction, string textAction)
    {
        //Here I am giving the agent a speedboost once it has taken damage from the other player
        if (health != 100 && moveSpeed < speedLimit)
        {
            moveSpeed = moveSpeed + 0.01f;
        }
        if (health < 10)
        {
            print("50 hp left");
        }
        //if health falls below 50% the agents speed is increased
        else if (health < 50)
        {
            print("50% hp");
        }
        // If health falls below 0 the agent dies and reset is called
        if (health <= 0)
        {
            ShootingAgent.GetComponent<ShootingAgent>().mediumReward(1f);
            AgentReset();
            ShootingAgent.GetComponent<ShootingAgent>().AgentReset();
            print("0 health, RewardCollector died");


        }
        //Here the agent is calling the MoveAgent Function and feeding in a vectorAction
        MoveAgent(vectorAction);

        // Rewards
        float distanceToTarget = Vector3.Distance(this.transform.localPosition, Target.localPosition);

        // Reached target
        // Below I am rewarding the agent a small reward as when it gets nearby the reward and another bigger reward when it gets even closer
        if (distanceToTarget < 2.5f)
        {
            AddReward(1.0f / (agentParameters.maxStep / 2.0f));
            print("super close reward distanceToTarget");
            //Done();
        }
        else if (distanceToTarget < 4f)
        {
            AddReward(1.0f / (agentParameters.maxStep / 1.5f));
            print("very close reward distanceToTarget");
            //Debug.Log(".2 reward!");g
            //Done();
        }
        else
        {

        }

        // Fell off platform reset
        if (this.transform.localPosition.y < 0)
        {
            Done();
        }

        // If target spawns on a wall reset
        if (Target.transform.localPosition.y > .5)
        {
            Done();
        }

        // Tiny negative reward every step
        // This incentivizes the agent to want to retrieve the reward as efficiently as possible as every extra step taken is an additional penalty.
        AddReward(-1f / agentParameters.maxStep);

    }

    //This is where the player controls are setup. This is not where you assign controls for the brain to control.
    public override float[] Heuristic()
    {
        var action = new float[4];
        if (Input.GetKey(KeyCode.D))
        {
            action[2] = 2f;
        }
        if (Input.GetKey(KeyCode.W))
        {
            action[0] = 1f;
        }
        if (Input.GetKey(KeyCode.A))
        {
            action[2] = 1f;
        }
        if (Input.GetKey(KeyCode.S))
        {
            action[0] = 2f;
        }
        //action[3] = Input.GetKey(KeyCode.Space) ? 1.0f : 0.0f;
        return action;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("reward"))
        {
            SetReward(2f);
            print("2 reward!");
            ShootingAgent.GetComponent<ShootingAgent>().punish();
            Done();
            //AgentReset();
            if (contribute)
            {
                //myAcademy.totalScore += 1;
            }
        }
        if (collision.gameObject.CompareTag("wallOuter"))
        {
            // small penalty for getting hit with a laser
            AddReward(-.02f);
            print("hit outer wall -.02");

        }
        if (collision.gameObject.CompareTag("wall"))
        {
            // medium penalty for colliding with an outer wall tagged 'wall'
            AddReward(-.01f);
            print("-.01 Hit an outer wall");
            if (contribute)
            {
                //myAcademy.totalScore -= 1;
            }
        }
        //if (collision.gameObject.CompareTag("laser"))
        //{
        //    AgentReset();
        //    print("I've been hit by a laser");
        //}
    }
}

