using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public GameObject manager; // the manager of the object
    public Vector2 location = Vector2.zero; // location
    public Vector2 velocity; // velocity
    Vector2 goalPos = Vector2.zero; // goal position
    Vector2 currentForce; // force

    // Start is called before the first frame update
    void Start()
    {
        velocity = new Vector2(Random.Range(0.01f, 0.1f), Random.Range(0.01f, 0.1f));
        location = new Vector2(this.gameObject.transform.position.x, this.gameObject.transform.position.y); // moving the unit
    }

    Vector2 seek(Vector2 target)
    {
        return (target - location); // just the difference of the target and location
    }

    void applyForce(Vector2 f) // pushes the unit with a certain amount of force
    {
        //Vector3 force = new Vector3(f.x, f.y, 0);
        //this.GetComponent<Rigidbody2D>().AddForce(force);
        //Debug.DrawRay(this.transform.position, force, Color.white); // drawing arrows for the force applied

        Vector3 force = new Vector3(f.x, f.y, 0);
        if (force.magnitude > manager.GetComponent<AllUnits>().maxforce) // if the force is greater than threshold
        {
            force = force.normalized; // normalize the force
            force *= manager.GetComponent<AllUnits>().maxforce; // scale it to the magnitude of the threshold
        }
        this.GetComponent<Rigidbody2D>().AddForce(force); // apply the force to the unit

        if (this.GetComponent<Rigidbody2D>().velocity.magnitude > manager.GetComponent<AllUnits>().maxvelocity) // if the velocity is greater than the threshold
        {
            this.GetComponent<Rigidbody2D>().velocity = this.GetComponent<Rigidbody2D>().velocity.normalized; // normalize the velocity
            this.GetComponent<Rigidbody2D>().velocity *= manager.GetComponent<AllUnits>().maxvelocity; // scale it to the magnitude of the threshold
        }
        Debug.DrawRay(this.transform.position, force, Color.white); // debugging to see the force applied to each unit
    }

    void setSeparation(float radius)
    {
        this.GetComponent<CircleCollider2D>().radius = radius;
    }

    // aligning: move in the same direction
    Vector2 align()
    {
        float neighbordist = manager.GetComponent<AllUnits>().neighbourDistance; // gets hold of the distance of the furtherest neighbor
        Vector2 sum = Vector2.zero; // sum kernel
        int count = 0; // counting how many bids are close by
        foreach (GameObject other in manager.GetComponent<AllUnits>().units)
        {
            if (other == this.gameObject) continue;
            float d = Vector2.Distance(location, other.GetComponent<Unit>().location); // distance from the current boid to the other boid
            if (d < neighbordist) // only care if the distance is less than the threshold
            {
                sum += other.GetComponent<Unit>().velocity;
                count++;
            }
        }
        if (count > 0)
        {
            sum /= count; // average
            Vector2 steer = sum - velocity; // average heading of the group - our current velocity
            return steer;
        } else
        {
            return Vector2.zero;
        }
    }

    // cohesion: staying within the group
    Vector2 cohesion()
    {
        float neighbordist = manager.GetComponent<AllUnits>().neighbourDistance; // gets hold of the distance of the furtherest neighbor
        Vector2 sum = Vector2.zero; // sum kernel
        int count = 0; // counting how many bids are close by
        foreach (GameObject other in manager.GetComponent<AllUnits>().units)
        {
            if (other == this.gameObject) continue;
            float d = Vector2.Distance(location, other.GetComponent<Unit>().location); // distance from the current boid to the other boid
            if (d < neighbordist) // only care if the distance is less than the threshold
            {
                sum += other.GetComponent<Unit>().location;
                count++;
            }
        }
        if (count > 0)
        {
            sum /= count; // average
            return seek(sum); // seeking the vector that takes the boid to the centroid of the neighbor
        }
        else // no neighbors
        {
            return Vector2.zero; // don't do anything
        }
    }

    void flock()
    {
        //// without the more complex flocking algorithm
        //location = this.transform.position; 
        //velocity = this.GetComponent<Rigidbody2D>().velocity;

        //Vector2 gl; // goal location
        //gl = seek(goalPos); // returned by seek (vector from current location to goal location)
        //currentForce = gl; // the direction of force is applied towards the goal position
        ////currentForce = currentForce.normalized; // making the vector a unit vector

        //applyForce(Vector3.Normalize(currentForce)); // apply the normalized unit force vector

        location = this.transform.position;
        velocity = this.GetComponent<Rigidbody2D>().velocity;

        if (manager.GetComponent<AllUnits>().obedient && Random.Range(0,50)<= 1) // 1 in 50 chances to recalculate the alignment, cohesion, and goal. Increasing fluidity
                                    // doing 1 in 50 to supress the load per update for the computer
        {
            Vector2 ali = align(); // getting the aligned return value
            Vector2 coh = cohesion(); // getting the cohesion return value
            Vector2 gl = Vector2.zero;

            if (manager.GetComponent<AllUnits>().seekgoal)
            {
                gl = seek(goalPos); // when seeking goal, gl is updated to go to the target
            }
            currentForce = gl + ali + coh; // everything added

            //currentForce = currentForce.normalized; // make it a unit vector
        }

        if (manager.GetComponent<AllUnits>().willful && Random.Range(0,50) <= 1)
        {
            if (Random.Range(0, 50) < 1) // change direction
            {
                currentForce = new Vector2(Random.Range(0.01f, 0.1f), Random.Range(0.01f, 0.1f)); // random direction
            }
        }

        applyForce(currentForce);
    }

    // Update is called once per frame
    void Update()
    {
        flock();
        //setSeparation(manager.GetComponent<AllUnits>().separationRadius); // setting the separation radius from unit manager
        if (this.GetComponent<Rigidbody2D>().velocity.magnitude > 2.4)
        {
            setSeparation(this.GetComponent<Rigidbody2D>().velocity.magnitude);
        }
        goalPos = manager.transform.position;
    }
}
