using UnityEngine;
using System.Collections;

public class PlanetRevolutionScript : MonoBehaviour {

	public Transform center;
	public Vector3 axis = Vector3.up;
	public Vector3 desiredpos;
	public float radius;
	public float radiusSpeed;
	public float rotationSpeed;
	public float spinspeed;


	// Use this for initialization
	void Start () 
	{
		center = GameObject.Find ("Sol").transform;
		this.transform.position = (this.transform.position - center.position).normalized * radius + center.position;
	}
	
	// Update is called once per frame
	void Update () 
	{
		transform.RotateAround (center.position, axis, rotationSpeed * Time.deltaTime);
		desiredpos = (transform.position - center.position).normalized * radius + center.position;
		transform.position = Vector3.MoveTowards(transform.position, desiredpos, Time.deltaTime * radiusSpeed);

		//planetspin
		this.transform.Rotate(0,spinspeed*Time.deltaTime,0);
	}
}
