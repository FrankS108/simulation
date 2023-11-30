using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

    [SerializeField] private List<Slider> slider = new List<Slider>();
    [SerializeField] private List<TextMeshProUGUI> text = new List<TextMeshProUGUI>();
    
   
    //Distancia
    //Cantidad de personas 

    // Start is called before the first frame update
    void Start()
    {
        /*Time.timeScale = speedTime;
        suceptible = N - 1;
        daysInSeconds = daysOfSimulation * SecondsInOneDay;
        StartCoroutine(Simulation());
        //SimulationCicle();*/
        virus = new Virus();

        InitializeUI();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        UpdateInfectionRate();
        UpdateRecoveryRate();
        UpdateMortality();
        UpdateNoMaskRate();
    }

    private void InitializeUI()
    {
        slider[0].value = virus.InfectionRate;
        slider[1].value = virus.RecoveryRate;
        slider[2].value = virus.Mortality;
        slider[3].value = virus.NoFaceMaskRate;

        text[0].text = slider[0].value.ToString("#.00");
        text[1].text = slider[1].value.ToString("#.00");
        text[2].text = slider[2].value.ToString("#.00");
        text[3].text = slider[3].value.ToString("#.00");
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

    private void UpdateInfectionRate()
    {
        virus.InfectionRate = slider[0].value;
        text[0].text = virus.InfectionRate.ToString("#.00");
    }

    private void UpdateRecoveryRate()
    {
        virus.RecoveryRate = slider[1].value;
        text[1].text = virus.RecoveryRate.ToString("#.00");
    }

    private void UpdateMortality()
    {
        virus.Mortality = slider[2].value;
        text[2].text = virus.Mortality.ToString("#.00");
    }

    private void UpdateNoMaskRate()
    {
        virus.NoFaceMaskRate = slider[3].value;
        text[3].text = virus.NoFaceMaskRate.ToString("#.00");
    }

    
}
