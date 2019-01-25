using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Apple : GOAPObject {

    public float hungerAmt;
    public float healthAmt;

    public override void Effect(GameObject interactor)
    {
        float newVal = (float)interactor.GetComponent<GOAPAgent>().playerValues["Health"];
        newVal += healthAmt;
        interactor.GetComponent<GOAPAgent>().playerValues["Health"] = newVal;
        newVal = (float)interactor.GetComponent<GOAPAgent>().playerValues["Hunger"];
        newVal += hungerAmt;
        interactor.GetComponent<GOAPAgent>().playerValues["Hunger"] = newVal;
        base.Effect(interactor);
    }
    public override void Awake()
    {
        effects.Add("Hunger", hungerAmt);
        effects.Add("Happyness", healthAmt);
    }
}
