using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    [SerializeField] private int N = 1000000;

    [SerializeField] private const int SecondsInOneDay = 86400; //60 min * 60 seg * 24hr
    [SerializeField] private Virus virus;
    [SerializeField] private int daysOfSimulation = 5;
    [SerializeField] private int daysInSeconds;
    [SerializeField] private int actualTime = 0;

    [SerializeField] private double suceptible = 0;
    [SerializeField] private double exposed = 1; //Expuesto
    [SerializeField] private double infected = 0;
    [SerializeField] private double recovered = 0;
    [SerializeField] private double dead = 0;
    [SerializeField] private int speedTime = 100;

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = speedTime;
        suceptible = N - 1;
        daysInSeconds = daysOfSimulation * SecondsInOneDay;
        virus = new Virus();
        StartCoroutine(Simulation());
        //SimulationCicle();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
    }

    private void ProvisionalSimulation()
    {
        double dS = -virus.InfectionRate * suceptible * infected / N;
        double dE = (virus.InfectionRate * suceptible * (infected / N) - virus.ExposedRate * exposed);
        double dI = (virus.ExposedRate * exposed - (virus.RecoveryRate + virus.Mortality) * infected);
        double dR = (virus.RecoveryRate * infected);
        double dM = virus.Mortality * infected * 1;

        suceptible += dS;
        exposed += dE;
        infected += dI;
        recovered += dR;
        dead += dM;

        Debug.Log($" S = { suceptible } E = {exposed} I = { infected } R = { recovered } M = { dead }");
    }

    public void SimulationCicle()
    {
        for(int i = 0; i < daysInSeconds; i++)
        {
            ProvisionalSimulation();
        }
    }

    public IEnumerator Simulation()
    {
        if(actualTime < daysInSeconds)
        {
            actualTime += 1;
            ProvisionalSimulation();
            yield return new WaitForSeconds(1);
            StartCoroutine(Simulation());
        }
    }
}
