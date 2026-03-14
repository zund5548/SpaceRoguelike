using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UniRx;
using Maps;
using Ships;
using Items;
public class GManager : MonoBehaviour
{
    public static GManager Instance{get;private set;}
    public ReactiveProperty<GameState> CurrentState = new ReactiveProperty<GameState>();
    public bool isMapCreated = false;
    public List<List<StageNode>> _stageFloorList = new();
    public StageNode currentStageNode;
    public List<StageNode> passsedStageNodes = new();
    public List<ShipData> playerShipDataList = new List<ShipData>();
    public List<ShipData> enemyShipDataList = new List<ShipData>(); 
    public List<Item> itemList = new();
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
                CurrentState.Value = GameState.OnLobby;
                break;
            case "MapScene":
                CurrentState.Value = GameState.OnMap;
                break;
            case "StageScene":
                CurrentState.Value = GameState.OnStage;
                break;
        }
    }
    [Serializable]
    public enum GameState
    {
         OnLobby,
         OnMap,
         OnStage
    }
    public void SetGameState(GameState gameState)
    {
        CurrentState.Value = gameState;
    }
}
