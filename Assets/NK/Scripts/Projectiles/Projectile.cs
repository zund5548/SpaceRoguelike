using UnityEngine;
using Ships;
using Items;
namespace Projectiles
{
    public class Projectile : MonoBehaviour
    {
        public int _power{get;private set;}
        public Ship _dealtShip;
        public bool _isPiercing{get;private set;}
        public Ship _ship;
        public bool _isDamaging = true;
        //
        private bool isDamagedOnce = false; 
        public void SetProjectile(Ship ship,int power,bool isPiercing)
        {
            _power = power;
            _isPiercing = isPiercing;
            _dealtShip = ship;
        }
        private void OnTriggerEnter2D(Collider2D collision)
        {
            //与えるダメージは発射した時点でのpowerの値
            bool b1 = collision.gameObject.CompareTag("PlayerShip") && gameObject.CompareTag("EnemyProjectile");
            bool b2 = collision.gameObject.CompareTag("EnemyShip") && gameObject.CompareTag("PlayerProjectile");
            if(b1 || b2)
            {
                if(_isDamaging)
                {
                    if(!isDamagedOnce)
                    {
                        var ship = collision.gameObject.GetComponent<Ship>();
                        ship.DealDamage(_power,_dealtShip);
                        //ship.stackEffectController.AddStackNum<CrackStackEffect>(ship,1);
                    }
                    
                    if(!_isPiercing)isDamagedOnce = true;
                }
                if(!_isPiercing)Destroy(gameObject);
            }
        }
    }
}

