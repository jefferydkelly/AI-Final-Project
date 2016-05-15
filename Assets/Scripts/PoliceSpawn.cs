using UnityEngine;
using System.Collections;

public class PoliceSpawn : MonoBehaviour {

	public GameObject police;
	public float polcount = 0f;
	public float max_police = 3f;
	float timecount = 0f;
	public float spawndelay = 3.0f;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		timecount += Time.deltaTime;
		if (timecount > spawndelay) 
		{
			timecount = 0;
			if(polcount<max_police)
				Spawn ();
		}
	
	}

	void Spawn()
	{
		GameObject polclone = (GameObject) Instantiate(police, this.transform.position, Quaternion.identity);
		polcount++;
	}
}
