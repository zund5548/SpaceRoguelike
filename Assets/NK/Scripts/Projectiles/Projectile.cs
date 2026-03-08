using UnityEngine;
using Ships;
namespace Projectiles
{
    public class Projectile : MonoBehaviour
    {
        public int power;
        private void OnTriggerEnter2D(Collider2D collision)
        {
            bool b1 = collision.gameObject.CompareTag("PlayerShip") && gameObject.CompareTag("EnemyProjectile");
            bool b2 = collision.gameObject.CompareTag("EnemyShip") && gameObject.CompareTag("PlayerProjectile");
            if(b1 || b2)
            {
                collision.gameObject.GetComponent<Ship>().DealDamage(power);
                Destroy(gameObject);
            }
        }
    }
}

