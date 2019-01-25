using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarBuilder : MonoBehaviour {
    public static AStarBuilder builder;
    [HideInInspector]
    public Vector3 lowestPoint;
    Vector3 highestPoint;
    [Range(1, 3)]
    public int heightCheckTiles;
    public LayerMask detectableObjects;
    [Range(1, 10)]
    public int tileSize;
    public TileData[,,] tiles;
    [HideInInspector]
    public int xTiles;
    [HideInInspector]
    public int yTiles;
    [HideInInspector]
    public int zTiles;
    // Use this for initialization
    void Awake () {
        builder = this;
        CalculatePointsGrid();
	}
	
	// Update is called once per frame
	void Update () {

	}
    void CalculatePointsGrid()
    {
        lowestPoint = new Vector3(Mathf.Infinity, Mathf.Infinity, Mathf.Infinity);
        highestPoint = new Vector3(Mathf.NegativeInfinity, Mathf.NegativeInfinity, Mathf.NegativeInfinity);
        GameObject[] terrain = GameObject.FindGameObjectsWithTag("Terrain");
        for(int i = 0; i < terrain.Length; i++)
        {
            Vector3 thisLowest = terrain[i].GetComponent<Collider>().bounds.min;
            thisLowest.x = Mathf.RoundToInt(thisLowest.x);
            thisLowest.y = Mathf.RoundToInt(terrain[i].GetComponent<Collider>().bounds.max.y);
            thisLowest.z = Mathf.RoundToInt(thisLowest.z);
            if(thisLowest.x < lowestPoint.x)
            {
                lowestPoint.x = thisLowest.x;
            }
            if(thisLowest.y < lowestPoint.y)
            {
                lowestPoint.y = thisLowest.y;
            }
            if(thisLowest.z < lowestPoint.z)
            {
                lowestPoint.z = thisLowest.z;
            }
            Vector3 thisHighest = terrain[i].GetComponent<Collider>().bounds.max;
            thisHighest.x = Mathf.RoundToInt(thisHighest.x);
            thisHighest.y = Mathf.RoundToInt(terrain[i].GetComponent<Collider>().bounds.max.y);
            thisHighest.z = Mathf.RoundToInt(thisHighest.z);
            if (thisHighest.x > highestPoint.x)
            {
                highestPoint.x = thisHighest.x;
            }
            if (thisHighest.y > highestPoint.y)
            {
                highestPoint.y = thisHighest.y;
            }
            if (thisHighest.z > highestPoint.z)
            {
                highestPoint.z = thisHighest.z;
            }
        }
        BuildGrid();
    }
    void BuildGrid()
    {
        xTiles = ((int)highestPoint.x - (int)lowestPoint.x)+ 1;
        yTiles = ((int)highestPoint.y - (int)lowestPoint.y)+ 1;
        zTiles = ((int)highestPoint.z - (int)lowestPoint.z)+ 1;
        tiles = new TileData[xTiles, yTiles, zTiles];
        print("Build");
        InitializeGrid();
    }
    void InitializeGrid()
    {
        for (int x = (int)lowestPoint.x; x <= (int)highestPoint.x; x += tileSize)
        {
            for (int y = (int)lowestPoint.y; y <= (int)highestPoint.y; y += tileSize)
            {
                for (int z = (int)lowestPoint.z; z <= (int)highestPoint.z; z += tileSize)
                {
                    Vector3 arrayPosition = new Vector3(x - lowestPoint.x, y - lowestPoint.y, z - lowestPoint.z);
                    tiles[(int)arrayPosition.x, (int)arrayPosition.y, (int)arrayPosition.z].worldPosition = new Vector3(x, y, z);
                    Collider[] groundCheck = Physics.OverlapBox(new Vector3(x, y, z), new Vector3((float)tileSize / 1.95f, (float)tileSize / 1.95f, (float)tileSize / 1.95f), Quaternion.identity, detectableObjects, QueryTriggerInteraction.Ignore);
                    if(groundCheck.Length > 0)
                    {
                        for(int q = 0; q < groundCheck.Length; q++)
                        {
                            if (groundCheck[q].tag == "Obstacle")
                            {
                                tiles[(int)arrayPosition.x, (int)arrayPosition.y, (int)arrayPosition.z].notWalkable = true;
                                break;
                            }
                            else
                            {
                                for (int i = 1; i <= heightCheckTiles + 1; i++)
                                {
                                    if (Physics.CheckBox(new Vector3(x, (y + (tileSize * i)), z), new Vector3((float)tileSize / 1.95f, (float)tileSize / 1.95f, (float)tileSize / 1.95f), Quaternion.identity, detectableObjects, QueryTriggerInteraction.Ignore))
                                    {
                                        tiles[(int)arrayPosition.x, (int)arrayPosition.y, (int)arrayPosition.z].notWalkable = true;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        tiles[(int)arrayPosition.x, (int)arrayPosition.y, (int)arrayPosition.z].notWalkable = true;
                    }
                    if(tiles[(int)arrayPosition.x, (int)arrayPosition.y, (int)arrayPosition.z].notWalkable)
                    {

                    }
                }
            }
        }
        print("INITIALIZED");
    }
    [System.Serializable]
    public struct TileData
    {
        public Vector3 worldPosition;
        public bool notWalkable;
        public int cost;
        public bool count;
    }
}
