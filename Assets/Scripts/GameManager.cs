using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _gameManager;
    public static GameManager Instance { get { return _gameManager; } }

    public List<IManager> subscribedList = new List<IManager>();
    private List<IManager> unsubscribeList = new List<IManager>();

    private AIManager _AIManager;
    private PathfindingManager _PFManager;

    public List<Leader> leaders = new List<Leader>();

    public List<Unit> team1 = new List<Unit>();
    public List<Unit> team2 = new List<Unit>();

    public bool team1Active = true;
    public int soldiersTeam1 = 5;

    public bool team2Active = true;
    public int soldiersTeam2 = 5;

    public float updateInterval = 0.25f;
    public float updateFixedInterval = 0.1f;

    public bool paused = false;

    public float soldierFleeChance = 100f;

    public Material team1Color;
    public Material team2Color;
    public Material teamGrey;

    private void Awake()
    {
        if (_gameManager == null) _gameManager = this;
        else Destroy(this.gameObject);

        foreach(Unit u in team1)
        {
            foreach(GameObject go in u.helmetAndFlag)
            {
                go.GetComponent<MeshRenderer>().material = team1Color;
            }
        }

        foreach (Unit u in team2)
        {
            foreach (GameObject go in u.helmetAndFlag)
            {
                go.GetComponent<MeshRenderer>().material = team2Color;
            }
        }
    }

    private void Start()
    {
        _AIManager = AIManager.Instance;
        _PFManager = PathfindingManager.Instance;

        SetTeamN(team1, 1);
        SetTeamN(team2, 2);

        if (team1Active)
        {
            team1[0].gameObject.SetActive(true);
            for (int i = 1; i < soldiersTeam1 + 1 && i < team1.Count; i++)
            {
                team1[i].gameObject.SetActive(true);
            }
        }

        if (team2Active)
        {
            team2[0].gameObject.SetActive(true);
            for (int i = 1; i < soldiersTeam2 + 1 && i < team2.Count; i++)
            {
                team2[i].gameObject.SetActive(true);
            }
        }

        GiveTargets();
    }

    private void Update()
    {
        if (unsubscribeList.Count > 0)
        {
            foreach (var item in unsubscribeList)
            {
                foreach (var item2 in subscribedList)
                {
                    if (item == item2)
                    {
                        subscribedList.Remove(item2);
                        unsubscribeList.Remove(item);
                    }
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            paused = !paused;
            foreach (IManager item in subscribedList)
            {
                item.Pause(paused);
            }
        }
    }

    public void SetTeamN(List<Unit> team, int n)
    {
        foreach (var item in team)
        {
            item.team = n;
        }
    }

    public void AddItem(IManager item)
    {
        subscribedList.Add(item);
    }

    public void RemoveItem(IManager item)
    {
        unsubscribeList.Remove(item);
    }

    #region BatallionManagement

    public void GiveTargets()
    {
        foreach(Leader l in leaders)
        {
            GetTarget(l);
        }
    }

    public void GetTarget(Leader l)
    {
        if (leaders.Count < 2) return;
        int r = -1;
        while (r == -1 || r == leaders.IndexOf(l))
        {
            r = Random.Range(0, leaders.Count);
        }
        l.targetLeader = leaders[r];
    }

    #endregion
}
