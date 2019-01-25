using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GOAPObject : MonoBehaviour {
    public Precondition[] preconditions;
    public int cost;
    public bool singleUse;
    public Hashtable effects = new Hashtable();

    public abstract void Awake();
    public virtual void Effect(GameObject interactor)
    {
        foreach (Precondition precondition in preconditions)
        {
            interactor.GetComponent<GOAPAgent>().ChangeStat(precondition.precondition, -precondition.requiredAmount);
        }
        if (singleUse)
        {
            Destroy(gameObject);
        }
    }
}
[System.Serializable]
public struct Precondition
{
    public string precondition;
    public int requiredAmount;
}
