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
        public List<ShipData> playerShipDataList = new List<ShipData>();
        public List<ShipData> enemyShipDataList = new List<ShipData>();
        private List<GameObject> playerShipObjectList = new List<GameObject>();
        private List<GameObject> enemyShipObjectList = new List<GameObject>();
        [Header("Canvas")]
        public RectTransform DamageValueCanvas;
        [Header("Prefab")]
        public TextMeshProUGUI damageValueDisplay;
        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }
        void Start()
        {
            if(GManager.Instance.playerShipDataList.Count != 0)playerShipDataList = GManager.Instance.playerShipDataList;
            if(GManager.Instance.enemyShipDataList.Count != 0)enemyShipDataList = GManager.Instance.enemyShipDataList;
            _shipObject = (GameObject)Resources.Load("Ship");
            _cam = Camera.main;
            gameObject.UpdateAsObservable()
                .Where(_=>Input.GetMouseButton(0))
                .Subscribe(_ =>
                {
                    Vector2 destination = _cam.ScreenToWorldPoint(Input.mousePosition);
                    if(Vector2.Distance(destination,_currentFleetPos) < 0.1f)return;
                    var v = destination - _currentFleetPos;
                    _currentFleetDeg = Mathf.MoveTowardsAngle(_currentFleetDeg,Mathf.Atan2(v.y,v.x) * Mathf.Rad2Deg,720f*Time.deltaTime);
                    
                    _currentFleetPos += new Vector2(Mathf.Cos(_currentFleetDeg * Mathf.Deg2Rad),Mathf.Sin(_currentFleetDeg * Mathf.Deg2Rad)) * 5f * Time.deltaTime;
                    //Debug.DrawRay(_currentFleetPos,new Vector2(Mathf.Cos(_currentFleetDeg * Mathf.Deg2Rad),Mathf.Sin(_currentFleetDeg * Mathf.Deg2Rad)));
                })
                .AddTo(gameObject);
            // foreach(var shipData in enemyShipDataList)
            // {
            //     InstantiateEnemyShip(shipData);
            // }  
            foreach(var shipData in playerShipDataList)
            {
                InstantiatePlayerShip(shipData);
            }   
            
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
        private void InstantiatePlayerShip(ShipData shipData)
        {
            GameObject shipObject = Instantiate(_shipObject);
            playerShipObjectList.Add(shipObject);

            float randDeg = UnityEngine.Random.Range(-180f,180f);
            var offset = 0.5f * new Vector2(Mathf.Cos(randDeg  * Mathf.Deg2Rad),Mathf.Sin(randDeg  * Mathf.Deg2Rad));
            shipObject.transform.position = _currentFleetPos;
            shipObject.transform.eulerAngles = new Vector3(0f,0f,_currentFleetDeg);
            shipObject.UpdateAsObservable()
            .Subscribe(_ =>
            {
                shipObject.transform.position = _currentFleetPos + offset;
                shipObject.transform.eulerAngles = new Vector3(0f,0f,_currentFleetDeg);
            })
            .AddTo(shipObject);

            Ship ship = shipObject.GetComponent<Ship>();
            ship.isPlayer = true;
            ship.tag = "PlayerShip";
            ship.shipData = shipData;
            ship.SetShipList(playerShipObjectList,enemyShipObjectList);
            ship.SetSPHP();
            ship.SetWeapon();

            ship.currentPower = new(shipData.power);
        }   
        private void InstantiateEnemyShip(ShipData shipData)
        {
            GameObject shipObject = Instantiate(_shipObject);
            enemyShipObjectList.Add(shipObject);

            float randDeg = UnityEngine.Random.Range(-180f,180f);
            var pos = 2f * new Vector2(Mathf.Cos(randDeg  * Mathf.Deg2Rad),Mathf.Sin(randDeg  * Mathf.Deg2Rad));
            shipObject.transform.position = pos;
            shipObject.transform.eulerAngles = new Vector3(0f,0f,0f);
            shipObject.UpdateAsObservable()
            .Subscribe(_ =>
            {
                var s = shipObject;
                if(Vector2.Distance(s.transform.position,_currentFleetPos) < 0.1f)return;
                var v = _currentFleetPos - (Vector2)s.transform.position;
                var deg = Mathf.MoveTowardsAngle(s.transform.eulerAngles.z,Mathf.Atan2(v.y,v.x) * Mathf.Rad2Deg,360f*Time.deltaTime);
                s.transform.eulerAngles = new Vector3(0f,0f,deg);
                s.transform.position = (Vector2)s.transform.position + new Vector2(Mathf.Cos(deg * Mathf.Deg2Rad),Mathf.Sin(deg * Mathf.Deg2Rad)) * shipData.speed * Time.deltaTime; 
            })
            .AddTo(shipObject);

            Ship ship = shipObject.GetComponent<Ship>();
            ship.isPlayer = false;
            ship.tag = "EnemyShip";
            ship.shipData = shipData;
            ship.SetShipList(enemyShipObjectList,playerShipObjectList);
            ship.SetSPHP();
            ship.SetWeapon();
            ship.currentPower = new(shipData.power);
        }   
        public void BattleEncountWave(List<EnemyWave> enemyWaveList,int limit)
        {
            StartCoroutine(BattleEncountWaveCoroutine(enemyWaveList,limit));
        }
        private IEnumerator BattleEncountWaveCoroutine(List<EnemyWave> enemyWaveList,int limit)
        {
            int currentWaveNum = 1;
            foreach(var enemyWave in enemyWaveList)
            {
                Debug.Log(currentWaveNum);
                yield return WaveCoroutine(enemyWave,limit);
                yield return new WaitForSeconds(2f);
                currentWaveNum++;
            }
            EventManager.OnStageClear.OnNext(Unit.Default);
        }
        private IEnumerator WaveCoroutine(EnemyWave enemyWave,int limit)
        {

            foreach(var shipData in enemyWave.shipDataList)
            {
                InstantiateEnemyShip(shipData);
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

