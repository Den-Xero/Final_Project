using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct SideFaces
{
    //The Hex faces struct for drawing.
    public List<Vector3> Vertices { get; private set; }
    public List<int> Triangles { get; private set; }
    public List<Vector2> UVS { get; private set; }

    public SideFaces(List<Vector3> Vertices, List<int> Triangles, List<Vector2> UVS)
    {
        this.Vertices = Vertices;
        this.Triangles = Triangles;
        this.UVS = UVS;
    }
}





[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class Hex : MonoBehaviour
{
    Mesh m_Mesh;
    MeshFilter m_MeshFilter;
    MeshRenderer m_MeshRenderer;
    List<SideFaces> m_Faces;
    public Vector2Int Coords;
    public int MovementCost;
    public string Effect;
    /* Effect list:
        Cobblestone, Dirt, Grassland = no effect.
        Sand = Range units have 2 less range, melee units take more damage or get stunned.
        Water = Unit can not attack.
        Mountain = Range units have 2 more range, melee units have a 25% change to ignore damage.
     */

    public Material Mat;
    public void SetMesh(Material mat)
    {
        //Sets the Mesh that the Hex that is being created will use.
        Mat = mat;
        m_MeshFilter = GetComponent<MeshFilter>();
        m_MeshRenderer = GetComponent<MeshRenderer>();

        m_Mesh = new Mesh();
        m_Mesh.name = "Hex";

        m_MeshFilter.mesh = m_Mesh;
        m_MeshRenderer.material = Mat;
    }
    public float InnerSize;
    public float OuterSize;
    public float Height;
    public bool FlatTop;

    protected Vector3 GetPoint(float Size, float Height, int Index)
    {
        //Small function to get the points for a hex face with the inputed numbers.
        float AngleDeg = FlatTop ? 60 * Index: 60 * Index - 30;
        float AngleRad = Mathf.PI / 180f * AngleDeg;
        return new Vector3(Size * Mathf.Cos(AngleRad), Height, Size * Mathf.Sin(AngleRad));
    }

    private SideFaces CreateSideFaces(float InnerRad, float OuterRad, float HeightA, float HeightB, int Point, bool Reverse = false)
    {
        //automated function to get the vertices, triangles and uvs for making the hex faces using inputed values.
        Vector3 PointA = GetPoint(InnerRad, HeightB, Point);
        Vector3 PointB = GetPoint(InnerRad, HeightB, (Point < 5) ? Point + 1 : 0);
        Vector3 PointC = GetPoint(OuterRad, HeightA, (Point < 5) ? Point + 1 : 0);
        Vector3 PointD = GetPoint(OuterRad, HeightA, Point);

        List<Vector3> Vertices = new List<Vector3>() { PointA, PointB, PointC, PointD };
        List<int> Triangles = new List<int>() { 0, 1, 2, 2, 3, 0 };
        List<Vector2> UVS = new List<Vector2>() { new Vector2(0,0), new Vector2(1,0), new Vector2(1,1), new Vector2(0,1) };
        if(Reverse)
        {
            Vertices.Reverse();
        }

        return new SideFaces(Vertices, Triangles, UVS);
    }

    public void DrawMesh()
    {
        //gets the faces then combines and draws them.
        DrawSideFaces();
        CombineSideFaces();
    }

    private void DrawSideFaces()
    {
        //Addes the faces to a list.
        m_Faces = new List<SideFaces>();

        //Top face
        for (int Point = 0; Point < 6; Point++)
        {
            m_Faces.Add(CreateSideFaces(InnerSize, OuterSize, Height / 2f, Height / 2f, Point));
        }

        //Bottom face
        for (int Point = 0; Point < 6; Point++)
        {
            m_Faces.Add(CreateSideFaces(InnerSize, OuterSize, -Height / 2f, -Height / 2f, Point, true));
        }

        //Outer face
        for (int Point = 0; Point < 6; Point++)
        {
            m_Faces.Add(CreateSideFaces(OuterSize, OuterSize, Height / 2f, -Height / 2f, Point, true));
        }

        //Inner face
        for (int Point = 0; Point < 6; Point++)
        {
            m_Faces.Add(CreateSideFaces(InnerSize, InnerSize, Height / 2f, -Height / 2f, Point));
        }
    }

    private void CombineSideFaces()
    {
        //Goes through the list of faces to combine and draw them.
        List<Vector3> Vertices = new List<Vector3>();
        List<int> Tri = new List<int>();
        List<Vector2> UVS = new List<Vector2>();

        for(int i = 0; i < m_Faces.Count; i++)
        {
            //Adding the vertices and uvs
            Vertices.AddRange(m_Faces[i].Vertices);
            UVS.AddRange(m_Faces[i].UVS);

            //Add offset to triangles
            int Offset = (4 * i);
            foreach (int Triangles in m_Faces[i].Triangles)
            {
                Tri.Add(Triangles + Offset);
            }
        }

        m_Mesh.vertices = Vertices.ToArray();
        m_Mesh.uv = UVS.ToArray();
        m_Mesh.triangles = Tri.ToArray();
        m_Mesh.RecalculateNormals();
    }

}
