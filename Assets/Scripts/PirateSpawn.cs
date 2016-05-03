using UnityEngine;
using System.Collections;

public class PirateSpawn : MonoBehaviour {
	public GameObject pirate; 
	float timecount = 0;
	public uint pircount = 0;
	public float spawndelay = 3.0f;
	public GameObject[] pirateshiplist;
	public uint maxpir = 100;
	// Use this for initialization
	void Start () 
	{
		pirateshiplist = new GameObject[maxpir];
	}

	// Update is called once per frame
	void Update () 
	{
		timecount += Time.deltaTime;
		if ((timecount > spawndelay)&&(pircount<maxpir)) 
		{
			Spawn();
			timecount = 0;
			pircount++;
		}
	}
	
	void Spawn() 
	{
		//GameObject pirclone = (GameObject) Instantiate(pirate, transform.position, transform.rotation);
		pirateshiplist [pircount] = (GameObject)Instantiate (pirate, transform.position, transform.rotation);
		//pirclone.transform.Rotate (Vector3.up, Random.Range (0, 359));
	}
}
