using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneticManager : MonoBehaviour
{
    //전반적인 유전 알고리즘 관리
    // Start is called before the first frame update
    public GeneticAlgorithm ga;
    public Terrain[] terrainGroup;
    public SimulationManager simulationManager;
    float target = 0.95f; //목표 확률
    int populationSize; //한 세대의 인구 수
    float mutationRate = 0.01f; 
    int elitism = 5;
    System.Random random;
    int count = 0;
    void Start()
    {
        random = new System.Random();
        //ga 를 만들어야함
        populationSize = terrainGroup.Length;
        Debug.Log("Make GA "+populationSize);
        ga = new GeneticAlgorithm(terrainGroup, simulationManager, populationSize,random, elitism, fitnessFunction, getRandomGene, debugWGA, mutationRate);
        // getRandomGene이랑 fitnessFunc 정의해야함
    }

    // Update is called once per frame
    void Update()
    {
        //ga의 상태를 파악 > 제너래이션 한 세대가 끝났는지 안끝났는지 확인 후 끝났으면 
        debugWGA("BestFitness "+ga.BestFitness);
        if(ga.BestFitness < target) // || ga.BestFitness.Equals(double.NaN)
        {
            debugWGA("update bestFiteness " + ga.BestFitness);
            if (ga.generationEnded && count<5)
            {
                ga.generationEnded = false;
                ga.NewGeneration();
                count++;
            }
        }
        else
        {
            debugWGA("종료!");
            this.enabled = false;
        }
        debugWGA("번호 "+count);
        
    }

    public float getRandomGene()
    {
        return (float)Random.Range(-2.5f,2.5f);
    }
    public double fitnessFunction(float valueOfZ)
    {
        int n = 10;

        float sum = 0;
        float x2 = valueOfZ * valueOfZ;
        float nom = valueOfZ;
        float denom = 1;
        float c = 1;

        for (int i = 0; i < n; i++)
        {
            sum += nom / denom;
            c += 2;
            nom *= x2;
            denom *= c;
        }
        Debug.Log("valueOfZ : " + valueOfZ + " x2 : " + x2 + " sum : " + sum);
        double ans = (0.5 + sum * Mathf.Exp(-x2 * 0.5f) / Mathf.Sqrt(2 * Mathf.PI));
        debugWGA("확률 " + ans);
        
        return ans;
    }
    public int debugWGA(string s)
    {
        Debug.Log(s);
        return 1;
    }
}
