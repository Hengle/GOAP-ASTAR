using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpdateStats : MonoBehaviour {
    public GOAPAgent agentTracking;
    public Image[] fillbars;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        fillbars[0].fillAmount = CalcFillAmount((float)agentTracking.playerValues["Health"]);
        fillbars[1].fillAmount = CalcFillAmount((float)agentTracking.playerValues["Hunger"]);
        fillbars[2].fillAmount = CalcFillAmount((float)agentTracking.playerValues["Energy"]);
        fillbars[3].fillAmount = CalcFillAmount((float)agentTracking.playerValues["Happyness"]);
    }
    float CalcFillAmount(float currentStat)
    {
        return currentStat / 100;
    }
}
