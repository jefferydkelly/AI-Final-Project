using UnityEngine;
using System.Collections;

public class AsteroidSource : MonoBehaviour {

	public GameObject asteroid1;
	public int max_asteroids;
	public float asteroidRadius = 1000;
	// Use this for initialization
	void Start () 
	{
		for (int i = 0; i < max_asteroids; i++) {
			float ang = i * 360.0f / max_asteroids;
			GameObject astoroid = (GameObject)Instantiate (asteroid1);
			astoroid.transform.position = new Vector3 (Mathf.Sin (ang), 0, Mathf.Cos (ang)) * asteroidRadius;
		}
	}
	
	void Update () 
	{

	}

	void Spawn() 
	{
		GameObject astclone = (GameObject) Instantiate(asteroid1, transform.position, transform.rotation);
		astclone.GetComponent<PlanetRevolutionScript> ().radius = astclone.transform.position.z  + Random.Range (0, 100);
	}

}
