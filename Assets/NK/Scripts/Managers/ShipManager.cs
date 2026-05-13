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
using UnityEngine.UI;
using Stats;
namespace Managers
{
    public class ShipManager : MonoBehaviour
    {
        public static ShipManager Instance{get;private set;}
        private Camera _cam;

        
        //艦隊の基準点と回転
        public Vector2 _currentFleetPos = Vector2.zero;
        public float _currentFleetDeg = 0f;
        //
        public float initPlayerSpeed;
        public Stat _playerSpeed;
        public float turnRate;
        public List<ShipData> playerShipDataList = new List<ShipData>();
        //public List<ShipData> enemyShipDataList = new List<ShipData>();
        public List<GameObject> playerShipObjectList = new List<GameObject>();
        public List<GameObject> enemyShipObjectList = new List<GameObject>();
        public List<Ship> playerShipList = new List<Ship>();
        public List<Ship> enemyShipList = new List<Ship>();
        private AnchorObject _currentAnchorObject;
        //public List<Item> itemList = new();
        [Header("Canvas")]
        public RectTransform DamageValueCanvas;
        public RectTransform PlayerShipBannerArea;
        [Header("UI")]
        public TextMeshProUGUI _StageNameText;
        [Header("Prefab")]
        public TextMeshProUGUI damageValueDisplay;
        public GameObject playerShipBanner;
        private GameObject _ShipObject;
        private GameObject _BossObject;
        private GameObject _WarpEffectObject;
        private GameObject _AnchorObject;
        //input
        public InputActionAsset inputAction;
        private InputAction clickAction;
        private InputAction positionAction;
        private Vector2 worldPos;
        private void OnEnable()
        {
            positionAction.performed += OnCursorMoved;
        }
        private void OnDisable()
        {
            positionAction.performed -= OnCursorMoved;
        }
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
            _ShipObject = (GameObject)Resources.Load("Ship");
            _BossObject = (GameObject)Resources.Load("BossShip");
            _WarpEffectObject = (GameObject)Resources.Load("WarpEffect");
            _AnchorObject = (GameObject)Resources.Load("AnchorObject");
            _cam = Camera.main;
            _playerSpeed = new(initPlayerSpeed);
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
                    _currentFleetPos += new Vector2(Mathf.Cos(_currentFleetDeg * Mathf.Deg2Rad),Mathf.Sin(_currentFleetDeg * Mathf.Deg2Rad)) * _playerSpeed.Value * Time.deltaTime;
                    
