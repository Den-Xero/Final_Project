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
        UnitIntOrder.Add(unit);
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
        if (!CurrentActiveUnit.PlayerUnit) return;
        CurrentActiveUnit.Moved = true;
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
        UnitIntOrder[CurrentOrderNum].UpdateLoop();

    }
}
