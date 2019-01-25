using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GOAPPlanner : MonoBehaviour {
    public float maxSteps;
    public static GOAPPlanner planner;
    public GOAPAgent agentPlanningFor;
    public List<PlanData> finalTiles;
    public List<GameObject> ignore;
    public List<PlanData> planData = new List<PlanData>();
    public List<Precondition> precondQueue = new List<Precondition>();
    public List<GameObject> path = new List<GameObject>();
    // Use this for initialization
    void Awake () {
        planner = this;
	}
    public void Reset()
    {
        path = new List<GameObject>();
        precondQueue = new List<Precondition>();
        ignore = new List<GameObject>();
        planData = new List<PlanData>();
        finalTiles = new List<PlanData>();
    }
    public List<GameObject> StartSpecific(GameObject target, GOAPAgent thisAgent)
    {
        Reset();
        Hashtable backupStats = thisAgent.playerValues;
        agentPlanningFor = thisAgent;
        print("SEARCHING");
        planData.Add(new PlanData(target, null, 0));
        if (target.GetComponent<GOAPObject>().singleUse)
        {
            ignore.Add(target);
        }
        if (target.GetComponent<GOAPObject>().preconditions.Length > 0)
        {
            for(int condition = 0; condition < target.GetComponent<GOAPObject>().preconditions.Length; condition++)
            {
                if (backupStats.ContainsKey(target.GetComponent<GOAPObject>().preconditions[condition].precondition))
                {
                    if((float)backupStats[target.GetComponent<GOAPObject>().preconditions[condition].precondition] >= target.GetComponent<GOAPObject>().preconditions[condition].requiredAmount)
                    {
                        float newVal = (float)backupStats[target.GetComponent<GOAPObject>().preconditions[condition].precondition];
                        newVal -= target.GetComponent<GOAPObject>().preconditions[condition].requiredAmount;
                        backupStats[target.GetComponent<GOAPObject>().preconditions[condition].precondition] = newVal;
                    }
                    else
                    {
                        float remainingVal = (float)backupStats[target.GetComponent<GOAPObject>().preconditions[condition].precondition];
                        remainingVal -= target.GetComponent<GOAPObject>().preconditions[condition].requiredAmount;
                        backupStats[target.GetComponent<GOAPObject>().preconditions[condition].precondition] = 0;
                        precondQueue.Add(new Precondition(target.GetComponent<GOAPObject>().preconditions[condition].precondition, remainingVal));
                    }
                }
                else
                {
                    return null;
                }
            }
            if(precondQueue.Count > 0)
            {
                ActionSearch(precondQueue[precondQueue.Count - 1].precondition, precondQueue[precondQueue.Count - 1].requiredAmount, planData[planData.Count - 1], precondQueue, 0, backupStats);
            }
        }
        else
        {
            finalTiles.Add(planData[planData.Count - 1]);
        }
        path = new List<GameObject>();
        BuildPath(CheapestPath());
        return path;
    }
    public List<GameObject> StartSearch(string effect, float requiredAmt, GOAPAgent thisAgent)
    {
        Reset();
        agentPlanningFor = thisAgent;
        Hashtable backupStats = thisAgent.playerValues;
        print("SEARCHING");
        precondQueue.Add(new Precondition("Holder"));
        ActionSearch(effect, requiredAmt, null, precondQueue, 0, backupStats);
        path = null;
        if (finalTiles.Count > 0)
        {
            path = new List<GameObject>();
            BuildPath(CheapestPath());
        }
        else
        {
            print("NOT ENOUGH OBJECTS");
        }
        return path;
        //PATH DONE
    }
    public PlanData CheapestPath()
    {
        int cheapestIndex = 0;
        int cheapestCost = 999999999;
        for(int i = 0; i < finalTiles.Count; i++)
        {
            if(finalTiles[i].totalCost < cheapestCost)
            {
                cheapestIndex = i;
                cheapestCost = finalTiles[i].totalCost;
            }
        }
        print(finalTiles.Count);
        print(cheapestIndex);
        print(finalTiles[cheapestIndex]);
        return finalTiles[cheapestIndex];
    }
    public void ActionSearch(string effect, float remainingAmount, PlanData data, List<Precondition> remainingPreconds, int counter, Hashtable remainingValues)
    {
        print("SEARCH");
        counter++;
        GameObject[] interactables = GameObject.FindGameObjectsWithTag("Interactable");
        interactables = RemoveIgnoreObjects(interactables);
        if (interactables.Length > 0)
        {
            remainingPreconds.RemoveAt(remainingPreconds.Count - 1);
        }
        else
        {
            return;
        }
        for (int i = 0; i < interactables.Length; i++)
        {
            if (interactables[i].GetComponent<GOAPObject>().effects.ContainsKey(effect))
            {
                if(data == null)
                {
                    planData.Add(new PlanData(interactables[i], null, interactables[i].GetComponent<GOAPObject>().cost));
                    data = planData[planData.Count - 1];
                }
                else
                {
                    data.path.Add(new PlanData(interactables[i], data, data.totalCost));
                }
                if (interactables[i].GetComponent<GOAPObject>().singleUse)
                {
                    ignore.Add(interactables[i]);
                }
                if (interactables[i].GetComponent<GOAPObject>().preconditions.Length > 0)
                {
                    for (int condition = 0; condition < interactables[i].GetComponent<GOAPObject>().preconditions.Length; condition++)
                    {
                        if (remainingValues.ContainsKey(interactables[i].GetComponent<GOAPObject>().preconditions[condition].precondition))
                        {
                                if ((float)remainingValues[interactables[i].GetComponent<GOAPObject>().preconditions[condition].precondition] >= interactables[i].GetComponent<GOAPObject>().preconditions[condition].requiredAmount)
                                {
                                    float newVal = (float)remainingValues[interactables[i].GetComponent<GOAPObject>().preconditions[condition].precondition];
                                    newVal -= interactables[i].GetComponent<GOAPObject>().preconditions[condition].requiredAmount;
                                    remainingValues[interactables[i].GetComponent<GOAPObject>().preconditions[condition].precondition] = newVal;
                                }
                                else
                                {
                                    float remainingVal = interactables[i].GetComponent<GOAPObject>().preconditions[condition].requiredAmount;
                                    remainingVal -= (float)remainingValues[interactables[i].GetComponent<GOAPObject>().preconditions[condition].precondition];
                                    remainingValues[interactables[i].GetComponent<GOAPObject>().preconditions[condition].precondition] = 0;
                                    remainingPreconds.Add(new Precondition(interactables[i].GetComponent<GOAPObject>().preconditions[condition].precondition, remainingVal));
                                }
                        }
                        else
                        {
                            return;
                        }
                    }
                }
                if ((float)interactables[i].GetComponent<GOAPObject>().effects[effect] < remainingAmount)
                {
                    float newAmt = remainingAmount;
                    newAmt -= (float)interactables[i].GetComponent<GOAPObject>().effects[effect];
                    remainingPreconds.Add(new Precondition(effect, newAmt));
                }
                if (remainingPreconds.Count > 0)
                {
                    if(counter == maxSteps - 1)
                    {
                        return;
                    }
                    else
                    {
                        if(data.path.Count > 0)
                        {
                            ActionSearch(remainingPreconds[remainingPreconds.Count - 1].precondition, remainingPreconds[remainingPreconds.Count - 1].requiredAmount, data.path[data.path.Count - 1], remainingPreconds, counter, remainingValues);
                        }
                        else
                        {
                            ActionSearch(remainingPreconds[remainingPreconds.Count - 1].precondition, remainingPreconds[remainingPreconds.Count - 1].requiredAmount, data, remainingPreconds, counter, remainingValues);
                        }
                        print("MORE PRE TO DO ");
                    }
                }
                else
                {
                    if(data.path.Count > 0)
                    {
                        finalTiles.Add(data.path[data.path.Count - 1]);
                    }
                    else
                    {
                        finalTiles.Add(data);
                    }
                }
            }
        }
    }
    public void BuildPath(PlanData pathToCheck)
    {
        path.Add(pathToCheck.interactableObject);
        if(pathToCheck.previousNode != null)
        {
            BuildPath(pathToCheck.previousNode);
        }
    }
    public bool CheckPrecondition(string condition, float value)
    {
        if (agentPlanningFor.playerState.ContainsKey(condition))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public GameObject[] RemoveIgnoreObjects(GameObject[] objects)
    {
        List<GameObject> returner = new List<GameObject>();
        foreach(GameObject checkObject in objects)
        {
            returner.Add(checkObject);
            foreach(GameObject ignoreObject in ignore)
            {
                if(checkObject == ignoreObject)
                {
                    returner.Remove(checkObject);
                    break;
                }
            }
        }
        return returner.ToArray();
    }
    [System.Serializable]
    public class PlanData
    {
        public List<PlanData> path = new List<PlanData>();
        public PlanData previousNode;
        public GameObject interactableObject;
        public int totalCost;

        public PlanData(GameObject theObject, PlanData currentNode, int currentCost)
        {
            interactableObject = theObject;
            previousNode = currentNode;
            totalCost = currentCost + interactableObject.GetComponent<GOAPObject>().cost;
        }
    }
    [System.Serializable]
    public class Precondition
    {
        public string precondition;
        public float requiredAmount;

        public Precondition(string precon, float requiredAmt = 0)
        {
            precondition = precon;
            requiredAmount = requiredAmt;
        }
    }
}
