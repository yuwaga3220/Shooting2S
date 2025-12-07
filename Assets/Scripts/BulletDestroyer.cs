using UnityEngine;

public class BulletDestroyer : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Bullet"))
        {
            Destroy(other.gameObject);
        }
    }
}