using UnityEngine;
using System.Collections;

public class NavMeshScript : MonoBehaviour {
	private NavMeshAgent agent;
	public Transform navTarget;
	// Use this for initialization
	void Start () 
	{
		agent = GetComponent<NavMeshAgent>();   

	}
	
	// Update is called once per frame
	void Update () 
	{
		agent.SetDestination(navTarget.position);
	}
}
