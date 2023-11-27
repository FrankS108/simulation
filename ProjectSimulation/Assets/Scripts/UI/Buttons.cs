using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Buttons : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textVelocity;

    private void Start()
    {
        textVelocity.text = Time.timeScale.ToString();
    }

    public void UpdateVelocityLess()
    {
        Time.timeScale = Time.timeScale > 0 ? --Time.timeScale : 0;
        textVelocity.text = Time.timeScale.ToString();
    }

    public void UpdateVelocityMore()
    {
        Time.timeScale = Time.timeScale < 100 ? ++Time.timeScale : 100;
        textVelocity.text = Time.timeScale.ToString();
    }
}
