using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ManagerStart : MonoBehaviour
{
    [SerializeField] private GameObject errorObject;
    [SerializeField] private TextMeshProUGUI textError;
    [SerializeField] private TMP_InputField inputAmount;
    [SerializeField] private Button start;
    [SerializeField] private GameObject ui;
    [SerializeField] private GameObject auxUI;
    [SerializeField] private GameObject timeGO;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private GameObject parentCitizens;
    [SerializeField] private GameObject prefab;
    [SerializeField] private int startTime = 0;
    //Cuando cambia
    private void Start()
    {
        errorObject.SetActive(false);
        timeGO.SetActive(false);
        start.enabled = false;
        ui.SetActive(false);
        Time.timeScale = 0;
    }


    public void ValueChange()
    {
        if (inputAmount.text == null || inputAmount.text == "") return;
        if(int.Parse(inputAmount.text.ToString()) < 3)
        {
            textError.text = "La cantidad minima es 2.";
            errorObject.SetActive(true);
        }
        else
        {
            errorObject.SetActive(false);
            start.enabled = true;
        }
    }

    public void StartSimulation()
    {
        int numberPersons = int.Parse(inputAmount.text.ToString());
        for (int i = 0; i < numberPersons - 1; i++)
        {
            GameObject go = Instantiate(prefab);
            go.transform.SetParent(parentCitizens.transform, false);
        }
        startTime = (int) Time.realtimeSinceStartup;
        Time.timeScale = 1;
        auxUI.SetActive(false);
        timeGO.SetActive(true);
        ui.SetActive(true);
       
    }

    public IEnumerator TimeUpdate()
    {
        
        yield return new WaitForSeconds(Time.realtimeSinceStartup);
    }

    private void FixedUpdate()
    {
        timeText.text = (Time.realtimeSinceStartup - startTime).ToString() + " seg";
    }


}
