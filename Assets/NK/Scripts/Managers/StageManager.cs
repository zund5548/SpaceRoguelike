using Maps;
using UniRx;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Managers
{
    public class StageManager : MonoBehaviour
    {
        public static StageManager Instance{get;private set;}
        public Button ToMapButton;
        GameObject _planetObject;
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
            _planetObject = (GameObject)Resources.Load("PlanetObject");
            SetToMapButton();
            InstantiateStage(GManager.Instance.currentStageNode);
        }
        private void SetToMapButton()
        {
            if(ToMapButton == null)return;
            ToMapButton.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    ToMapButton.interactable = false;
                    SceneLoader.Instance.ToMap();
                })
                .AddTo(ToMapButton.gameObject);
        }
        private void InstantiateStage(StageNode stageNode)
        {
            if(stageNode.planetList.Count == 0)return;
            //恒星
            // var starObject = Instantiate(_planetObject);
            // Star star = (Star)stageNode.planetList[0];    
            // starObject.name = "StarObject";
            // starObject.transform.position = Vector2.zero;
            // starObject.transform.localScale = star.radius * Vector2.one;
             
            // if(star.sprite != null)star.GetComponent<SpriteRenderer>().sprite = star.sprite;
            //惑星
            
            foreach(var star in stageNode.starList)
            {
                var planetObject = Instantiate(_planetObject);
                planetObject.transform.position = Vector2.zero;
                planetObject.transform.localScale = star.radius * Vector2.one;
            }   
            float radius = 3f;
            int pointNum = 100;
            foreach(var planet in stageNode.planetList)
            {
                var planetObject = Instantiate(_planetObject);
                float rad = Random.Range(-Mathf.PI,Mathf.PI);
                planetObject.transform.position = radius * new Vector2(Mathf.Cos(rad),Mathf.Sin(rad));
                planetObject.transform.localScale = planet.radius * Vector2.one;
                var LR = planetObject.GetComponent<LineRenderer>();
                LR.positionCount = pointNum;
                float orbitRad = 0f;
                for(int i = 0;i < pointNum;i++)
                {
                    LR.SetPosition(i,radius * new Vector3(Mathf.Cos(orbitRad),Mathf.Sin(orbitRad),0f));
                    orbitRad += 2f * Mathf.PI/pointNum;
                }
                radius += Random.Range(2f,4f);
            }   
        }
    }
}

