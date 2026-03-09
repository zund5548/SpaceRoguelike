using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UniRx;
using Maps;
public class GManager : MonoBehaviour
{
    public static GManager Instance{get;private set;}
    public ReactiveProperty<GameState> CurrentState = new ReactiveProperty<GameState>();
    public bool isMapCreated = false;
    public List<List<StageNode>> _stageFloorList = new();
    public StageNode currentStageNode;
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        switch(SceneManager.GetActiveScene().name)
        {
            case "LobbyScene":
                CurrentState.Value = GameState.onLobby;
                break;
            case "MapScene":
                CurrentState.Value = GameState.onMap;
                break;
            case "StageScene":
                CurrentState.Value = GameState.onStage;
                break;
        }
    }
    [Serializable]
    public enum GameState
    {
         onLobby,
         onMap,
         onStage
    }
    public void SetGameState(GameState gameState)
    {
        CurrentState.Value = gameState;
    }
}
