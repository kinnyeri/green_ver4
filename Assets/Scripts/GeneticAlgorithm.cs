using System;
using System.Collections.Generic;
using UnityEngine;

//제너래이션 생성
public class GeneticAlgorithm
{
    public List<DNA> Population { get; private set; }
    public int Generation { get; private set; }
    public double BestFitness;
    public float[,] BestGenes; 

    public int Elitism;
    public float MutationRate;
    public bool generationEnded = false;
    private List<DNA> newPopulation;
    private System.Random random;
    private double fitnessSum;
    private Func<float> getRandomGene;
    private Func<float, double> fitnessFunction;
    private Terrain[] terrainGroup;
    private int populationSize;
    private SimulationManager simulationManager;
    private Func<string, int> debugW; // 디버깅 용 필요 없음

    public GeneticAlgorithm(Terrain[] terrainGroup, SimulationManager simulationManager, int populationSize, System.Random random, int elitism,
        Func<float, double> fitnessFunction, Func<float> getRandomGene, Func<string, int> debugW, float mutationRate = 0.01f)
    {
        Generation = 1;
        Elitism = elitism;
        MutationRate = mutationRate;
        Population = new List<DNA>(populationSize);
        newPopulation = new List<DNA>(populationSize);
        this.random = random;
        this.populationSize = populationSize;
        this.debugW = debugW;
        this.simulationManager = simulationManager;
        this.fitnessFunction = fitnessFunction;
        debugW("GA 객체");

        for (int i = 0; i < populationSize;i++)
        {
            debugW("DNA "+i);
            Population.Add(new DNA(terrainGroup[i], 450, random, fitnessFunction, getRandomGene, debugW, shouldInitGenes: true));
            debugW("Done DNA " + i);
        }
        simulationManager.simuationsStart();
        generationEnded = !generationEnded;
        debugW("s generationEnded " + generationEnded);
    }
    public void NewGeneration(int numNewDNA = 0, bool crossoverNewDNA = false)
    {
        debugW("new generation start");
        int finalCount = Population.Count + numNewDNA;

        if (finalCount <= 0)
        {
            return;
        }

        if (Population.Count > 0)
        {
            CalculateFitness();
            Population.Sort(CompareDNA);
        }
        newPopulation.Clear();

        for (int i = 0; i < Population.Count; i++)
        {
            if (i < Elitism && i < Population.Count)
            {
                newPopulation.Add(Population[i]);
            }
            else if (i < Population.Count || crossoverNewDNA)
            {
                DNA parent1 = ChooseParent();
                DNA parent2 = ChooseParent();

                DNA child = parent1.Crossover(parent2);

                child.Mutate(MutationRate);
                newPopulation.Add(child);
                child.makeTerrain(); //새로운 gene에 대해서 terrain 생성
            }
            else
            {
                newPopulation.Add(new DNA(terrainGroup[i], 450, random, fitnessFunction, getRandomGene, debugW, shouldInitGenes: true));
                //새로운 gene에 대해서 dna 생성하면서 만들어질 것.
            }
        }

        List<DNA> tmpList = Population;
        Population = newPopulation;
        newPopulation = tmpList;

        Generation++;
        this.generationEnded = false;
        debugW("new generation end "+generationEnded);
        simulationManager.simuationsStart();
        generationEnded = !generationEnded;
        debugW("generationEnded " + generationEnded);
    }

    private int CompareDNA(DNA a, DNA b)
    {
        if (a.Fitness > b.Fitness)
        {
            return -1;
        }
        else if (a.Fitness < b.Fitness)
        {
            return 1;
        }
        else
        {
            return 0;
        }
    }

    private void CalculateFitness()
    {
        fitnessSum = 0;
        DNA best = Population[0];

        for (int i = 0; i < Population.Count; i++)
        {
            fitnessSum += Population[i].CalculateFitness(i);

            if (Population[i].Fitness > best.Fitness)
            {
                best = Population[i];
            }
        }

        BestFitness = best.Fitness;
        //best.Genes = BestGenes.Clone();
        best.Genes= BestGenes;
    }

    private DNA ChooseParent()
    {
        double randomNumber = random.NextDouble() * fitnessSum;

        for (int i = 0; i < Population.Count; i++)
        {
            if (randomNumber < Population[i].Fitness)
            {
                return Population[i];
            }

            randomNumber -= Population[i].Fitness;
        }

        return null;
    }
}
