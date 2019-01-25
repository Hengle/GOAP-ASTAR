using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Medkit : GOAPObject {

    public float healthAmt;

    public override void Effect(GameObject interactor)
    {
        float newVal = (float)interactor.GetComponent<GOAPAgent>().playerValues["Health"];
        newVal += healthAmt;
        interactor.GetComponent<GOAPAgent>().playerValues["Health"] = newVal;
        base.Effect(interactor);
    }
    public override void Awake()
    {
        effects.Add("Happyness", healthAmt);
    }
}
