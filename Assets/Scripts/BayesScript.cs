using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

//this bayesian classifier is based off the one included in al's handouts, and decides whether or not a police ship should fire based off 
// of distance to target(float), presence of obstacles nearby(bool) and number of shots remaining(int).

public class BayesScript : MonoBehaviour {

	public string fileName = "bulletlog.txt";
	double sqrt2PI = Mathf.Sqrt (2.0 * Mathf.PI);

	// Outcome (Boolean)
	int[] outcomeCt = new int[2];				// Outcome (play T or F)
	double[] outcomePrp = new double[2];

	//targetdist (continuous float)
	float[] targetdistSum = new float[2];
	float[] targetdistSumSq = new float[2];
	double[] targetdistMean = new double[2];
	double[] targetdistStdDev = new double[2];

	// obstacles condition (Boolean)
	int[,] obstaclesCt = new int[2, 2];			// Discrete with 2 values (T or F)
	double[,] obstaclesPrp = new double[2, 2];	

	// Remaining shots condition (continuous)
	int[] remainingshotsSum = new int[2];
	int[] remainingshotsSumSq = new int[2];
	double[] remainingshotsMean = new double[2];
	double[] remainingshotsStdDev = new double[2];

	public struct Observation
	{
		public bool outcome;		// true/false for success/failure
		public float targetdist;	// distance to target in ingame units
		public bool obstacles;		// true/false depending on if obstacles were present nearby
		public int remainingshots;	// Remaining shots left
	}

	List<Observation> obsTab = new List<Observation> ();

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void InitStats ()
	{
		for (int i = 0; i < 2; i++) {
			outcomeCt [i] = 0;
		}

		for (int i = 0; i < targetdistSum.Length; i++) {
			targetdistSum [i] = 0;
			targetdistSumSq [i] = 0;
		}

		for (int i = 0; i < remainingshotsSum.Length; i++) {
			remainingshotsSum [i] = 0;
			remainingshotsSumSq [i] = 0;
		}

		for (int i = 0; i < 2; i++) {
			for (int j = 0; j < 2; j++) {
				obstaclesCt [i, j] = 0;
			}
		}
	}

	void FillObsTable()
	{
		using (StreamReader rdr = new StreamReader (fileName)) 
		{
			string lineBuf = null;
			while ((lineBuf = rdr.ReadLine ()) != null) 
			{
				string[] lineAra = lineBuf.Split (' ');
				AddObs ((lineAra [0] == "True" ? true : false), float.Parse (lineAra [1]), (lineAra [2] == "True" ? true : false),
					int.Parse (lineAra [3]));
			}
		}
	}

	void AddObs(bool outcome, float targetdist, bool obstacles, int remainingshots)
	{
		Observation obs;
		obs.outcome = outcome;
		obs.targetdist = targetdist;
		obs.obstacles = obstacles;
		obs.remainingshots = remainingshots;
		obsTab.Add (obs);
	}

	void BuildStats()
	{
		InitStats ();
		// Accumulate all the counts and sums
		foreach (Observation obs in obsTab) {
			// Do this once
			int outcomeOff = obs.outcome ? 0 : 1;

			targetdistSum [outcomeOff] += obs.targetdist;
			targetdistSumSq [outcomeOff] += obs.targetdist * obs.targetdist;

			remainingshotsSum [outcomeOff] += obs.remainingshots;
			remainingshotsSumSq [outcomeOff] += obs.remainingshots * obs.remainingshots;

			obstaclesCt [obs.obstacles ? 0 : 1, outcomeOff]++;

			outcomeCt [outcomeOff]++;
		}

		// Calculate all the statistics
		CalcProps (obstaclesCt, outcomeCt, obstaclesPrp);

		targetdistMean [0] = Mean (targetdistSum [0], outcomeCt [0]);
		targetdistMean [1] = Mean (targetdistSum [1], outcomeCt [1]);
		targetdistStdDev [0] = StdDev (targetdistSumSq [0], targetdistSum [0], outcomeCt [0]);
		targetdistStdDev [1] = StdDev (targetdistSumSq [1], targetdistSum [1], outcomeCt [1]);

		remainingshotsMean [0] = Mean (remainingshotsSum [0], outcomeCt [0]);
		remainingshotsMean [1] = Mean (remainingshotsSum [1], outcomeCt [1]);
		remainingshotsStdDev [0] = StdDev (remainingshotsSumSq [0], remainingshotsSum [0], outcomeCt [0]);
		remainingshotsStdDev [1] = StdDev (remainingshotsSumSq [1], remainingshotsSum [1], outcomeCt [1]);

		outcomePrp [0] = (double)outcomeCt [0] / obsTab.Count;
		outcomePrp [1] = (double)outcomeCt [1] / obsTab.Count;
	}

	double CalcBayes (bool outcome, float targetdist, bool obstacles, int remainingshots)
	{
		int outcomeOff = outcome ? 0 : 1;
		double like = GauProb (targetdistMean [outcomeOff], targetdistStdDev [outcomeOff], targetdist) *
			GauProb (remainingshotsMean [outcomeOff], remainingshotsStdDev [outcomeOff], remainingshots) *
			obstaclesPrp [obstacles ? 0 : 1, outcomeOff] *
			outcomePrp [outcomeOff];
		return like;
	}

	public bool Decide (float targetdist, bool obstacles, int remainingshots)
	{
		double playYes = CalcBayes (true, targetdist, obstacles, remainingshots);
		double playNo = CalcBayes (false, targetdist, obstacles, remainingshots);
		return playYes > playNo;
	}
		
	// Standard statistical functions.  These should be useful without modification. 
	// Calculates the proportions for a discrete table of counts
	// Handles the 0-frequency problem by assigning an artificially
	// low value that is still greater than 0.
	void CalcProps (int[,] counts, int[] n, double[,] props)
	{
		for (int i = 0; i < counts.GetLength (0); i++)
			for (int j = 0; j < counts.GetLength (1); j++)
				// Detects and corrects a 0 count by assigning a proportion
				// that is 1/10 the size of a proportion for a count of 1
				if (counts [i, j] == 0)
					props [i, j] = 0.1d / outcomeCt [j];	// Can't have 0
				else
					props [i, j] = (double)counts [i, j] / n [j];
	}

	double Mean (int sum, int n)
	{
		return (double)sum / n;
	}

	double StdDev (int sumSq, int sum, int n)
	{
		return Mathf.Sqrt ((sumSq - (sum * sum) / (double)n) / (n - 1));
	}

	// Calculates probability of x in a normal distribution of
	// mean and stdDev.  This corrects a mistake in the pseudo-code,
	// which used a power function instead of an exponential.
	double GauProb (double mean, double stdDev, int x)
	{
		double xMinusMean = x - mean;
		return (1.0d / (stdDev * sqrt2PI)) *
			Mathf.Exp (-1.0d * xMinusMean * xMinusMean / (2.0d * stdDev * stdDev));
	}
}
