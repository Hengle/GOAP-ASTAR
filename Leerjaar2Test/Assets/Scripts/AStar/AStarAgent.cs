using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class AStarAgent : MonoBehaviour {
    public GameObject target;
    Vector3 targetPos;
    public Thread calculateThread;
    Vector3 currentTile;
    Vector3 startTile;
    TileData[,,] localTilesData;
    List<Vector3> checkTiles = new List<Vector3>();
    List<Vector3> walkedTiles = new List<Vector3>();
    List<Vector3> path = new List<Vector3>();
    List<Vector3> cleanTiles = new List<Vector3>();
    public bool walking;
	// Use this for initialization
	void Start () {
        
	}

    // Update is called once per frame
    private void Reset()
    {
        checkTiles = new List<Vector3>();
        walkedTiles = new List<Vector3>();
        path = new List<Vector3>();
        cleanTiles = new List<Vector3>();
        localTilesData = new TileData[AStarBuilder.builder.xTiles, AStarBuilder.builder.yTiles, AStarBuilder.builder.zTiles];
    }
    void Update () {
        if (walking)
        {
            if (Vector3.Distance(transform.position, path[path.Count - 1]) <= AStarBuilder.builder.tileSize * 0.2f)
            {
                path.RemoveAt(path.Count - 1);
                if (path.Count == 0)
                {
                    walking = false;
                    target = null;
                    GetComponent<GOAPAgent>().state = GOAPAgent.AgentState.Using;
                }
            }
            else
            {
                transform.position = Vector3.MoveTowards(transform.position, path[path.Count - 1], 0.25f);
            }
        }
    }
    public void InitializeStart()
    {
        print("INITIALIZINGSTART");
        Reset();
        targetPos = target.transform.position;
        targetPos.x = Mathf.Round(targetPos.x);
        targetPos.y = Mathf.Round(targetPos.y);
        targetPos.z = Mathf.Round(targetPos.z);
        targetPos -= AStarBuilder.builder.lowestPoint;
        currentTile = transform.position;
        currentTile.x = Mathf.Round(currentTile.x);
        currentTile.y = Mathf.Round(currentTile.y);
        currentTile.z = Mathf.Round(currentTile.z);
        currentTile -= AStarBuilder.builder.lowestPoint;
        startTile = currentTile;
        walkedTiles.Add(currentTile);
        if(currentTile.x < AStarBuilder.builder.xTiles && currentTile.y < AStarBuilder.builder.yTiles && currentTile.z < AStarBuilder.builder.zTiles)
        {
            if(targetPos.x < AStarBuilder.builder.xTiles && targetPos.y < AStarBuilder.builder.yTiles && targetPos.z < AStarBuilder.builder.zTiles)
            {
                print("REACHABLE");
                StartCoroutine(NextCheck());
            }
            else
            {
                print("NOT REACHABLE");
            }
        }
        else
        {
            print("NOT REACHABLE");
        }
    }
    void CheckSurroundingTiles()
    {
        print("STARTED CHECKING");
        checkTiles = new List<Vector3>();
        for(int x = -1; x <= 1; x++)
        {
            for(int y = -1; y <= 1; y++)
            {
                for (int z = -1; z <= 1; z++)
                {
                    if (x == 0 && y == 0 && z == 0)
                    {

                    }
                    else
                    {
                        Vector3 thisTile = new Vector3(x, y, z) + currentTile;
                        if (thisTile.x < AStarBuilder.builder.xTiles && thisTile.y < AStarBuilder.builder.yTiles && thisTile.z < AStarBuilder.builder.zTiles)
                        {
                            if (thisTile.x >= 0 && thisTile.y >= 0 && thisTile.z >= 0)
                            {
                                if (!AStarBuilder.builder.tiles[(int)thisTile.x, (int)thisTile.y, (int)thisTile.z].notWalkable)
                                {
                                    bool canWalk = true;
                                    for (int i = 0; i < walkedTiles.Count; i++)
                                    {
                                        if (walkedTiles[i] == thisTile)
                                        {
                                            canWalk = false;
                                        }
                                    }
                                    if (canWalk)
                                    {
                                        checkTiles.Add(thisTile);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        if(checkTiles.Count > 0)
        {
            CalculateCost();
        }
        else
        {
            Backtrack();
            print("NEED TO BACKTRACK");
        }
    }
    void CalculateCost()
    {
        print("CALCULATING");
        Vector3 nextTile = new Vector3();
        float lowestCost = Mathf.Infinity;

        for(int i = 0; i < checkTiles.Count; i++)
        {
            float cost = 0;
            cost += Mathf.Abs(checkTiles[i].x) + Mathf.Abs(startTile.x);
            cost += Mathf.Abs(checkTiles[i].y) + Mathf.Abs(startTile.y);
            cost += Mathf.Abs(checkTiles[i].x) + Mathf.Abs(startTile.x);

            cost += Mathf.Abs(checkTiles[i].x) + Mathf.Abs(targetPos.x);
            cost += Mathf.Abs(checkTiles[i].y) + Mathf.Abs(targetPos.y);
            cost += Mathf.Abs(checkTiles[i].x) + Mathf.Abs(targetPos.z);
            if (Vector3.Distance(checkTiles[i] , targetPos) < lowestCost)
            {
                lowestCost = Vector3.Distance(checkTiles[i], targetPos);
                nextTile = checkTiles[i];
            }
        }
        localTilesData[(int)nextTile.x, (int)nextTile.y, (int)nextTile.z].previousTile = currentTile;
        localTilesData[(int)nextTile.x, (int)nextTile.y, (int)nextTile.z].previousTileWalked = true;
        currentTile = nextTile;
        walkedTiles.Add(currentTile);
        if(Vector3.Distance(targetPos, currentTile) < AStarBuilder.builder.tileSize * 2.5f)
        {
            path = new List<Vector3>();
            ShowPath(currentTile);
        }
        else
        {
            StartCoroutine(NextCheck());
            print("REBOOTING");
        }
    }
    void ShowPath(Vector3 tile)
    {
        path.Add(tile);
        if (localTilesData[(int)tile.x, (int)tile.y, (int)tile.z].previousTileWalked)
        {
            ShowPath(localTilesData[(int)tile.x, (int)tile.y, (int)tile.z].previousTile);
        }
        else
        {
            print("CLEANING");
           CleanPath();
        }
    }
    IEnumerator NextCheck()
    {
        yield return null;
        CheckSurroundingTiles();
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        if (path.Count > 0)
        {
            for (int i = 0; i < path.Count; i++)
            {
                Gizmos.DrawCube(path[i], new Vector3(0.8f, 0.8f, 0.8f));
            }
        }
        Gizmos.color = Color.cyan;
        for(int i = 0; i < cleanTiles.Count; i++)
        {
            Gizmos.DrawCube(cleanTiles[i], Vector3.one * 1.1f);
        }
    }
    void CleanPath()
    {
        for(int i = 0; i < path.Count; i++)
        {
            for(int q = path.Count - 1; q >=  i; q--)
            {
                if(Mathf.Abs(path[q].x - path[i].x) == 1 && Mathf.Abs(path[q].z - path[i].z) == 1 && Mathf.Abs(path[q].y - path[i].y) == 1)
                {
                    if(Vector3.Distance(path[q], transform.position) < Vector3.Distance(localTilesData[(int)path[i].x, (int)path[i].y, (int)path[i].z].previousTile, transform.position))
                    {
                        localTilesData[(int)path[i].x, (int)path[i].y, (int)path[i].z].previousTile = path[q];
                        i = q;
                        break;
                    }
                }
                else
                {
                    if (Mathf.Abs(path[q].x - path[i].x) == 1 && Mathf.Abs(path[q].z - path[i].z) == 0 && Mathf.Abs(path[q].y - path[i].y) == 0)
                    {
                        if (Vector3.Distance(path[q], transform.position) < Vector3.Distance(localTilesData[(int)path[i].x, (int)path[i].y, (int)path[i].z].previousTile, transform.position))
                        {
                            localTilesData[(int)path[i].x, (int)path[i].y, (int)path[i].z].previousTile = path[q];
                            i = q;
                            break;
                        }
                    }
                    else
                    {
                        if (Mathf.Abs(path[q].z - path[i].z) == 1 && Mathf.Abs(path[q].x - path[i].x) == 0 && Mathf.Abs(path[q].y - path[i].y) == 0)
                        {
                            if (Vector3.Distance(path[q], transform.position) < Vector3.Distance(localTilesData[(int)path[i].x, (int)path[i].y, (int)path[i].z].previousTile, transform.position))
                            {
                                localTilesData[(int)path[i].x, (int)path[i].y, (int)path[i].z].previousTile = path[q];
                                i = q;
                                break;
                            }
                        }
                        else
                        {
                            if(Mathf.Abs(path[q].z - path[i].z) == 1 && Mathf.Abs(path[q].x - path[i].x) == 1 && Mathf.Abs(path[q].y - path[i].y) == 0)
                            {
                                if (Vector3.Distance(path[q], transform.position) < Vector3.Distance(localTilesData[(int)path[i].x, (int)path[i].y, (int)path[i].z].previousTile, transform.position))
                                {
                                    localTilesData[(int)path[i].x, (int)path[i].y, (int)path[i].z].previousTile = path[q];
                                    i = q;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }
        print("CLEANED");
        path = new List<Vector3>();
        StartCoroutine(Finish(currentTile));
    }
    void Backtrack()
    {
        currentTile = localTilesData[(int)currentTile.x, (int)currentTile.y, (int)currentTile.z].previousTile;
        CheckSurroundingTiles();
    }
    IEnumerator Finish(Vector3 tile)
    {

        path.Add((tile + AStarBuilder.builder.lowestPoint));
        if (localTilesData[(int)tile.x, (int)tile.y, (int)tile.z].previousTileWalked)
        {
            yield return null;
            StartCoroutine(Finish(localTilesData[(int)tile.x, (int)tile.y, (int)tile.z].previousTile));
        }
        else
        {
            walking = true;
        }
    }
    public struct TileData
    {
        public Vector3 previousTile;
        public bool previousTileWalked;
        public float cost;
        public bool calculated;
    }
}
