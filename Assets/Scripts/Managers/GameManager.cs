using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [HideInInspector]
    public CharacterBaseStats playerBaseStats;

    private List<IEndGameObserver> endGameObservers = new List<IEndGameObserver>();
    protected override void Awake()
    {
        base.Awake();
        //DontDestroyOnLoad();
    }
    public void RigisterPlayer(CharacterBaseStats play)
    {
        playerBaseStats = play;
    }
    public void AddObserver(IEndGameObserver observer)
    {
        endGameObservers.Add(observer);
    }
    public void RemoveObserver(IEndGameObserver observer)
    {
        endGameObservers.Remove(observer);
    }
    public void NotifyObservers()
    {
        foreach(IEndGameObserver observer in endGameObservers)
        {
            observer.EndNotify();
        }
    }
}
