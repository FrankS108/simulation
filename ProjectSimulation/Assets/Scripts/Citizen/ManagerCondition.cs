using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagerCondition
{
    private SpriteRenderer state;
    private int index = 0;

    public ManagerCondition(GameObject state, string initialState) { 
        this.state = state.GetComponent<SpriteRenderer>();
        index = findStateColorByName(initialState);
        this.state.color = conditions[index].Color;
    }

    public List<Condition> conditions = new List<Condition>()
    {
        new Condition("SANO", Color.green, 0),
        new Condition("INFECTADO", Color.red, 2),
        new Condition("RECUPERADO", new Color(0, 176/255f, 255/255f, 255/255f), 1.5),
        new Condition("MUERTO", Color.black, 0),
    };

    //Determinar como va ser representado el estado.
    public void UpdateCondition(string state)
    {
        index = findStateColorByName(state);
        this.state.color = conditions[index].Color;
    }

    private int findStateColorByName(string state)
    {
        foreach(Condition condition in conditions) 
        { 
            if(condition.Name == state)
            {
                return conditions.IndexOf(condition);
            }
        }

        return -1;
    }

}
