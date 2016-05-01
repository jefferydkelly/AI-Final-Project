using UnityEngine;
using System.Collections;

public class SpawnPolice : MonoBehaviour 
{
	public GameObject police; 
	float timecount = 0;
	public float polcount = 0;
	public float spawndelay = 3.0f;
	// Use this for initialization
	void Start () 
	{
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		timecount += Time.deltaTime;
		if ((timecount > spawndelay)&&(polcount<5)) 
		{
			Spawn();
			timecount = 0;
			polcount++;
		}
	}
	
	void Spawn() 
	{
		GameObject polclone = (GameObject) Instantiate(police, transform.position, transform.rotation);
	}

}
