using UnityEngine;
using System.Collections;
using System;
using System.IO;

public class PirateSpawn : MonoBehaviour {
	public GameObject pirate; 
	float timecount = 0;
	public uint pircount = 0;
	public float spawndelay = 3.0f;
	public GameObject[] pirateshiplist;
	public uint maxpir = 100;
	//GA var
	public GameObject GAobject;
	public bool UsingGA = false;

	bool firstgeneration = false;

	// Use this for initialization
	void Start () 
	{
		pirateshiplist = new GameObject[maxpir];
		if (UsingGA) {
			StreamReader inStreamGAdata = null;
			try
			{
				inStreamGAdata = new StreamReader ("PirateShipGAdata.txt");
			}
			catch
			{
				firstgeneration = true;
			}
		}
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
	
	public void Spawn() 
	{
		//GameObject pirclone = (GameObject) Instantiate(pirate, transform.position, transform.rotation);
		pirateshiplist [pircount] = (GameObject)Instantiate (pirate, transform.position, transform.rotation);
		if (UsingGA) {
			if(firstgeneration){
				//pirateshiplist[pircount].GetComponent<PirateShipController> ().
			}
		}
		//pirclone.transform.Rotate (Vector3.up, Random.Range (0, 359));
	}
}
