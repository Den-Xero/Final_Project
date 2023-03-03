using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public static GameManager Main { get; private set; }
    public GameMap GameBoard { get; private set; }
    public AStarPathfinding AStar { get; private set; }
    public GameObject Player { get; private set; }

    public Vector2Int PlayerCoords;


    private void Awake()
    {
        if(Main != null && Main != this)
        {
            Destroy(this);
            return;
        }
        DontDestroyOnLoad(this);
        Main = this;
        GameBoard = GetComponent<GameMap>();
        AStar = GetComponent<AStarPathfinding>();
    }


    // Start is called before the first frame update
    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
