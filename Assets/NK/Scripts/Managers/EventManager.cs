using UnityEngine;
using UniRx;
namespace Managers
{
    public class EventManager : MonoBehaviour
    {
        public static EventManager Instance{get;private set;}     
        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }
        public static Subject<Unit> OnStageClear = new Subject<Unit>();
    }
}

