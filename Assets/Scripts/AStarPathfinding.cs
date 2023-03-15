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
    public int TotalHexesMoved;
    public GameObject Marker;
    public PathMarker Parent;

    public PathMarker(Vector2Int hexLocation, float g, float h, float f, int T, GameObject marker, PathMarker parent)
    {
        HexLocation = hexLocation;
        G = g;
        H = h;
        F = f;
        TotalHexesMoved = T;
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
    public PathMarker LastPos;
    public bool Done = false;
    public bool Incomplete = false;
    public bool Pathway = false;
    public Stack<GameObject> waypoint = new Stack<GameObject>();
    public Stack<Vector2Int> coords = new Stack<Vector2Int>();

    static void ShuffleList(List<Vector2Int> list)
    {
        // Get the count of elements in the list
        int count = list.Count;

        // Iterate through the list from the last index to the second index
        for (int i = count - 1; i > 0; i--)
        {
            // Generate a random index between 0 and i (inclusive)
            int j = UnityEngine.Random.Range(0, i + 1);

            // Swap the elements at indices i and j
            Vector2Int temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }
    }



    public void RemoveAllMarkers()
    {
        GameObject[] marker = GameObject.FindGameObjectsWithTag("Marker");
        foreach (GameObject m in marker) Destroy(m);
    }


    public void BeginSearch(Hex goal)
    {
        Done = false;
        RemoveAllMarkers();

        if (!GameManager.Main.PlayerArcher.Moved)
        {
            foreach (UnitBaseClass unit in GameManager.Main.UnitIntOrder)
            {
                if (unit.Pos == goal.Coords) { print("A unit is already at that location"); return; }
            }
        }
        
        Vector3 startHex = m_GameMap.GetPositionFromCoordinate(GameManager.Main.PlayerArcher.Pos);
        StartHex = new PathMarker(GameManager.Main.PlayerArcher.Pos, 0, 0, 0, 0, Instantiate(StartMarker, startHex, Quaternion.identity), null);

        //picks second top hex to be the end location.
        Vector3 goalHex = goal.transform.position;
        GoalHex = new PathMarker(goal.Coords, 0, 0, 0, 0, Instantiate(GoalMarker, goalHex, Quaternion.identity), null);

        Open.Clear();
        Closed.Clear();
        Closed.Add(StartHex);
        LastPos = StartHex;

    }


    public void PathFinding(PathMarker ThisHex, int MaxMovement)
    {
        if (ThisHex == null) return;
        if (ThisHex.HexLocation == GoalHex.HexLocation) { Done = true; print("End hit"); return; } // goal has been reached.
        //if (Open.Count == 0) { Debug.Log("End location out of movement range."); Incomplete = true; }

        if(!m_GameMap.FlatTop)
        {
            if(ThisHex.HexLocation.y % 2 == 0)
            {
                foreach (Vector2Int dir in m_GameMap.PointTopOffsetNeighbours)
                {
                    Vector2Int neighbour = dir + ThisHex.HexLocation;
                    if (neighbour.x < 0 || neighbour.x > m_GameMap.GridSize.x || neighbour.y < 0 || neighbour.y > m_GameMap.GridSize.y) continue;
                    int T = m_GameMap.GetMovementCost(neighbour) + ThisHex.TotalHexesMoved;
                    if (T > MaxMovement) continue;
                    if (IsInClosed(neighbour)) continue;

                    float G = Vector2.Distance(ThisHex.HexLocation, neighbour) + ThisHex.G;
                    float H = Vector2.Distance(neighbour, GoalHex.HexLocation);
                    float F = G + H;

                    GameObject PathBlock = Instantiate(Path, m_GameMap.GetPositionFromCoordinate(new Vector2Int(neighbour.x, neighbour.y)), Quaternion.identity);
                    PathBlock.GetComponent<Renderer>().material = OpenedMat;

                    //TextMesh[] Values = PathBlock.GetComponentsInChildren<TextMesh>();
                    //Values[0].text = "G: " + G.ToString("0.00");
                    //Values[1].text = "H: " + H.ToString("0.00");
                    //Values[2].text = "F: " + F.ToString("0.00");

                    if (!MarkerNeedUpdate(neighbour, G, H, F, ThisHex)) 
                        Open.Add(new PathMarker(neighbour, G, H, F, T, PathBlock, ThisHex));

                }
                
            }
            else
            {
                foreach (Vector2Int dir in m_GameMap.PointTopNoOffsetNeighbours)
                {
                    Vector2Int neighbour = dir + ThisHex.HexLocation;
                    if (neighbour.x < 0 || neighbour.x > m_GameMap.GridSize.x || neighbour.y < 0 || neighbour.y > m_GameMap.GridSize.y) continue;
                    int T = m_GameMap.GetMovementCost(neighbour) + ThisHex.TotalHexesMoved;
                    if (T > MaxMovement) continue;
                    if (IsInClosed(neighbour)) continue;

                    float G = Vector2.Distance(ThisHex.HexLocation, neighbour) + ThisHex.G;
                    float H = Vector2.Distance(neighbour, GoalHex.HexLocation);
                    float F = G + H;

                    GameObject PathBlock = Instantiate(Path, m_GameMap.GetPositionFromCoordinate(new Vector2Int(neighbour.x, neighbour.y)), Quaternion.identity);
                    PathBlock.GetComponent<Renderer>().material = OpenedMat;

                    //TextMesh[] Values = PathBlock.GetComponentsInChildren<TextMesh>();
                    //Values[0].text = "G: " + G.ToString("0.00");
                    //Values[1].text = "H: " + H.ToString("0.00");
                    //Values[2].text = "F: " + F.ToString("0.00");

                    if (!MarkerNeedUpdate(neighbour, G, H, F, ThisHex))
                        Open.Add(new PathMarker(neighbour, G, H, F, T, PathBlock, ThisHex));

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
                    int T = m_GameMap.GetMovementCost(neighbour) + ThisHex.TotalHexesMoved;
                    if (T > MaxMovement) continue;
                    if (IsInClosed(neighbour)) continue;

                    float G = Vector2.Distance(ThisHex.HexLocation, neighbour) + ThisHex.G;
                    float H = Vector2.Distance(neighbour, GoalHex.HexLocation);
                    float F = G + H;

                    GameObject PathBlock = Instantiate(Path, m_GameMap.GetPositionFromCoordinate(new Vector2Int(neighbour.x, neighbour.y)), Quaternion.identity);
                    PathBlock.GetComponent<Renderer>().material = OpenedMat;

                    //TextMesh[] Values = PathBlock.GetComponentsInChildren<TextMesh>();
                    //Values[0].text = "G: " + G.ToString("0.00");
                    //Values[1].text = "H: " + H.ToString("0.00");
                    //Values[2].text = "F: " + F.ToString("0.00");

                    if (!MarkerNeedUpdate(neighbour, G, H, F, ThisHex))
                        Open.Add(new PathMarker(neighbour, G, H, F, T, PathBlock, ThisHex));

                }
                
            }
            else
            {
                foreach (Vector2Int dir in m_GameMap.FlatTopNoOffsetNeighbours)
                {
                    Vector2Int neighbour = dir + ThisHex.HexLocation;
                    if (neighbour.x < 0 || neighbour.x > m_GameMap.GridSize.x || neighbour.y < 0 || neighbour.y > m_GameMap.GridSize.y) continue;
                    int T = m_GameMap.GetMovementCost(neighbour) + ThisHex.TotalHexesMoved;
                    if (T > MaxMovement) continue;
                    if (IsInClosed(neighbour)) continue;

                    float G = Vector2.Distance(ThisHex.HexLocation, neighbour) + ThisHex.G;
                    float H = Vector2.Distance(neighbour, GoalHex.HexLocation);
                    float F = G + H;

                    GameObject PathBlock = Instantiate(Path, m_GameMap.GetPositionFromCoordinate(new Vector2Int(neighbour.x, neighbour.y)), Quaternion.identity);
                    PathBlock.GetComponent<Renderer>().material = OpenedMat;

                    //TextMesh[] Values = PathBlock.GetComponentsInChildren<TextMesh>();
                    //Values[0].text = "G: " + G.ToString("0.00");
                    //Values[1].text = "H: " + H.ToString("0.00");
                    //Values[2].text = "F: " + F.ToString("0.00");

                    if (!MarkerNeedUpdate(neighbour, G, H, F, ThisHex))
                        Open.Add(new PathMarker(neighbour, G, H, F, T, PathBlock, ThisHex));

                }
                
            }
        }
        if (Open.Count == 0) 
        { 
            Debug.Log("End location out of movement range."); 
            Incomplete = true; 
            RemoveAllMarkers(); 
            return;  
        }
        Open = Open.OrderBy(p => p.F).ThenBy(n => n.H).ToList<PathMarker>();
        PathMarker pm = Open.ElementAt(0);
        Closed.Add(pm);
        Open.RemoveAt(0);
        pm.Marker.GetComponent<Renderer>().material = ClosedMat;
        LastPos = pm;

    }

    bool MarkerNeedUpdate(Vector2Int pos, float g, float h, float f, PathMarker par)
    {
        foreach (PathMarker p in Open)
        {
            if(p.HexLocation == pos)
            {
                p.G = g;
                p.H = h;
                p.F = f;
                p.Parent = par;
                return true;
            }
        }
        return false;
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

    public int GetPathway()
    {
        RemoveAllMarkers();
        PathMarker Marker = LastPos;
        waypoint.Clear();
        while (StartHex.HexLocation != Marker.HexLocation && Marker != null)
        {
            Instantiate(Path, m_GameMap.GetPositionFromCoordinate(new Vector2Int(Marker.HexLocation.x, Marker.HexLocation.y)), Quaternion.identity);
            
            //make stack of waypoints.
            waypoint.Push(m_GameMap.FindWaypoint(Marker.HexLocation));
            coords.Push(Marker.HexLocation);
            Marker = Marker.Parent;
            
            
        }
        
        Instantiate(Path, m_GameMap.GetPositionFromCoordinate(new Vector2Int(StartHex.HexLocation.x, StartHex.HexLocation.y)), Quaternion.identity);
        return LastPos.TotalHexesMoved;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
