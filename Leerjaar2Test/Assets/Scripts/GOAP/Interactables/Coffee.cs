using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coffee : GOAPObject {
    public float energyAmt;
    public float hungerAmt;
    public override void Effect(GameObject interactor)
    {
        float newVal = (float)interactor.GetComponent<GOAPAgent>().playerValues["Hunger"];
        newVal += hungerAmt;
        interactor.GetComponent<GOAPAgent>().playerValues["Hunger"] = newVal;
        newVal = (float)interactor.GetComponent<GOAPAgent>().playerValues["Energy"];
        newVal += energyAmt;
        interactor.GetComponent<GOAPAgent>().playerValues["Energy"] = newVal;
    }
    public override void Awake()
    {
        effects.Add("Energy", energyAmt);
        effects.Add("Hunger", hungerAmt);
    }
}
