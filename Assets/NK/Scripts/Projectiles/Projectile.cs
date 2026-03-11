using UnityEngine;
using Ships;
namespace Projectiles
{
    public class Projectile : MonoBehaviour
    {
        public int _power{get;private set;}
        public bool _isPiercing{get;private set;}
        public bool _isDamaging = true;
        public void SetProjectile(int power,bool isPiercing)
        {
            _power = power;
            _isPiercing = isPiercing;
        }
        private void OnTriggerEnter2D(Collider2D collision)
        {
            bool b1 = collision.gameObject.CompareTag("PlayerShip") && gameObject.CompareTag("EnemyProjectile");
            bool b2 = collision.gameObject.CompareTag("EnemyShip") && gameObject.CompareTag("PlayerProjectile");
            if(b1 || b2)
            {
                collision.gameObject.GetComponent<Ship>().DealDamage(_power);
                if(!_isPiercing && _isDamaging)Destroy(gameObject);
            }
        }
    }
}

