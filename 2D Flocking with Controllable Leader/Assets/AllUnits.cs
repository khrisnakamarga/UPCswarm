using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllUnits : MonoBehaviour
{

    public GameObject[] units; // hold all individual boids created
    public GameObject unitPrefab; // prefab for each unit to instantiate them
    public int numUnits = 10; // set the number of particles to be created
    public Vector3 range = new Vector3(5, 5, 5); // size of the space around the unit manager where the particles will instantiate

    public bool seekgoal = true; // whether the boid follows the target position
    public bool obedient = true; // choose to obey the flocking rule
    public bool willful = false; // free will of where to go

    [Range(0, 200)] // allows us to set the range of the next variable that comes up and gives a slider in Unity
    public int neighbourDistance = 50; // the radius for boids considered as neighbors

    [Range(0, 50)]
    public float maxforce = 0.5f; // maximum force that can be applied

    [Range(0, 50)]
    public float maxvelocity = 2.0f; // maximum velocity for the boids

    [Range(0, 5)]
    public float separationRadius = 2.5f; // separation of each boid


    //[range(0, 2)]
    //public float maxforce = 0.5f;

    //[range(0, 5)]
    //public float maxvelocity = 2.0f;

    void OnDrawGizmosSelected() // see the range of the unit manager
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(this.transform.position, range * 2); // draw wirecube around the center
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(this.transform.position, 0.2f); // draw sphere in the middle

    }

    // Use this for initialization
    void Start()
    {
        units = new GameObject[numUnits];
        for (int i = 0; i < numUnits; i++)
        {
            Vector3 unitPos = new Vector3(Random.Range(-range.x, range.x),
                                            Random.Range(-range.y, range.y),
                                            Random.Range(0, 0));
            units[i] = Instantiate(unitPrefab, this.transform.position + unitPos, Quaternion.identity) as GameObject; // instantiate each boid in the random location generated
            units[i].GetComponent<Unit>().manager = this.gameObject; // unit knows who's the manager
            //System.Console.WriteLine("Made one object");
        }

    }
}