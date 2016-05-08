using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class PirateSpawn : MonoBehaviour {
	public GameObject pirate; 
	float timecount = 0;
	public int pircount = 0;
	public float spawndelay = 3.0f;
	public uint maxpir = 100;
    public List<GameObject> pirateshiplist;

    //GA var
    public GameObject GAobject;
	public bool UsingGA = false;
    public string GAdataPath;
	bool firstgeneration = false;
    public List<List<uint>> piratechromosomes = null;
    public List<uint> fitness = null;
    StreamReader inStreamGAdata = null;
    char[] delim = { ' ' };
    // Use this for initialization
    void Start () 
	{
        pirateshiplist = new List<GameObject>();
        //if we are using GA, try to open the .txt file which hold the chrom data
        if (UsingGA) {
            GAdataPath = "PirateShipGAdata.txt";
            if(!firstgeneration)
            {
                try
			    {
				    inStreamGAdata = new StreamReader (GAdataPath);
			    }
			    catch
			    {
                    //if failed to open the file, means this is the first generation
                    firstgeneration = true;
			    }
            }

            piratechromosomes = new List<List<uint>>();
            fitness = new List<uint>();
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
	
	void Spawn() 
	{
        //GameObject pirclone = (GameObject) Instantiate(pirate, transform.position, transform.rotation);
        
        //generate a new pirateship here
        pirateshiplist.Add((GameObject)Instantiate (pirate, transform.position, transform.rotation));
        
        //if we are using genetic algorithm then we need to calculate the 
        //agressiveness and fire distance of the pirateship
        if (UsingGA)
        {
            //if this is the first generation of pirateship, we randomly generate their data
			if(firstgeneration)
            {
                //a random float agreesiveness between 0 to 5
                pirateshiplist[pircount].GetComponent<PirateShipController>().Aggressiveness = UnityEngine.Random.Range(0f, 5.0f);
                pirateshiplist[pircount].GetComponent<PirateShipController>().fireDistance = UnityEngine.Random.Range(15f, 50f);
            }
            else
            {
                //read data from txt file;
                string line = inStreamGAdata.ReadLine();
                string[] tokens = line.Split(delim);
                //set pirateship's data
                pirateshiplist[pircount].GetComponent<PirateShipController>().Aggressiveness = ((float)(uint.Parse(tokens[0])))/100;
                pirateshiplist[pircount].GetComponent<PirateShipController>().fireDistance = ((float)(uint.Parse(tokens[1]))) / 10;
            }
        }
        //pirclone.transform.Rotate (Vector3.up, Random.Range (0, 359));
    }

    //called by maingame, as a interface to receive the round end event
    public void OnRoundEnd()
    {
        if(UsingGA)
        {
            inStreamGAdata.Close();
            //take chrom and fitness from generation for GA
            for(int i = 0; i < pirateshiplist.Count; i++)
            {
                //calculate fitness and save to list for GA later
                PirateShipController temppir = pirateshiplist[i].GetComponent<PirateShipController>();
                fitness.Add((uint)temppir.TimeAlive + (uint)temppir.Accuracy * 100);
                List<uint> pirchrom = new List<uint>();
                pirchrom.Add((uint)(temppir.Aggressiveness * 100));
                pirchrom.Add((uint)(temppir.fireDistance * 10));
                piratechromosomes.Add(pirchrom);
            }


        }
    }
}
