using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    public static GameManager Main { get; private set; }
    public GameMap GameBoard { get; private set; }
    public AStarPathfinding AStar { get; private set; }
    public UnitBaseClass CurrentActiveUnit { get; private set; }

    
    [SerializeField] GameObject UnitNotMoved;
    float TimeAcive = 5f;
    float TimeToDisappear;
    public List<UnitBaseClass> UnitIntOrder = new List<UnitBaseClass>();

    bool End = false;
    bool StarGame = false;
    int CurrentOrderNum = 0;

    [Header("UI")]
    [SerializeField] List<Image> PlayerAvatar;
    [SerializeField] List<Image> AIAvatar;
    [SerializeField] List<Image> TurnTracker;
    [SerializeField] List<Slider> PlayerHealthbars;
    [SerializeField] List<Slider> AIHealthbars;
    [SerializeField] List<Image> PlayerFills;
    [SerializeField] List<Image> AIFills;
    [SerializeField] Gradient HealthBarGradient;
    [SerializeField] List<TextMeshProUGUI> PlayerHealthBarText;
    [SerializeField] List<TextMeshProUGUI> AIHealthBarText;
    [SerializeField] List<TextMeshProUGUI> TurnIDText;
    [SerializeField] List<GameObject> TurnTrackerArrow;
    int CurrentActiveArrow = 0;
    int UnitsOutOfGame = 0;

    public void UISetUp()
    {
        //Set the UI up once all the objects have been spawned.
        int i = 0;
        foreach (UnitBaseClass unit in UnitIntOrder)
        {
            if(unit.PlayerUnit)
            {
                PlayerAvatar[unit.PlayerID - 1].sprite = unit.Avatar;
                PlayerHealthbars[unit.PlayerID - 1].maxValue = unit.MaxHealth;
                PlayerHealthbars[unit.PlayerID - 1].value = unit.MaxHealth;
                PlayerFills[unit.PlayerID - 1].color = HealthBarGradient.Evaluate(1f);
                PlayerHealthBarText[unit.PlayerID - 1].SetText(unit.MaxHealth.ToString());
                TurnIDText[i].SetText(unit.PlayerID.ToString());
                TurnIDText[i].color = Color.cyan;
            }
            else
            {
                AIAvatar[unit.AIID - 1].sprite = unit.Avatar;
                AIHealthbars[unit.AIID - 1].maxValue = unit.MaxHealth;
                AIHealthbars[unit.AIID - 1].value = unit.MaxHealth;
                AIFills[unit.AIID - 1].color = HealthBarGradient.Evaluate(1f);
                AIHealthBarText[unit.AIID - 1].SetText(unit.MaxHealth.ToString());
                TurnIDText[i].SetText(unit.AIID.ToString());
                TurnIDText[i].color = Color.magenta;
            }
            TurnTracker[i].sprite = unit.Avatar;
            i++;
        }
        TurnTrackerArrow[0].SetActive(true);
    }

    public void TurnTrackerUpdate()
    {
        //Updates the turn tracker when a unit is taken out of the game.
        int i = 0;
        TurnTrackerArrow[CurrentActiveArrow].SetActive(false);
        foreach (UnitBaseClass unit in UnitIntOrder)
        {
            if (unit.PlayerUnit)
            {
                TurnIDText[i].SetText(unit.PlayerID.ToString());
                TurnIDText[i].color = Color.cyan;
            }
            else
            {
                TurnIDText[i].SetText(unit.AIID.ToString());
                TurnIDText[i].color = Color.magenta;
            }
            TurnTracker[i].sprite = unit.Avatar;
            if(unit == CurrentActiveUnit)
            {
                TurnTrackerArrow[i].SetActive(true);
                CurrentActiveArrow = i;
            }
            i++;
        }
        UnitsOutOfGame++;
        TurnTracker[TurnTracker.Count - UnitsOutOfGame].gameObject.SetActive(false);
    }

    public void SetHealthSlider(int ID, int Health, bool Player)
    {
        //Updates the UI health bars to show what the units healths are at that moment.
        if(Player)
        {
            PlayerHealthbars[ID - 1].value = Health;
            PlayerFills[ID - 1].color = HealthBarGradient.Evaluate(PlayerHealthbars[ID - 1].normalizedValue);
            PlayerHealthBarText[ID - 1].SetText(Health.ToString());
        }
        else
        {
            AIHealthbars[ID - 1].value = Health;
            AIFills[ID - 1].color = HealthBarGradient.Evaluate(AIHealthbars[ID - 1].normalizedValue);
            AIHealthBarText[ID - 1].SetText(Health.ToString());
        }
    }

    private void Awake()
    {
        //Makes it so only on of this script type can be in the level at any moment and gets some veribles stored.
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


    public void UnitSetUp(UnitBaseClass unit)
    {
        //Added units to the turn order list.
        UnitIntOrder.Add(unit);
    }

    public void MakeIntOrder()
    {
        //Makes the turn order list put the units in it in the right order.
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
        //Ends the AI turn.
        End = true;
        CurrentActiveUnit.Moved = false;
        CurrentActiveUnit.Action = false;
        TurnTrackerArrow[CurrentActiveArrow].SetActive(false);
        if (CurrentActiveArrow == TurnTrackerArrow.Count - 1)
        {
            CurrentActiveArrow = 0;
        }
        else
        {
            CurrentActiveArrow++;
        }
        TurnTrackerArrow[CurrentActiveArrow].SetActive(true);
        return TreeNodes.Status.SUCCESS;
    }

    public void ButtonEndTurn()
    {
        //Ends the player unit turn when button is clicked.
        if(!CurrentActiveUnit.PlayerUnit) return;
        if (!CurrentActiveUnit.Moved)
        {
            UnitNotMoved.SetActive(true);
            TimeToDisappear = Time.time + TimeAcive;
            return;
        }
        End = true;
        CurrentActiveUnit.Moved = false;
        CurrentActiveUnit.Action = false;
        TurnTrackerArrow[CurrentActiveArrow].SetActive(false);
        if(CurrentActiveArrow == TurnTrackerArrow.Count - 1)
        {
            CurrentActiveArrow = 0;
        }
        else
        {
            CurrentActiveArrow++;
        }
        TurnTrackerArrow[CurrentActiveArrow].SetActive(true);
    }

    public void ButtonSetAsMoved()
    {
        //Makes it so the player unit acts as if it has moved when button is clicked.
        if (!CurrentActiveUnit.PlayerUnit) return;
        CurrentActiveUnit.Moved = true;
    }

    public TreeNodes.Status SetAsMoved()
    {
        //Makes it so the AI unit acts as if it has moved.
        CurrentActiveUnit.Moved = true;
        if (CurrentActiveUnit.Moved) return TreeNodes.Status.SUCCESS;
        return TreeNodes.Status.FAILURE;
    }


    // Update is called once per frame
    void Update()
    {
        //Only runs when all objects are in the level.
        if(!StarGame) return;
        //Makes text disapear after set time.
        if(UnitNotMoved.active && Time.time >= TimeToDisappear) UnitNotMoved.SetActive(false);
        //Checks if game is over and if it is a player or AI win.
        int playerUnitsAlive = 0;
        int aiUnitsAlive = 0;
        foreach (UnitBaseClass unit in UnitIntOrder)
        {
            if (unit.PlayerUnit)
            {
                playerUnitsAlive++;
            }
            else
            {
                aiUnitsAlive++;
            }
        }
        if (playerUnitsAlive == 0)
        {
            SceneManager.LoadScene("LossScene");
        }
        else if (aiUnitsAlive == 0)
        {
            SceneManager.LoadScene("WinScene");
        }
        //Ends turn and moves on to the next unit in the turn order.
        if (End)
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
        //Runs the current unit update loop.
        UnitIntOrder[CurrentOrderNum].UpdateLoop();

    }
}
