using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleDecisionTree : MonoBehaviour {
    public int maxSearch;
    public int minSearch;
    public int findNum;
    public bool find;
    bool canfind = true;
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
        if (find)
        {
            find = false;
            StartCoroutine(FindNum(findNum));
        }
	}
    IEnumerator FindNum(int number)
    {
        if (canfind)
        {
            canfind = false;
            int min = minSearch;
            int max = maxSearch + 1;
            int current = (max - min) / 2;
            if (number >= minSearch && number <= maxSearch && number > 0)
            {
                while (current != number)
                {
                    yield return new WaitForSeconds(0.1f);
                    if (number < current)
                    {
                        max = current;
                        current = (current + min) / 2;
                        print(current);
                    }
                    else
                    {
                        min = current;
                        current = (current + max) / 2;
                        print(current);
                    }
                }
            }
            print("Your number is " + current);
            canfind = true;
        }
    }
}
