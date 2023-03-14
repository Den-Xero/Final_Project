using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    public static GameManager Main { get; private set; }
    public GameMap GameBoard { get; private set; }
    public AStarPathfinding AStar { get; private set; }
    public PlayerArcher PlayerArcher { get; private set; }
    public AIArcher AiArcher { get; private set; }

    public GameObject UnitNotMoved;
    float TimeAcive = 5f;
    float TimeToDisappear;

    List<UnitBaseClass> UnitIntOrder = new List<UnitBaseClass>();
    bool End = false;
    UnitBaseClass CurrentActiveUnit;
    bool StarGame = false;
    int CurrentOrderNum = 0;

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
        AiArcher = GetComponent<AIArcher>();
        UnitIntOrder.Add(AiArcher);
    }

    public void SetPlayerArcher(GameObject player)
    {
        PlayerArcher = player.GetComponent<PlayerArcher>();
        UnitIntOrder.Add(PlayerArcher);
    }

    public void MakeIntOrder()
    {
        if (UnitIntOrder.Count > 0)
        {
            UnitIntOrder = UnitIntOrder.OrderByDescending(p => p.Initiative).ThenBy(n => n.PlayerUnit ? 1 : 0).ToList<UnitBaseClass>();
            CurrentActiveUnit = UnitIntOrder[0];
            StarGame = true;
        }
        else
        {
            Debug.LogError("Why the fuck are u still empty");
        }
    }

    public void EndTurn()
    {
        if(!CurrentActiveUnit.Moved)
        {
            UnitNotMoved.SetActive(true);
            TimeToDisappear = Time.time + TimeAcive;
            return;
        }
        End = true;
        CurrentActiveUnit.Moved = false;
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(!StarGame) return;
        if(UnitNotMoved.active && Time.time >= TimeToDisappear) UnitNotMoved.SetActive(false);
        if(End)
        {
            if (UnitIntOrder.Count <= CurrentOrderNum + 1)
            {
                CurrentOrderNum = 0;
                CurrentActiveUnit = UnitIntOrder[CurrentOrderNum];
                End = false;
            }
            else
            {
                CurrentOrderNum++;
                CurrentActiveUnit = UnitIntOrder[CurrentOrderNum];
                End = false;
            }
        }
        UnitIntOrder[CurrentOrderNum].UpdateLoop();
    }
}
