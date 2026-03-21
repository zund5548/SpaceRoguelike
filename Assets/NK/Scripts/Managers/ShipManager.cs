using UnityEngine;
using UnityEngine.InputSystem;
using UniRx;
using UniRx.Triggers;
using System.Collections.Generic;
using Ships;
using TMPro;
using System;
using System.Collections;
using Maps;
using Managers;
using Items;
using UnityEngine.UI;
using Unity.VisualScripting;
namespace Managers
{
    public class ShipManager : MonoBehaviour
    {
        public static ShipManager Instance{get;private set;}
        private Camera _cam;

        private GameObject _shipObject;
        //艦隊の基準点と回転
        private Vector2 _currentFleetPos = Vector2.zero;
        private float _currentFleetDeg = 0f;
        //
        public float playerSpeed;
        public float turnRate;
        public List<ShipData> playerShipDataList = new List<ShipData>();
        //public List<ShipData> enemyShipDataList = new List<ShipData>();
        public List<GameObject> playerShipObjectList = new List<GameObject>();
        public List<GameObject> enemyShipObjectList = new List<GameObject>();
        public InputActionAsset inputAction;
        //public List<Item> itemList = new();
        [Header("Canvas")]
        public RectTransform DamageValueCanvas;
        public RectTransform PlayerShipBannerArea;
        [Header("UI")]
        public TextMeshProUGUI _StageNameText;
        [Header("Prefab")]
        public TextMeshProUGUI damageValueDisplay;
        public GameObject playerShipBanner;
        //input
        private InputAction clickAction;
        private InputAction positionAction;
        private Vector2 worldPos;
        private void OnEnable()
        {
            //clickAction.performed += OnClick;
            positionAction.performed += OnCursorMoved;
        }
        private void OnDisable()
        {
            //clickAction.performed -= OnClick;
            positionAction.performed -= OnCursorMoved;
        }
        // private void OnClick(InputAction.CallbackContext ctx)
        // {
        //     Vector2 screenPos = positionAction.ReadValue<Vector2>();
        //     worldPos = _cam.ScreenToWorldPoint(new Vector2(screenPos.x,screenPos.y));
        // }
        private void OnCursorMoved(InputAction.CallbackContext ctx)
        {
            Vector2 screenPos = positionAction.ReadValue<Vector2>();
            worldPos = _cam.ScreenToWorldPoint(new Vector2(screenPos.x,screenPos.y));
        }
        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            clickAction = inputAction["Click"];
            positionAction = inputAction["Position"];
        }
        void Start()
        {
            if(GManager.Instance.playerShipDataList.Count != 0)playerShipDataList = GManager.Instance.playerShipDataList;
            //if(GManager.Instance.enemyShipDataList.Count != 0)enemyShipDataList = GManager.Instance.enemyShipDataList;
            _shipObject = (GameObject)Resources.Load("Ship");
            _cam = Camera.main;
            gameObject.UpdateAsObservable()
                //.Where(_=>Input.GetMouseButton(0))
                .Subscribe(_ =>
                {
                    Vector2 destination = worldPos;
                    //Vector2 destination = _cam.ScreenToWorldPoint(Input.mousePosition);
                    if(Vector2.Distance(destination,_currentFleetPos) < 0.1f)return;
                    if(!clickAction.IsPressed())return;
                    var v = destination - _currentFleetPos;
                    _currentFleetDeg = Mathf.MoveTowardsAngle(_currentFleetDeg,Mathf.Atan2(v.y,v.x) * Mathf.Rad2Deg,turnRate * Time.deltaTime);
                    _currentFleetPos += new Vector2(Mathf.Cos(_currentFleetDeg * Mathf.Deg2Rad),Mathf.Sin(_currentFleetDeg * Mathf.Deg2Rad)) * playerSpeed * Time.deltaTime;
                    
                    //Debug.DrawRay(_currentFleetPos,new Vector2(Mathf.Cos(_currentFleetDeg * Mathf.Deg2Rad),Mathf.Sin(_currentFleetDeg * Mathf.Deg2Rad)));
                })
                .AddTo(gameObject);
            // foreach(var shipData in enemyShipDataList)
            // {
            //     InstantiateEnemyShip(shipData);
            // }  
            foreach(var shipData in playerShipDataList)
            {
                var shipObject = InstantiatePlayerShip(shipData);
            }   
            foreach(var item in GManager.Instance.itemList)
            {
                foreach(var itemEffect in  item.itemEffectList)
                {
                    itemEffect.OnApply();
                }
            }
            foreach(var shipObject in playerShipObjectList)
            {
                var ship = shipObject.GetComponent<Ship>();
                ship.SetCurrent();
                SetPlayerShipBanner(ship);
            }   
        }
        
        
        private GameObject InstantiatePlayerShip(ShipData shipData)
        {
            GameObject shipObject = Instantiate(_shipObject);
            Ship ship = shipObject.GetComponent<Ship>();
            float deg = (float)playerShipObjectList.Count/playerShipDataList.Count * 360f;
            //Debug.Log(playerShipObjectList.Count.ToString()+"/"+playerShipDataList.Count.ToString());
            //Debug.Log(playerShipObjectList.Count.ToString()+"/"+playerShipDataList.Count.ToString());
            var offset = 0.5f * new Vector2(Mathf.Cos(deg * Mathf.Deg2Rad),Mathf.Sin(deg  * Mathf.Deg2Rad));
            playerShipObjectList.Add(shipObject);

            shipObject.transform.position = _currentFleetPos;
            shipObject.transform.eulerAngles = new Vector3(0f,0f,_currentFleetDeg);
            shipObject.UpdateAsObservable()
                .Subscribe(_ =>
                {
                    //プレイヤーの船は止まらない
                    // if(!ship.isAbleToMove)
                    // {
                    //     Debug.Log("cant move");
                    //     return;
                    // }
                    shipObject.transform.position = _currentFleetPos + offset;
                    shipObject.transform.eulerAngles = new Vector3(0f,0f,_currentFleetDeg);
                })
                .AddTo(shipObject);

            
            ship.isPlayer = true;
            ship.tag = "PlayerShip";
            ship.shipData = shipData;
            ship.SetShipList(playerShipObjectList,enemyShipObjectList);
            //Shipをinstantiate → SetSPHPでStatのインスタンスをつくる →　Statにmodifier追加 →　Statを反映
            ship.SetStats();
            return shipObject;
        }   
        float separationDist = 1f;
        float separationStrength = 0.5f;
        private GameObject InstantiateEnemyShip(ShipData shipData)
        {
            GameObject shipObject = Instantiate(_shipObject);
            Ship ship = shipObject.GetComponent<Ship>();
            enemyShipObjectList.Add(shipObject);

            float randDeg = UnityEngine.Random.Range(-180f,180f);
            var pos = 10f * new Vector2(Mathf.Cos(randDeg  * Mathf.Deg2Rad),Mathf.Sin(randDeg  * Mathf.Deg2Rad));
            shipObject.transform.position = pos;
            shipObject.transform.eulerAngles = new Vector3(0f,0f,0f);
            shipObject.UpdateAsObservable()
            .Subscribe(_ =>
            {
                if(!ship.isAbleToMove)
                {
                    // Debug.Log("cant move");
                    return;
                }
                //var s = shipObject;
                if(Vector2.Distance(shipObject.transform.position,_currentFleetPos) < 0.1f)return;
                var v = _currentFleetPos - (Vector2)shipObject.transform.position;
                Vector2 moveDir = v.normalized;
                Vector2 separation = Vector2.zero;
                foreach (var other in ship.allyShipObjectList)
                {
                    if(other == shipObject)continue;
                    float dist = Vector2.Distance(shipObject.transform.position,other.transform.position);
                    if(dist < separationDist)
                    {
                        Vector2 away = shipObject.transform.position - other.transform.position;
                        separation += away.normalized/dist;
                    }
                }
                Vector2 finalDir = moveDir + separation * separationStrength;
                shipObject.transform.position = (Vector2)shipObject.transform.position + finalDir.normalized * shipData.speed * Time.deltaTime; 
                float deg = Mathf.MoveTowardsAngle(shipObject.transform.eulerAngles.z,Mathf.Atan2(finalDir.y,finalDir.x) * Mathf.Rad2Deg,180f*Time.deltaTime);
                shipObject.transform.eulerAngles = new Vector3(0f,0f,deg);
            })
            .AddTo(shipObject);

           
            ship.isPlayer = false;
            ship.tag = "EnemyShip";
            ship.shipData = shipData;
            ship.SetShipList(enemyShipObjectList,playerShipObjectList);
            
            //Shipをinstantiate → SetSPHPでStatのインスタンスをつくる →　Statにmodifier追加 →　Statを反映
            ship.SetStats();
            return shipObject;
        }   
        public void SetDamagevalue(int value,Vector2 worldPos,bool onShield)
        {
            var display = Instantiate(damageValueDisplay);
            //位置
            display.transform.SetParent(DamageValueCanvas,false);
            Vector2 screenPos = _cam.WorldToScreenPoint(worldPos);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(DamageValueCanvas,screenPos,_cam,out Vector2 localPos);
            float randDeg = UnityEngine.Random.Range(-180f,180f);
            display.rectTransform.anchoredPosition = localPos + 25f * new Vector2(Mathf.Cos(randDeg * Mathf.Deg2Rad),Mathf.Sin(randDeg * Mathf.Deg2Rad));
            //色と値
            var textMesh = display.GetComponent<TextMeshProUGUI>();
            textMesh.text = onShield?$"<color={"#1EE3DA"}>{value}</color>":value.ToString();
            display.UpdateAsObservable()
                .Delay(TimeSpan.FromSeconds(0.5f))
                .Subscribe(_ =>
                {
                    Destroy(display.gameObject);
                })
                .AddTo(display);
        }
        public void SetPlayerShipBanner(Ship ship)
        {
            var shipBanner = Instantiate(playerShipBanner);
            shipBanner.transform.SetParent(PlayerShipBannerArea,false);
            Slider shieldSlider = shipBanner.transform.GetChild(0).GetChild(0).GetComponent<Slider>();
            Slider hullSlider = shipBanner.transform.GetChild(0).GetChild(1).GetComponent<Slider>();
            shieldSlider.maxValue = ship.maxShieldPoint.Value;
            hullSlider.maxValue = ship.maxHullPoint.Value;
            TextMeshProUGUI shieldValueDisplay = shieldSlider.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI hullValueDisplay = hullSlider.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI StatDisplay = shipBanner.transform.GetChild(0).GetChild(2).GetComponent<TextMeshProUGUI>();
            shipBanner.UpdateAsObservable()
                .Subscribe(_=>
                {
                    shieldSlider.value = ship.currentShieldPoint;
                    shieldValueDisplay.text = shieldSlider.value.ToString() + " / " +  shieldSlider.maxValue.ToString();
                    hullSlider.value = ship.currentHullPoint;
                    hullValueDisplay.text = hullSlider.value.ToString() + " / " +  hullSlider.maxValue.ToString();
                    StatDisplay.text = "攻撃:" + ship.currentPower.Value.ToString();
                })
                .AddTo(ship);
                
            ship.OnDestroyAsObservable()
                .Subscribe(_ =>
                {
                    Destroy(shipBanner);
                });
        }