                    //Debug.DrawRay(_currentFleetPos,new Vector2(Mathf.Cos(_currentFleetDeg * Mathf.Deg2Rad),Mathf.Sin(_currentFleetDeg * Mathf.Deg2Rad)));
                })
                .AddTo(gameObject);
            gameObject.UpdateAsObservable()
                .Where(_=>playerShipObjectList.Count == 0)
                .Subscribe(_ =>
                {
                    EventManager.Instance.PublishFail();
                })
                .AddTo(gameObject);
            // foreach(var shipData in enemyShipDataList)
            // {
            //     InstantiateEnemyShip(shipData);
            // }  
            foreach(var shipData in playerShipDataList)
            {
                SpawnPlayerShip(shipData);
            }   
            foreach(var ship in playerShipList)
            {
                ship.SetCurrentUniqueStat();
            }   
            foreach(var item in GManager.Instance.itemList)
            {
                if(item.itemEffectList == null)continue;
                foreach(var itemEffect in item.itemEffectList)
                {
                    if(itemEffect)itemEffect.OnApply();
                }
                
            }
            foreach(var ship in playerShipList)
            {
                ship.SetCurrentSPHP();
                ship.SetCurrentWeapon();
                SetPlayerShipBanner(ship);
            }   
        }
        
        
        private GameObject SpawnPlayerShip(ShipData shipData)
        {
            GameObject shipObject = Instantiate(_ShipObject);
            playerShipObjectList.Add(shipObject);
            Ship ship = shipObject.GetComponent<Ship>();
            playerShipList.Add(ship);
            float deg = (float)playerShipObjectList.Count/playerShipDataList.Count * 360f;
            var offset = 0.5f * new Vector2(Mathf.Cos(deg * Mathf.Deg2Rad),Mathf.Sin(deg  * Mathf.Deg2Rad));
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
            //ship.shipData.weaponData.SetUniqueStat(ship);
            return shipObject;
        }   
        
        float separationDist = 1f;//敵艦同士の間隔
        float separationStrength = 0.5f;//敵艦同士の間隔以下に近づいたときにこの速度で遠ざける
        private GameObject SpawnEnemyShip(ShipData shipData,Vector2 pos)
        {
            GameObject shipObject = Instantiate(_ShipObject);
            Ship ship = shipObject.GetComponent<Ship>();
            enemyShipObjectList.Add(shipObject);
            float deg = Mathf.Atan2(-pos.y,-pos.x) * Mathf.Rad2Deg;
            shipObject.transform.position = pos;
            shipObject.transform.eulerAngles = new Vector3(0f,0f,deg);
            shipObject.UpdateAsObservable()
            .Subscribe(_ =>
            {
                //Debug.Log(ship.isSurged);
                if(ship.isSurged)return;
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
            SetEnemyStatBuff(ship);
            ship.SetCurrentSPHP();
            ship.SetCurrentUniqueStat();
            ship.SetCurrentWeapon();
            //ship.shipData.weaponData.SetUniqueStat(ship);
            return shipObject;
        }   

        public GameObject SpawnAnchor(ShipData shipData)
        {
            GameObject anchorObject = Instantiate(_AnchorObject);
            AnchorObject anchor = anchorObject.GetComponent<AnchorObject>();
            playerShipObjectList.Add(anchorObject);
            // anchorObject.UpdateAsObservable()
            //     .Subscribe(_ =>
            //     {
            //         if(anchor.isSurged)return;
            //     })
            //     .AddTo(anchorObject);
            anchor.isPlayer = true;
            anchor.tag = "PlayerAnchor";
            anchor.shipData = shipData;
            anchor.SetShipList(playerShipObjectList,enemyShipObjectList);
            anchor.SetStats();
            _currentAnchorObject = anchor;
            return anchorObject;
        }

        public AnchorObject GetAnchorObject()
        {
            if(_currentAnchorObject)return _currentAnchorObject;
            else return null;
        }

        private IEnumerator SpawnWithDelay(ShipData shipData,int limit)
        {
            if(enemyShipObjectList.Count >= limit)yield return new WaitUntil(()=>enemyShipObjectList.Count < limit);
            float randDeg = UnityEngine.Random.Range(-180f,180f);
            var pos = 10f * new Vector2(Mathf.Cos(randDeg  * Mathf.Deg2Rad),Mathf.Sin(randDeg  * Mathf.Deg2Rad));
            var warpEffect = Instantiate(_WarpEffectObject,pos,Quaternion.identity);
            Observable.Timer(TimeSpan.FromSeconds(1f))
                .Subscribe(_ =>
                {
                    Destroy(warpEffect);
                })
                .AddTo(warpEffect);
            yield return new WaitForSeconds(0.5f);
            SpawnEnemyShip(shipData,pos);
            yield return null;
        }
        
        /// <summary>ステージを経るにつれて敵艦を強化</summary>
        public void SetEnemyStatBuff(Ship enemyShip)
        {
            int k = GManager.Instance.currentStageNode.floorStageNum / 5;
            if(k == 0)return;
            enemyShip.GetStat(StatType.Hull).AddModifier(new StatModifier(k * 10f,ModType.Percent));
            enemyShip.GetStat(StatType.Shield).AddModifier(new StatModifier(k * 50f,ModType.Percent));
            enemyShip.GetStat(StatType.Power).AddModifier(new StatModifier(k * 50f,ModType.Percent));
        }
        public GameObject SpawnBossShip(ShipData shipData,BossType bossType)
        {
            GameObject bossShipObject = Instantiate(_BossObject);
            BossShip bossShip = bossShipObject.GetComponent<BossShip>();
            enemyShipObjectList.Add(bossShipObject);
            bossShipObject.transform.position = new Vector2(5f,0f);

            bossShip.isPlayer = false;
            bossShip.tag = "EnemyShip";
            bossShip.shipData = shipData;
            bossShip.SetShipList(enemyShipObjectList,playerShipObjectList);
            //Shipをinstantiate → SetSPHPでStatのインスタンスをつくる →　Statにmodifier追加 →　Statを反映
            bossShip.SetStats();
            //
            bossShip.bossType = bossType;
            bossShip.SetCurrentSPHP();
            bossShip.SetCurrentWeapon();
            StartCoroutine(bossShip.bossType.BossAttack(bossShip));
            bossType.SetMove(bossShip);
            return bossShipObject;
        }

        public void SetDamagevalue(int value,Vector2 worldPos,bool onShield,bool isCrit)
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
            string txt = onShield?$"<color={"#1EE3DA"}>{value}</color>":value.ToString();
            //isCritが真:値の表記に!をつける
            //onShieldが真:値の色をtarquoiseにする
            textMesh.text = isCrit ? txt + (onShield ? $"<color={"#1EE3DA"}>{"!"}</color>" : "!") : txt;
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
                yield return SpawnWithDelay(shipData,limit);
            }
            // //ここで敵のアイテムをロード
            // foreach(var shipObject in enemyShipObjectList)
            // {
               
            // }   
            yield return new WaitUntil(()=>enemyShipObjectList.Count == 0);
        }

        public IEnumerator BossEncountCoroutine(ShipData shipData,BossType bossType)
        {
            var bossShipObject = SpawnBossShip(shipData,bossType);
            yield return new WaitUntil(()=>!bossShipObject);
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

