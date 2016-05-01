using UnityEngine;
using System.Collections;

public class SolScript : MonoBehaviour {

	public float spinspeed;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		this.transform.Rotate (0, spinspeed * Time.deltaTime, 0);
	}
}
