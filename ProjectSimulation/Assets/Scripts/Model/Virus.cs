using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Virus
{
    [SerializeField] private float infectionRate = 0.3f;
    [SerializeField] private float exposedRate = 0.1f;
    [SerializeField] private float recoveryRate = 0.2f;
    [SerializeField] private float mortalityRate = 0.01f;
    [SerializeField] private static Virus instance;

    public static Virus GetInstance()
    {
        if (instance.IsNull())
        {
            instance = new Virus();
        }

        return instance;
    }


    public bool IsNull()
    {
        return instance == null ? true : false;
    }

    public float InfectionRate { get {  return  infectionRate; } }

    public float ExposedRate {  get { return exposedRate; } }

    public float RecoveryRate { get { return recoveryRate; } }

    public float Mortality { get { return mortalityRate; } }

}
