using UnityEngine;
using System.Collections;

public class AsteroidSource : MonoBehaviour {

	public GameObject asteroid1;
	private int asteroidcount;
	public int max_asteroids;
	public float spawndelay;
	private float timecount;

	// Use this for initialization
	void Start () 
	{
		asteroidcount = 0;
		timecount = 0;
	}
	
	void Update () 
	{
		timecount += Time.deltaTime;
		if ((timecount > spawndelay)&&(asteroidcount<max_asteroids)) 
		{
			Spawn();
			timecount = 0;
			asteroidcount++;
			spawndelay = Random.Range (0.5f, 1.5f);
		}
	}

	void Spawn() 
	{
		GameObject astclone = (GameObject) Instantiate(asteroid1, transform.position, transform.rotation);
		astclone.GetComponent<PlanetRevolutionScript> ().radius = astclone.transform.position.z  + Random.Range (0, 100);
		//pirclone.transform.Rotate (Vector3.up, Random.Range (0, 359));
	}

}
