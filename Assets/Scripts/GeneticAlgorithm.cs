using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class GeneticAlgorithm : MonoBehaviour {

    //variables
    int popsize;
    uint totfit;//for parents selection
    public int nBits;
    public float crossoverProb;
    public float mutateProb;
    List<List<uint>> newPopChromList;
    List<List<uint>> oldPopChromList;
    List<uint> oldPopFitness;
    StreamWriter outStream;

    void Start()
    {
        popsize = 0;
        newPopChromList = new List<List<uint>>();
        nBits = 9;
        totfit = 0;
        crossoverProb = 0.9f;
        mutateProb = 0.1f;
        
    }

    public void startGA(List<List<uint>> chromosomes, List<uint> fitness, string datapath)
    {
        //get population size
        popsize = fitness.Count;
        //get total fitness for parents selection
        for(int i = 0; i< popsize; i++)
            totfit += fitness[i];

        oldPopChromList = chromosomes;
        oldPopFitness = fitness;
        while (newPopChromList.Count < popsize)
        {
            //first one needs to be best
            if (newPopChromList.Count == 0)
                findOldBest();
            else
            {
                //breed new one
                int p1, p2; //where parents at in OldList
                //select parents
                p1 = SelectRoul();
                p2 = SelectRoul();
                //do crossover and add new dude
                Crossover(p1, p2);
                //mutate the new dude at the end of newlist
                Mutate();
            }
        }
        //Evolve end, write new pop chroms to datafile
        outStream = new StreamWriter(datapath);
        SaveNewPoptoFile();
        outStream.Close();        
    }

    void SaveNewPoptoFile()
    {
        for( int i = 0; i<newPopChromList.Count; i++)
        {
            for (int j = 0; j < newPopChromList[0].Count; j++)
            {
                outStream.Write(newPopChromList[i][j]);
                outStream.Write(" ");
            }
            outStream.WriteLine();
        }

    }

    void Mutate()
    {
        if (Random.Range(0f, 1.0f) < mutateProb)
        {
            for (int i = 0; i < oldPopChromList[0].Count; i++)
            {
                int mutPt = Random.Range(0, nBits);
                int mutMask = 1 << mutPt;
                newPopChromList[newPopChromList.Count - 1][i] ^= (uint)mutMask;
            }
        }
    }



    void Crossover(int p1, int p2)
    {
        List<uint> newDude = new List<uint>();
        if (Random.Range(0f, 1.0f) < crossoverProb)
        {
            //go through all parents chrom to do crossover
            for (int i = 0; i < oldPopChromList[0].Count; i++)
            {
                int xOverPt = Random.Range(0, nBits);
                //parents chrom
                uint c1, c2;
                c1 = oldPopChromList[p1][i];
                c2 = oldPopChromList[p2][i];
                c1 = (c1 >> xOverPt) << xOverPt;
                c2 = (c2 << (32 - xOverPt)) >> (32 - xOverPt);
                uint newKid = c1 | c2;
                newDude.Add(newKid);
            }
            //add new dude to newpop list
            newPopChromList.Add(newDude);
        }
        else
        {
            //random add one of the parents to newpop;
            int temp = Random.Range(0f, 0.5f) < 0.5 ? p1 : p2;
            newPopChromList.Add(oldPopChromList[temp]);
        }
    }

    int SelectRoul()
    {
        int roll = Random.Range(0, (int)totfit);
        uint accum = oldPopFitness[0];
        int iSel = 0;
        while(accum <= roll && iSel < (popsize-1))
        {
            iSel++;
            accum += oldPopFitness[iSel];
        }

        return iSel;
    }

    void findOldBest()
    {
        int whereBest = 0;
        uint bestFit = oldPopFitness[0];
        for(int i = 1; i < popsize; i++)
        {
            if(oldPopFitness[i] > bestFit)
            {
                whereBest = i;
                bestFit = oldPopFitness[i];
            }
        }
        newPopChromList.Add(oldPopChromList[whereBest]);
    }

}
