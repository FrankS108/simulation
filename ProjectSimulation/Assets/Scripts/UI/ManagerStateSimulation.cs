using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ManagerStateSimulation : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timeSpeed;
    [SerializeField] private int auxSpeed;
    [SerializeField] private bool isPaused = false;

    [SerializeField] private Button play;
    [SerializeField] private Button pause;

    [SerializeField] private List<GameObject> listUi;

    private void Start()
    {
        play.enabled = false;
    }

    public void Play()
    {
        pause.enabled = true;
        PlayAllObjects();
        Time.timeScale = auxSpeed;
    }

    public void Pause()
    {
        play.enabled = true;
        PauseAllObjects();
        auxSpeed = int.Parse(timeSpeed.text);
        Time.timeScale = 0;
    }

    private void PauseAllObjects()
    {
        foreach (GameObject go in listUi) {
            go.SetActive(false);
        }
    }

    private void PlayAllObjects()
    {
        foreach (GameObject go in listUi)
        {
            go.SetActive(true);
        }
    }
}
