using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class MerchantSpawn : MonoBehaviour {


	public GameObject merchant;
	private List<GameObject> planetlist;
	private GameObject origin;
	float timecount = 0;
	public float merchcount = 0;
	public float spawndelay = 10.0f;
	public float max_merchants = 100;
	// Use this for initialization
	void Start () 
	{

	}
	
	// Update is called once per frame
	void Update () 
	{
		timecount += Time.deltaTime;
		if ((timecount > spawndelay)&&(merchcount<max_merchants)) 
		{
			Spawn();
			timecount = 0;
		}
		merchcount = GameObject.FindGameObjectsWithTag ("Merchant").Length;
	}

	void Spawn() 
	{
		planetlist = GameObject.FindGameObjectsWithTag ("Planet").ToList();
		origin = planetlist [Random.Range (0, planetlist.Count - 1)];
		planetlist.Remove (origin);

		GameObject.Instantiate(merchant, origin.transform.position, Quaternion.identity);
	}
}
