using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct SideFaces
{
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
public class HexDrawer : MonoBehaviour
{
    private Mesh m_Mesh;
    private MeshFilter m_MeshFilter;
    private MeshRenderer m_MeshRenderer;
    private List<SideFaces> m_Faces;

    public Material Mat;
    public void SetMaterial(Material mat)
    {
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
        float AngleDeg = FlatTop ? 60 * Index: 60 * Index - 30;
        float AngleRad = Mathf.PI / 180f * AngleDeg;
        return new Vector3(Size * Mathf.Cos(AngleRad), Height, Size * Mathf.Sin(AngleRad));
    }

    private SideFaces CreateSideFaces(float InnerRad, float OuterRad, float HeightA, float HeightB, int Point, bool Reverse = false)
    {
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

    private void Awake()
    {
        m_MeshFilter = GetComponent<MeshFilter>();
        m_MeshRenderer = GetComponent<MeshRenderer>();

        m_Mesh = new Mesh();
        m_Mesh.name = "Hex";

        m_MeshFilter.mesh = m_Mesh;
        m_MeshRenderer.material = Mat;
    }

    private void OnEnable()
    {
        DrawMesh();
    }

    public void OnValidate()
    {
        if (Application.isPlaying)
        {
            DrawMesh();
        }
    }

    public void DrawMesh()
    {
        DrawSideFaces();
        CombineSideFaces();
    }

    private void DrawSideFaces()
    {
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
