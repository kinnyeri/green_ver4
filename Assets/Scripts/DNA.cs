using System;
using System.Collections.Generic;
using UnityEngine;

public class DNA
{
    public float[,] Genes; //{ get; private set; }
    public double Fitness; //{ get; private set; }
    public bool isNewGene = false;

    private System.Random random;
    private Func<float> getRandomGene;
    private Func<float, double> fitnessFunction;
    private Terrain terrain;
    private Func<string, int> debugW;
    static float sizeWeight = 1.5f;
    int size;

    public DNA(Terrain terrain,int size, System.Random random, Func<float, double> fitnessFunction, Func<float> getRandomGene, Func<string, int> debugW, bool shouldInitGenes = true)
    {
        Genes = new float[size, size];
        this.random = random;
        this.terrain = terrain;
        this.size = size;
        this.debugW = debugW;
        this.fitnessFunction = fitnessFunction;
        this.getRandomGene = getRandomGene;
        if (shouldInitGenes)
        {
            debugW("start making Genes[]");
            makeGenes();
            debugW("end making Genes[]");
            debugW("Start makig terrain");
            makeTerrain();
            debugW("Done makig terrain");
        }
    }
    public void makeGenes()
    {
        int x = 0, y = 0;
        float rangeX = 30f * sizeWeight, rangeY = 30f * sizeWeight;
        float maxNumHeight = 100 * sizeWeight;
        float center_difference;

        //// 0.5로 초기화 전체
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                Genes[i, j] = 0.5f;
            }
        }
        //maxNumHeight = 1;
        double newHeight;
        for (int v = 0; v < maxNumHeight; v++)
        {
            int limit = (int)(50 * sizeWeight);
            x = random.Next(limit, size - limit);  //x
            y = random.Next(limit, size - limit); //y

            rangeX = random.Next(30, (int)(100 * sizeWeight));
            rangeY = random.Next(30, (int)(100 * sizeWeight));

            center_difference = (float)random.NextDouble() - 0.5f; //랜덤``

            for (float i = -rangeX / 2; i <= rangeX / 2; i++)
            {
                for (float j = -rangeY / 2; j <= rangeY / 2; j++)
                {
                    int tmpX = Mathf.FloorToInt(x + i);
                    int tmpY = Mathf.FloorToInt(y + j);
                    if ((tmpX >= size || tmpX < 0) || (tmpY >= size || tmpY < 0)) break;

                    newHeight = center_difference * Mathf.Clamp01(1 - Mathf.Sqrt(Mathf.Pow((x - tmpX) / rangeX * 2, 2) + Mathf.Pow((y - tmpY) / rangeY * 2, 2)));

                    Genes[tmpX, tmpY] += ((float)newHeight);
                    Genes[tmpX, tmpY] = Mathf.Clamp01(Genes[tmpX, tmpY]);
                }
            }
        }
    }
    public void makeTerrain()
    {
        debugW("makeTerrain :"+terrain.name);
        terrain.terrainData = GenerateTerrain(terrain.terrainData, Genes, size);
    }
    TerrainData GenerateTerrain(TerrainData terrainData, float[,] Genes, int size)
    {
        terrainData.heightmapResolution = size + 1;

        terrainData.size = new Vector3(size, 5, size); //width, depth,height
        terrainData.SetHeights(0, 0, Genes);
        return terrainData;
    }
    public double CalculateFitness(int index)
    {
        Simulation simulation = terrain.GetComponent<Simulation>();
        Fitness = fitnessFunction(simulation.valueOfZ);
        return Fitness;
    }

    public DNA Crossover(DNA otherParent)
    {
        DNA child = new DNA(new Terrain(), 450, random, fitnessFunction, getRandomGene, debugW, shouldInitGenes: false);

        for (int i = 0; i < Genes.Length; i++)
        {
            for (int j = 0; j < Genes.Length; j++)
            {
                child.Genes[i, j] = random.NextDouble() < 0.5 ? Genes[i, j] : otherParent.Genes[i, j];
            }
        }

        return child;
    }

    public void Mutate(float mutationRate)
    {
        for (int i = 0; i < Genes.Length; i++)
        {
            for (int j = 0; j < Genes.Length; j++)
            {
                if (random.NextDouble() < mutationRate)
                {
                    Genes[i, j] += getRandomGene(); // 원래 꺼에서 빼고 더하고..
                }
            }
        }
    }
}