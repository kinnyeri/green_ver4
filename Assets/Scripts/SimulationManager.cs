using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulationManager : MonoBehaviour
{
    public Simulation[] simulations;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void simuationsStart()
    {
        for(int i = 0; i < simulations.Length; i++)
        {
            simulations[i].simulationStart();
        }
    }
}
