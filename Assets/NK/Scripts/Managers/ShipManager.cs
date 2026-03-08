using UnityEngine;
using UnityEngine.InputSystem;
using UniRx;
using UniRx.Triggers;
using System.Collections.Generic;
using Ships;
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
        private List<GameObject> playerShipList = new List<GameObject>();
        private List<GameObject> enemyShipList = new List<GameObject>();
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
            foreach(var shipData in playerShipDataList)
            {
                InstantiatePlayerShip(shipData);
            }   
            foreach(var shipData in enemyShipDataList)
            {
                InstantiateEnemyShip(shipData);
            }  
        }
        private void InstantiatePlayerShip(ShipData shipData)
        {
            GameObject shipObject = Instantiate(_shipObject);
            playerShipList.Add(shipObject);

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
            ship.shipData = shipData;
            ship.SetAO(playerShipList,enemyShipList);
            ship.SetSPHP();
            ship.SetWeapon();
        }   
        private void InstantiateEnemyShip(ShipData shipData)
        {
            GameObject shipObject = Instantiate(_shipObject);
            enemyShipList.Add(shipObject);

            float randDeg = UnityEngine.Random.Range(-180f,180f);
            var pos = 2f * new Vector2(Mathf.Cos(randDeg  * Mathf.Deg2Rad),Mathf.Sin(randDeg  * Mathf.Deg2Rad));
            shipObject.transform.position = pos;
            // shipObject.transform.eulerAngles = new Vector3(0f,0f,_currentFleetDeg);
            // shipObject.UpdateAsObservable()
            // .Subscribe(_ =>
            // {
            //     shipObject.transform.position = _currentFleetPos + offset;
            //     shipObject.transform.eulerAngles = new Vector3(0f,0f,_currentFleetDeg);
            // })
            // .AddTo(shipObject);

            Ship ship = shipObject.GetComponent<Ship>();
            ship.isPlayer = false;
            ship.shipData = shipData;
            ship.SetAO(enemyShipList,playerShipList);
            ship.SetSPHP();
        }   
    }
}

