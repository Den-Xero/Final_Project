using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PathMarker
{
    public Vector3 HexLocation;
    public float G;
    public float H;
    public float F;
    public GameObject Marker;
    public PathMarker Parent;

    public PathMarker(Vector3 hexLocation, float g, float h, float f, GameObject marker, PathMarker parent)
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

        List<Vector3> hex = new List<Vector3>();
        for (int y = 0; y < m_GameMap.GridSize.y; y++)
            for (int x = 0; x < m_GameMap.GridSize.x; x++)
            {
                hex.Add(new Vector3(x, y));
            }

        //Randomly pick a hex to have as start location.
        Vector3 startHex = new Vector3(0,0,0);
        StartHex = new PathMarker(startHex, 0, 0, 0, Instantiate(StartMarker, startHex, Quaternion.identity), null);

        //Randomly pick a hex to have as start location.
        Vector3 goalHex = new Vector3(0, 0, 0);
        GoalHex = new PathMarker(goalHex, 0, 0, 0, Instantiate(GoalMarker, goalHex, Quaternion.identity), null);


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
