using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shower : GOAPObject {
    public float happynessAmt;
    public float healthAmt;

    public override void Effect(GameObject interactor)
    {
        float newVal = (float)interactor.GetComponent<GOAPAgent>().playerValues["Health"];
        newVal += healthAmt;
        interactor.GetComponent<GOAPAgent>().playerValues["Health"] = newVal;
        newVal = (float)interactor.GetComponent<GOAPAgent>().playerValues["Happyness"];
        newVal += happynessAmt;
        interactor.GetComponent<GOAPAgent>().playerValues["Happyness"] = newVal;
        base.Effect(interactor);
    }
    public override void Awake()
    {
        effects.Add("Health", happynessAmt);
        effects.Add("Happyness", healthAmt);
    }
}
