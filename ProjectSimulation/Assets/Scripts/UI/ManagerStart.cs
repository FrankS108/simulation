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
    //Cuando cambia
    private void Start()
    {
        errorObject.gameObject.SetActive(false);
        start.enabled = false;
    }


    public void ValueChange()
    {
        if (inputAmount.text == null || inputAmount.text == "") return;
        if(int.Parse(inputAmount.text.ToString()) <= 2)
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
}
