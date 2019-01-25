using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bed : GOAPObject {
    public float happynessAmt;
    public float energyAmt;
    public override void Effect(GameObject interactor)
    {
        float newVal = (float)interactor.GetComponent<GOAPAgent>().playerValues["Happyness"];
        newVal += happynessAmt;
        interactor.GetComponent<GOAPAgent>().playerValues["Happyness"] = newVal;
        newVal = (float)interactor.GetComponent<GOAPAgent>().playerValues["Energy"];
        newVal += energyAmt;
        interactor.GetComponent<GOAPAgent>().playerValues["Energy"] = newVal;
        base.Effect(interactor);
    }
    public override void Awake()
    {
        effects.Add("Energy", energyAmt);
        effects.Add("Happyness", happynessAmt);
    }
}
