using Microsoft.Unity.VisualStudio.Editor;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class ManagerData : MonoBehaviour
{

    [SerializeField] private GameObject citizens;
    [SerializeField] private int infectados = 0;
    [SerializeField] private int sanos = 0;
    [SerializeField] private int recuperados = 0;
    [SerializeField] private int muertos = 0;
    [SerializeField] private GameObject ui;
    [SerializeField] private bool state = false;
    [SerializeField] private List<TextMeshProUGUI> amount;
   
    public enum State
    {
        SANO,
        INFECTADO,
        RECUPERADO,
        MUERTO
    }

    private void Start()
    {

        ui.SetActive(state);
    }

    public void MostrarGraficas()
    {
        state = !state;
        ui.SetActive(state);
    }

    public void GenerarGraficas()
    {
        sanos = 0;
        infectados = 0;
        recuperados = 0;
        muertos = 0;

        foreach (CitizenController cc in citizens.GetComponentsInChildren<CitizenController>())
        {
            if (cc.estado == CitizenController.State.INFECTADO) infectados++;
            else if (cc.estado == CitizenController.State.SANO) sanos++;
            else if (cc.estado == CitizenController.State.RECUPERADO) recuperados++;
            else if (cc.estado == CitizenController.State.MUERTO) muertos++;
        }

        amount[0].text = sanos.ToString();
        amount[1].text = infectados.ToString();
        amount[2].text = recuperados.ToString();
        amount[3].text = muertos.ToString();
    }

    private void FixedUpdate()
    {
        GenerarGraficas();
    }

}
