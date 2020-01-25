using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Important
using UnityEngine.AI;

public class SCP173 : MonoBehaviour
{
    //Using the camera component this time rather than full gameobject
    public Camera cam;

    private BoxCollider objCollider;
    private Plane[] planes;
    private bool moveable = false;
    private NavMeshAgent agent;

    void Start()
    {
        
        objCollider = GetComponent<BoxCollider>();
        agent = GetComponent<NavMeshAgent>();
    }

    bool CalculateFrustumMoveability()
    {
        //Find camera frustum
        planes = GeometryUtility.CalculateFrustumPlanes(cam);
        //If this gameobject is NOT inside the camera frustum
        if (GeometryUtility.TestPlanesAABB(planes, objCollider.bounds) == false)
            //Move to player
            return true;
        else
            //We must still check if the player is occluded by walls
            return CalculateIfOccluded();
    }

    bool CalculateIfOccluded()
    {
        //Stores raycast data
        RaycastHit hit;
        //Direction to shoot ray is from transform point of this gameobject to the camera's transform
        Vector3 rayDirection = cam.gameObject.transform.position - transform.position;
        //IMPORTANT: make sure player body does not interfere with raycasts!
        if (Physics.Raycast(transform.position, rayDirection, out hit))
        {
            if (hit.transform.root.name != "PlayerBody")
                //Allow movement is player is occluded
                return true;
            else
                //In this case, the player sees this and is not occluded
                return false;
        }
        return false;
    }

    void Update()
    {
        //Determine if we can move
        moveable = CalculateFrustumMoveability();

        if (moveable)
            agent.SetDestination(cam.transform.position);
        else
            agent.SetDestination(transform.position);
    }
}
