using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UniRx;
using Maps;
using Ships;
using Items;
using TMPro;
using UnityEngine.InputSystem;

namespace Managers
{
    public class GManager : MonoBehaviour
    {
        public static GManager Instance{get;private set;}
        public ReactiveProperty<GameState> CurrentState = new ReactiveProperty<GameState>();
        public int _floorNum;
        [HideInInspector]
        public bool isMapCreated = false;
        public List<List<StageNode>> _stageFloorList = new();
        public StageNode currentStageNode;
        public List<StageNode> passsedStageNodes = new();
        public List<ShipData> playerShipDataList = new List<ShipData>();
        public List<ShipData> enemyShipDataList = new List<ShipData>(); 
        public List<Item> itemList = new();
        //public List<int> currentShipHullPoint = new();
        public int credit{get;private set;}
        private bool isInvOpen = false;
        //input
        public InputActionAsset inputAction;
        private InputAction inventoryAction;
        [Header("UI")]
        public TextMeshProUGUI _CreditDisplay;
        [Header("Canvas")]
        public GameObject InventoryCanvas;
        public Camera inventoryWorldCamera;
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(InventoryCanvas);
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
            DontDestroyOnLoad(InventoryCanvas);
            // inventoryAction = inputAction["Inventory"];
        }
        public  void ResetManager()
        {
            isMapCreated = false;
            credit = 0;
            _stageFloorList.Clear();
            currentStageNode = null;
            passsedStageNodes.Clear();
            passsedStageNodes.Clear();
            itemList.Clear();
            //currentShipHullPoint.Clear();
        }
        private void SetInventoryInput()
        {
            inventoryAction = inputAction["Inventory"];
            inventoryAction.performed += OnInventoryAccess;
        }
        // private void OnEnable(){inventoryAction.performed += OnInventoryAccess;}
        // private void OnDisable(){inventoryAction.performed -= OnInventoryAccess;}
        private void OnInventoryAccess(InputAction.CallbackContext ctx)
        {
            var inventory = InventoryCanvas.transform.GetChild(1).gameObject;
            var inventoryFade = InventoryCanvas.transform.GetChild(0).gameObject;
            if(CurrentState.Value == GameState.OnLobby)return;
            if(inventoryWorldCamera == null)
            {
                inventoryWorldCamera = Camera.main;
                InventoryCanvas.GetComponent<Canvas>().worldCamera = inventoryWorldCamera;
            }
            var animator = inventory.GetComponent<Animator>();
            if(!isInvOpen)
            {
                animator.SetTrigger("Open");
                inventoryFade.SetActive(true);
            }
            else
            {
                animator.SetTrigger("Close");
                inventoryFade.SetActive(false);
            }
            isInvOpen = !isInvOpen;
        }
        private void Start() 
        {
            credit = 0;
            AddCredit(1000);
            SetInventoryInput();
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
        public void AddCredit(int value)
        {
            credit += value;
        }
        private Dictionary<ShipData.ShipType,(int,int)> CreditTable = new Dictionary<ShipData.ShipType, (int, int)>
        {
            {ShipData.ShipType.Frigate,(25,30)},
            {ShipData.ShipType.EliteFrigate,(50,55)},
            {ShipData.ShipType.Destroyer,(150,155)},
            {ShipData.ShipType.EliteDestroyer,(200,205)},
        };
        public void AddCredit(ShipData.ShipType shipType)
        {
            var creditmm = CreditTable[shipType];
            //var addValue = UnityEngine.Random.Range(creditmm.Item1,creditmm.Item2); 
            credit += UnityEngine.Random.Range(creditmm.Item1,creditmm.Item2);
            SetCreditDisplay();
        }
        public void UseCredit(int value)
        {
            if(credit < value)
            {
                Debug.Log("not enough credit");
                return;
            }
            credit -= value;
            SetCreditDisplay();
        }
        public void SetCreditDisplay()
        {
            _CreditDisplay.SetText(credit.ToString() + "C");
        }
        // public void AddCurrentHullPoint(Ship ship)
        // {
        //     currentShipHullPoint.Add(ship.currentHullPoint);
        // }
        // public void SetCurrentHullPoints(List<Ship> playerShips)
        // {
        //     for(int i = 0;i < playerShips.Count;i++)
        //     {
        //         currentShipHullPoint[i] = playerShips[i].currentHullPoint;
        //     }
        // }
        // public void GetCurrentHullPoints(List<Ship> playerShips)
        // {
        //     for(int i = 0;i < playerShips.Count;i++)
        //     {
        //         playerShips[i].currentHullPoint = currentShipHullPoint[i];
        //     }
        // }
    }
}

