using UnityEngine;

public class BulletController : MonoBehaviour
{
    [SerializeField] private int damageAmount = 10;    

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerController playerHealth = collision.GetComponent<PlayerController>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damageAmount);
            Destroy(gameObject);
        }
        Destroy(gameObject,1f);
    }
}
