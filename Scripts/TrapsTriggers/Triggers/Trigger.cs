using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//abstract class; does not get implemented
public abstract class Trigger : MonoBehaviour {

    //the attached trap getting activated
    [HideInInspector]
    public Trap trap;
    public List<Trap> traps = new List<Trap>(5);

    //activates the attached traps
    protected void trigger()
    {
        for(int i = 0; i < traps.Capacity; i++)
        {
            if (traps[i] != null)
            {
                trap = traps[i];
                trap.activate();
            }
        }
    }

    //deactivates the attached traps
    protected void detrigger()
    {
        for (int i = 0; i < traps.Capacity; i++)
        {
            if (traps[i] != null)
            {
                trap = traps[i];
                trap.deactivate();
            }
        }
    }
}
