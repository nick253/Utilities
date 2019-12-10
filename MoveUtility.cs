using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveUtility : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float turnSpeed = 50f;
    public Vector3 point;
    public GameObject[] waypoints;


    public float speed = 0.6f;
    private int count = 0;

    // Start is called before the first frame update
    void Start()
    {
        point = waypoints[0].transform.position;//GameObject.Find("Waypoint").transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey("w"))
        {
            print("w key was pressed");
            transform.Translate(Vector3.forward * Time.deltaTime * 30);
            //transform.Translate(0, 0, Time.deltaTime);
        }
        if (Input.GetKey("a"))
        {
            print("a key was pressed");
            transform.Rotate(Vector3.up, -turnSpeed * Time.deltaTime);
            //transform.Translate(0, 0, Time.deltaTime);
        }
        if (Input.GetKey("s"))
        {
            print("s key was pressed");
            transform.Translate((Vector3.forward * -1) * Time.deltaTime * 15);
            //transform.Translate(0, 0, Time.deltaTime);
        }
        if (Input.GetKey("d"))
        {
            print("d key was pressed");
            transform.Rotate(Vector3.up, turnSpeed * Time.deltaTime);
            //transform.Translate(0, 0, Time.deltaTime);
        }
        if (Input.GetKey("q"))
        {
            print("q key was pressed");

            transform.position = Vector3.MoveTowards(
                transform.position, point, Time.deltaTime * speed);
            //transform.Translate(0, 0, Time.deltaTime);
        }

        // this checks if you have reached a waypoint and then changes point to equal the next waypoint in the list
        // which allows for my gameobject to follow along a path of waypoints uysing the "q" key
        float distanceToTarget = Vector3.Distance(this.transform.localPosition, point);
        if (waypoints.Length > count)
        {
            if (distanceToTarget < 1f)
            {
                point = waypoints[count].transform.position;
                transform.LookAt(point);
                count++;
            }
        }

        transform.LookAt(point);


    }
}