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
    bool Pathway = false;


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

        //Suffles list then picks top hex to be the start location.
        ShuffleList(hex);
        Vector3 startHex = m_GameMap.GetPositionFromCoordinate(new Vector2Int(hex[0].x, hex[0].y));
        StartHex = new PathMarker(new Vector2Int(hex[0].x, hex[0].y), 0, 0, 0, Instantiate(StartMarker, startHex, Quaternion.identity), null);

        //picks second top hex to be the end location.
        Vector3 goalHex = m_GameMap.GetPositionFromCoordinate(new Vector2Int(hex[1].x, hex[1].y));
        GoalHex = new PathMarker(new Vector2Int(hex[1].x, hex[1].y), 0, 0, 0, Instantiate(GoalMarker, goalHex, Quaternion.identity), null);


        Open.Clear();
        Closed.Clear();
        Open.Add(StartHex);
        LastPos = StartHex;

    }


    void PathFinding(PathMarker ThisHex)
    {
        if (ThisHex == null) return;
        if (ThisHex.HexLocation == GoalHex.HexLocation) { Done = true; return; } // goal has been reached.

        if(!m_GameMap.FlatTop)
        {
            if(ThisHex.HexLocation.y % 2 == 0)
            {
                foreach (Vector2Int dir in m_GameMap.PointTopOffsetNeighbours)
                {
                    Vector2Int neighbour = dir + ThisHex.HexLocation;
                    if(neighbour.x < 0 || neighbour.x > m_GameMap.GridSize.x || neighbour.y < 0 || neighbour.y > m_GameMap.GridSize.y) continue;
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
                        Open.Add(new PathMarker(neighbour, G, H, F, PathBlock, ThisHex));

                }
                
            }
            else
            {
                foreach (Vector2Int dir in m_GameMap.PointTopNoOffsetNeighbours)
                {
                    Vector2Int neighbour = dir + ThisHex.HexLocation;
                    if (neighbour.x < 0 || neighbour.x > m_GameMap.GridSize.x || neighbour.y < 0 || neighbour.y > m_GameMap.GridSize.y) continue;
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
                        Open.Add(new PathMarker(neighbour, G, H, F, PathBlock, ThisHex));

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
                        Open.Add(new PathMarker(neighbour, G, H, F, PathBlock, ThisHex));

                }
                
            }
            else
            {
                foreach (Vector2Int dir in m_GameMap.FlatTopNoOffsetNeighbours)
                {
                    Vector2Int neighbour = dir + ThisHex.HexLocation;
                    if (neighbour.x < 0 || neighbour.x > m_GameMap.GridSize.x || neighbour.y < 0 || neighbour.y > m_GameMap.GridSize.y) continue;
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
                        Open.Add(new PathMarker(neighbour, G, H, F, PathBlock, ThisHex));

                }
                
            }
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

    void GetPathway()
    {
        RemoveAllMarkers();
        PathMarker Marker = LastPos;

        while (StartHex.HexLocation != Marker.HexLocation && Marker != null)
        {
            Instantiate(Path, m_GameMap.GetPositionFromCoordinate(new Vector2Int(Marker.HexLocation.x, Marker.HexLocation.y)), Quaternion.identity);
            Marker = Marker.Parent;
        }

        Instantiate(Path, m_GameMap.GetPositionFromCoordinate(new Vector2Int(StartHex.HexLocation.x, StartHex.HexLocation.y)), Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A)) { BeginSearch(); Pathway = false; }
        if(Input.GetKeyDown(KeyCode.Space) && !Done) PathFinding(LastPos);
        if(Done && !Pathway) {GetPathway(); Pathway = true; }
        
    }
}
