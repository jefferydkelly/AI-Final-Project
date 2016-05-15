using UnityEngine;
using System.Collections;

public class PoliceSpawn : MonoBehaviour {

	public GameObject police;
	private int polcount = 0;
	public int max_police = 3;
	float timecount = 0f;
	public float spawndelay = 3.0f;
	// Use this for initialization
	void Start () {
		Invoke ("Spawn", spawndelay);
	}
	
	// Update is called once per frame
	void Update () 
	{
		/*
		timecount += Time.deltaTime;
		if (timecount > spawndelay) 
		{
			timecount = 0;
				Spawn ();
		}*/
	
	}

	void Spawn()
	{
		if (polcount < max_police) {
			GameObject.Instantiate (police, transform.position, Quaternion.identity);
			polcount++;
		}

		Invoke ("Spawn", spawndelay);

	}

	public int NumberOfPolice {
		get {
			return polcount;
		}

		set {
			polcount = value;
		}
	}
}
