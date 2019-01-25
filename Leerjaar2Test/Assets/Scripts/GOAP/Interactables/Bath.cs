using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bath : GOAPObject {
    public float healthAmt;
    public float energyAmt;
    public override void Effect(GameObject interactor)
    {
        float newVal = (float)interactor.GetComponent<GOAPAgent>().playerValues["Health"];
        newVal += healthAmt;
        interactor.GetComponent<GOAPAgent>().playerValues["Health"] = newVal;
        newVal = (float)interactor.GetComponent<GOAPAgent>().playerValues["Energy"];
        newVal += energyAmt;
        interactor.GetComponent<GOAPAgent>().playerValues["Energy"] = newVal;
        base.Effect(interactor);
    }
    public override void Awake()
    {
        effects.Add("Energy", energyAmt);
        effects.Add("Health", healthAmt);
    }
}
