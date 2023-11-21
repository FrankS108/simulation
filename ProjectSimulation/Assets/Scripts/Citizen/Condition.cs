using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Condition
{
    /*
     * Se tienen multiple estados, cada estado tiene un color y una probabilidad de infectar a otra persona.
     */
    private string name;
    private Color color;
    private double rate;

    public Condition(string name, Color color, double rate)
    {
        this.name = name;
        this.color = color;
        this.rate = rate;
    }

    public string Name { get { return name; } }
    public Color Color { get { return color; } }
    public double Rate { get { return rate; } }
}
