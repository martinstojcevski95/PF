using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class PathMover : MonoBehaviour {

	public NavMeshAgent navmeshagent;
    [HideInInspector]
    public Queue<Vector3> pathPoints = new Queue<Vector3>();

	private void Awake() 
	{
		navmeshagent = GetComponent<NavMeshAgent>();

   //     FindObjectOfType<PathCreator>().OnNewPathCreated += SetPoints;



    }


    // Set these points from the points of each player player stats.Points so it cam move trough them
    public void SetPoints(IEnumerable<Vector3> points)
	{
		pathPoints = new Queue<Vector3>(points);
	}


	// Update is called once per frame
	private void Update () {
		UpdatePathing();

    }

	private void UpdatePathing()
	{
        var pathmover = gameObject.GetComponent<PathMover>();
        if(pathmover != null)
            {
            if (ShouldSetDestination())
            {

                navmeshagent.SetDestination(pathPoints.Dequeue());
            }
        }
    
    }


	private bool ShouldSetDestination()
	{
		if (pathPoints.Count == 0)
			return false;

		if(navmeshagent.hasPath == false || navmeshagent.remainingDistance < 0.5f)
            return true;
       
			
		
		return false;
	}
}
