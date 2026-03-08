using UnityEngine;
using UnityEngine.InputSystem;
namespace Managers
{
    public class ShipManager : MonoBehaviour
    {
        private  Keyboard _keyboard;

        private GameObject shipObject;
        public static ShipManager Instance{get;private set;}
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
            shipObject = (GameObject)Resources.Load("Ship");
        }

    }
}

