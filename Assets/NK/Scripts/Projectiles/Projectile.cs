using UnityEngine;
using Ships;
using Items;
using Managers;
namespace Projectiles
{
    public class Projectile : MonoBehaviour
    {
        public Ship _dealtShip;
        //public Ship _ship;
        public int _power{get;private set;}
        public bool _isPiercing{get;private set;}
        
        public bool _isDamaging = true;
        //
        private bool isDamagedOnce = false; 
        public void Start()
        {
            if(gameObject.CompareTag("PlayerProjectile"))GetComponent<SpriteRenderer>().color = Color.turquoise;
            else if(gameObject.CompareTag("EnemyProjectile"))GetComponent<SpriteRenderer>().color = Color.orange;
        }
        public void SetProjectile(Ship ship,int power,bool isPiercing,bool isDamaging)
        {
            _power = power;
            _isPiercing = isPiercing;
            _dealtShip = ship;
            _isDamaging = isDamaging;
        }
        private void OnTriggerEnter2D(Collider2D collision)
        {
            //与えるダメージは発射した時点でのpowerの値
            if(!_isDamaging)return;
            bool b1 = collision.gameObject.CompareTag("PlayerShip") && gameObject.CompareTag("EnemyProjectile");
            bool b2 = collision.gameObject.CompareTag("EnemyShip") && gameObject.CompareTag("PlayerProjectile");
            if(b1 || b2)
            {
                if(!isDamagedOnce)
                {
                    var ship = collision.gameObject.GetComponent<Ship>();
                    bool isCrit = Random.Range(1,1000) < _dealtShip.critRate.Value * 10;
                    int truePower =  isCrit? _power * 2 : _power;
                    ship.DealDamage(truePower,isCrit,_dealtShip);
                    if(!_isPiercing)isDamagedOnce = true;
                }
                if(!_isPiercing)Destroy(gameObject);
            }
        }
    }
}

