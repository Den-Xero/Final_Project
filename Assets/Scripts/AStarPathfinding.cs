using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PathMarker
{
    public Vector2Int HexLocation;
    public float G;
    public float H;
    public float F;
    public GameObject Marker;
    public PathMarker Parent;

    public PathMarker(Vector2Int hexLocation, float g, float h, float f, GameObject marker, PathMarker parent)
    {
        HexLocation = hexLocation;
        G = g;
        H = h;
        F = f;
        Marker = marker;
        Parent = parent;
    }

    public override bool Equals(object obj)
    {
        if ((obj == null) || this.GetType().Equals(obj.GetType()))
        {
            return false;
        }
        else
        {
            return HexLocation.Equals(((PathMarker)obj).HexLocation);
        }

    }

    public override int GetHashCode()
    {
        return 0;
    }

}







public class AStarPathfinding : MonoBehaviour
{
    public GameMap m_GameMap;
    public Material ClosedMat;
    public Material OpenedMat;

    List<PathMarker> Open = new List<PathMarker>();
    List<PathMarker> Closed = new List<PathMarker>();

    public GameObject StartMarker;
    public GameObject GoalMarker;
    public GameObject Path;

    PathMarker GoalHex;
    PathMarker StartHex;
    PathMarker LastPos;
    bool Done = false;


    void RemoveAllMarkers()
    {
        GameObject[] marker = GameObject.FindGameObjectsWithTag("Marker");
        foreach (GameObject m in marker) Destroy(m);
    }


    void BeginSearch()
    {
        Done = false;
        RemoveAllMarkers();

        List<Vector2Int> hex = new List<Vector2Int>();
        for (int y = 0; y < m_GameMap.GridSize.y; y++)
            for (int x = 0; x < m_GameMap.GridSize.x; x++)
            {
                hex.Add(new Vector2Int(x, y));
            }

        //Randomly pick a hex to have as start location.
        Vector3 startHex = m_GameMap.GetPositionFromCoordinate(new Vector2Int(hex[0].x, hex[0].y));
        StartHex = new PathMarker(new Vector2Int(hex[0].x, hex[0].y), 0, 0, 0, Instantiate(StartMarker, startHex, Quaternion.identity), null);

        //Randomly pick a hex to have as start location.
        Vector3 goalHex = m_GameMap.GetPositionFromCoordinate(new Vector2Int(hex[0].x, hex[0].y));
        GoalHex = new PathMarker(new Vector2Int(hex[1].x, hex[1].y), 0, 0, 0, Instantiate(GoalMarker, goalHex, Quaternion.identity), null);


        Open.Clear();
        Closed.Clear();
        Open.Add(StartHex);
        LastPos = StartHex;

    }


    void PathFinding(PathMarker ThisHex)
    {
        if (ThisHex.Equals(GoalHex)) { Done = true; return; } // goal has been reached.

        if(!m_GameMap.FlatTop)
        {
            if(ThisHex.HexLocation.y % 2 == 0)
            {
                foreach (Vector2Int dir in m_GameMap.PointTopOffsetNeighbours)
                {
                    Vector2Int neighbour = dir + ThisHex.HexLocation;
                    if(neighbour.x < 0 || neighbour.x > m_GameMap.GridSize.x || neighbour.y < 0 || neighbour.y > m_GameMap.GridSize.y) continue;
                    if (IsInClosed(neighbour)) continue;

                    GameObject PathBlock = Instantiate(Path, m_GameMap.GetPositionFromCoordinate(new Vector2Int(neighbour.x, neighbour.y)), Quaternion.identity);
                }
                
            }
            else
            {
                foreach (Vector2Int dir in m_GameMap.PointTopNoOffsetNeighbours)
                {
                    Vector2Int neighbour = dir + ThisHex.HexLocation;
                    if (neighbour.x < 0 || neighbour.x > m_GameMap.GridSize.x || neighbour.y < 0 || neighbour.y > m_GameMap.GridSize.y) continue;
                    if (IsInClosed(neighbour)) continue;

                    GameObject PathBlock = Instantiate(Path, m_GameMap.GetPositionFromCoordinate(new Vector2Int(neighbour.x, neighbour.y)), Quaternion.identity);
                }
            }
        }
        else
        {
            if (ThisHex.HexLocation.x % 2 == 0)
            {
                foreach (Vector2Int dir in m_GameMap.FlatTopOffsetNeighbours)
                {
                    Vector2Int neighbour = dir + ThisHex.HexLocation;
                    if (neighbour.x < 0 || neighbour.x > m_GameMap.GridSize.x || neighbour.y < 0 || neighbour.y > m_GameMap.GridSize.y) continue;
                    if (IsInClosed(neighbour)) continue;

                    GameObject PathBlock = Instantiate(Path, m_GameMap.GetPositionFromCoordinate(new Vector2Int(neighbour.x, neighbour.y)), Quaternion.identity);
                }
            }
            else
            {
                foreach (Vector2Int dir in m_GameMap.FlatTopNoOffsetNeighbours)
                {
                    Vector2Int neighbour = dir + ThisHex.HexLocation;
                    if (neighbour.x < 0 || neighbour.x > m_GameMap.GridSize.x || neighbour.y < 0 || neighbour.y > m_GameMap.GridSize.y) continue;
                    if (IsInClosed(neighbour)) continue;

                    GameObject PathBlock = Instantiate(Path, m_GameMap.GetPositionFromCoordinate(new Vector2Int(neighbour.x, neighbour.y)), Quaternion.identity);
                }
            }
        }


    }

    bool IsInClosed(Vector2Int pos)
    {
        foreach(PathMarker p in Closed)
        {
            if(p.HexLocation == pos) return true;
        }
        return false;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space)) BeginSearch();
    }
}
