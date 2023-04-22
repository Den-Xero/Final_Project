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
    public AIWarrior AiWarrior { get; private set; }

    public GameObject UnitNotMoved;
    float TimeAcive = 5f;
    float TimeToDisappear;
    public List<UnitBaseClass> UnitIntOrder = new List<UnitBaseClass>();

    bool End = false;
    public UnitBaseClass CurrentActiveUnit;
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
    }

    public void SetPlayerArcher(GameObject player)
    {
        PlayerArcher = player.GetComponent<PlayerArcher>();
        UnitIntOrder.Add(PlayerArcher);
    }

    public void SetAIWarrior(GameObject AI)
    {
        AiWarrior = AI.GetComponent<AIWarrior>();
        UnitIntOrder.Add(AiWarrior);
    }

    public void SetAIArcher(GameObject AI)
    {
        AiArcher = AI.GetComponent<AIArcher>();
        UnitIntOrder.Add(AiArcher);
    }

    public void MakeIntOrder()
    {
        if (UnitIntOrder.Count > 0)
        {
            UnitIntOrder = UnitIntOrder.OrderByDescending(p => p.Initiative).ThenBy(n => n.PlayerUnit ? 0 : 1).ToList<UnitBaseClass>();
            CurrentActiveUnit = UnitIntOrder[0];
            StarGame = true;
        }
        else
        {
            Debug.LogError("Why the fuck are u still empty");
        }
    }

    public TreeNodes.Status EndTurn()
    {
        if(!CurrentActiveUnit.Moved)
        {
            UnitNotMoved.SetActive(true);
            TimeToDisappear = Time.time + TimeAcive;
            return TreeNodes.Status.FAILURE;
        }
        End = true;
        CurrentActiveUnit.Moved = false;
        CurrentActiveUnit.Action = false;
        return TreeNodes.Status.SUCCESS;
    }

    public void ButtonEndTurn()
    {
        if (!CurrentActiveUnit.Moved)
        {
            UnitNotMoved.SetActive(true);
            TimeToDisappear = Time.time + TimeAcive;
            return;
        }
        End = true;
        CurrentActiveUnit.Moved = false;
        CurrentActiveUnit.Action = false;
    }

    public TreeNodes.Status SetAsMoved()
    {
        CurrentActiveUnit.Moved = true;
        if (CurrentActiveUnit.Moved) return TreeNodes.Status.SUCCESS;
        return TreeNodes.Status.FAILURE;
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
