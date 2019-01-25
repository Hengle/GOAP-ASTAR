using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActualTree : MonoBehaviour {
    public Node baseNode;
    [Header("ADD NUMBER")]
    public int addNum;
    public bool addTheNumber;
    [Header("FIND NUMBER")]
    public int findNum;
    public bool findTheNum;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (addTheNumber)
        {
            addTheNumber = false;
            baseNode.AddNumber(addNum);
            addNum = 0;
        }
        if (findTheNum)
        {
            findTheNum = false;
            baseNode.GetNumber(findNum);
        }
	}
    void FindNum(int num)
    {
        Node currentNode = baseNode;
        List<Node> nodes = new List<Node>();
        nodes.Add(currentNode);
        while(currentNode.number != num)
        {
            if(num < currentNode.number)
            {
                currentNode = currentNode.lower;
            }
            else
            {
                currentNode = currentNode.higher;
            }
            nodes.Add(currentNode);
        }
        for(int i = 0; i < nodes.Count; i++)
        {
            print(nodes[i].number);
        }
    }
    [System.Serializable]
    public class Node
    {
        public int number;
        public Node higher;
        public Node lower;


        public void AddNumber(int num)
        {
            if(num != number)
            {
                if(num > number)
                {
                    if(higher == null)
                    {
                        higher = new Node(num);
                        print("ADDED TO HIGHER");
                    }
                    else
                    {
                        print("Going further in tree(higher)");
                        higher.AddNumber(num);
                    }
                }
                else
                {
                    if (lower == null)
                    {
                        lower = new Node(num);
                        print("ADDED TO LOWER");
                    }
                    else
                    {
                        print("Going further in tree(lower)");
                        lower.AddNumber(num);
                    }
                }
            }
            else
            {
                print("NUMBER ALREADY EXCISTS");
            }
        }
        public void GetNumber(int num)
        {
            if(number == num)
            {
                print("GOT IT" + number);
            }
            else
            {
                if(num < number)
                {
                    lower.GetNumber(num);
                }
                else
                {
                    higher.GetNumber(num);
                }
            }
        }
        public Node(int num)
        {
            number = num;
        }
    }
}
