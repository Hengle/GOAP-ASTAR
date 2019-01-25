using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GOAPAgent : MonoBehaviour {
    public enum AgentState {Thinking, Walking, Using, Nothing }
    public Hashtable playerState = new Hashtable();
    public Hashtable playerValues = new Hashtable();
    public List<GameObject> path = new List<GameObject>();
    public string[] randomActions;
    public AgentState state;
    [Range(1,50)]
    public int lowest;
    // Use this for initialization
    void Awake () {
        playerValues.Add("Hunger", (float)50);
        playerValues.Add("Happyness", (float)50);
        playerValues.Add("Energy", (float)50);
        playerValues.Add("Health", (float)50);
    }
    public void Start()
    {
        StartCoroutine(StatDecrease());
    }

    // Update is called once per frame
    IEnumerator StatDecrease()
    {
        while (true)
        {
            yield return new WaitForSeconds(2);
            ChangeStat("Hunger", -1);
            ChangeStat("Health", -1);
            ChangeStat("Energy", -1);
            ChangeStat("Happyness", -1);
        }
    }
    public void ChangeStat(string name, float amount)
    {
        float newStat;
        newStat = (float)playerValues[name];
        newStat += amount;
        playerValues[name] = newStat;
    }

    void Update ()
    {
        switch (state)
        {
            case AgentState.Nothing:

                break;
            case AgentState.Thinking:
                StartCoroutine(Think());
                state = AgentState.Nothing;
                break;
            case AgentState.Walking:
                GetComponent<AStarAgent>().target = path[path.Count - 1];
                GetComponent<AStarAgent>().InitializeStart();
                state = AgentState.Nothing;
                break;
            case AgentState.Using:
                path[path.Count - 1].GetComponent<GOAPObject>().Effect(gameObject);
                path.RemoveAt(path.Count - 1);
                if(path.Count > 0)
                {
                    state = AgentState.Walking;
                }
                else
                {
                    state = AgentState.Thinking;
                }
                break;
        }
	}
    IEnumerator Think()
    {
        path = new List<GameObject>();
        yield return new WaitForSeconds(Random.Range(5, 7));
        if((float)playerValues["Health"] < lowest)
        {
            path = GOAPPlanner.planner.StartSearch("Health", 0, this);
        }
        else
        {
            if((float)playerValues["Hunger"] < lowest)
            {
                print("FINDING HUNGER");
                path = GOAPPlanner.planner.StartSearch("Hunger", 20, this);
                print("FOUND HUNGER");
            }
            else
            {
                if((float)playerValues["Energy"] < lowest)
                {
                    path = GOAPPlanner.planner.StartSearch("Energy", 0, this);
                }
                else
                {
                    if((float)playerValues["Happyness"] < lowest)
                    {
                        path = GOAPPlanner.planner.StartSearch("Happyness", 0, this);
                    }
                    else
                    {
                        if(randomActions.Length > 0)
                        {
                            path = GOAPPlanner.planner.StartSearch(randomActions[Random.Range(0, randomActions.Length)], 0, this);
                        }
                    }
                }
            }
        }
        if(path != null && path.Count > 0)
        {
            print(path.Count);
            state = AgentState.Walking;
        }
        else
        {
            if(randomActions.Length > 0)
            {
                while (path.Count == 0)
                {
                    path = GOAPPlanner.planner.StartSearch(randomActions[Random.Range(0, randomActions.Length)], 0 , this);
                }
            }
            else
            {
                StartCoroutine(Think());
            }
        }
        //Think of what action the AI wants to do
    }
}