        public IEnumerator BattleEncountWave(List<EnemyWave> enemyWaveList,int limit)
        {
            _StageNameText.text = "ウェーブ : -/-";
            yield return BattleEncountWaveCoroutine(enemyWaveList,limit);
        }

        private IEnumerator BattleEncountWaveCoroutine(List<EnemyWave> enemyWaveList,int limit)
        {
            int currentWaveNum = 1;
            foreach(var enemyWave in enemyWaveList)
            {
                //Debug.Log(currentWaveNum);
                _StageNameText.text = "ウェーブ : "+currentWaveNum.ToString()+"/"+ enemyWaveList.Count.ToString();
                yield return WaveCoroutine(enemyWave,limit);
                yield return new WaitForSeconds(2f);
                currentWaveNum++;
            }
        }

        private IEnumerator WaveCoroutine(EnemyWave enemyWave,int limit)
        {
            foreach(var shipData in enemyWave.shipDataList)
            {
                var shipObject = InstantiateEnemyShip(shipData);
            }
            //ここで敵のアイテムをロード
            foreach(var shipObject in enemyShipObjectList)
            {
                var ship = shipObject.GetComponent<Ship>();
                ship.SetCurrent();
            }   
            yield return new WaitUntil(()=>enemyShipObjectList.Count == 0);
        }

        public void DeleteAllEnemy()
        {
            int n = enemyShipObjectList.Count;
            for(int i = 0;i < n;i++)
            {
                Destroy(enemyShipObjectList[i]);
            }
            enemyShipObjectList.Clear();
        }
        public void DeleteAllPlayer()
        {
            int n = playerShipObjectList.Count;
            for(int i = 0;i < n;i++)
            {
                Destroy(playerShipObjectList[i]);
            }
            playerShipObjectList.Clear();
        }
    }
}

