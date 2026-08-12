using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class GhostProjectile : MonoBehaviour
{
    [Header("Projectile")]
    [SerializeField] private float speed = 6f;
    [SerializeField] private int damage = 10;
    [SerializeField] private float lifeTime = 4f;

    private Rigidbody2D rb;
    private bool hasHit;

    private void Awake()
    {
        rb =
            GetComponent<Rigidbody2D>();

        rb.gravityScale = 0f;
        rb.freezeRotation = true;
    }

    private void Start()
    {
        Destroy(
            gameObject,
            lifeTime
        );
    }

    public void Initialize(
        Vector2 direction)
    {
        if (direction.sqrMagnitude
            <= 0.001f)
        {
            direction =
                Vector2.down;
        }

        direction.Normalize();

        rb.linearVelocity =
            direction * speed;

        float angle =
            Mathf.Atan2(
                direction.y,
                direction.x
            )
            * Mathf.Rad2Deg;

        transform.rotation =
            Quaternion.Euler(
                0f,
                0f,
                angle
            );
    }

    private void OnTriggerEnter2D(
        Collider2D other)
    {
        if (hasHit)
        {
            return;
        }

        if (IsWall(other))
        {
            hasHit = true;
            Destroy(gameObject);
            return;
        }

        if (!other.CompareTag("Player"))
        {
            return;
        }

        PlayerStats playerStats =
            other.GetComponent<PlayerStats>();

        if (playerStats == null)
        {
            playerStats =
                other.GetComponentInParent<
                    PlayerStats
                >();
        }

        if (playerStats != null)
        {
            playerStats.TakeDamage(
                damage
            );
        }

        hasHit = true;
        Destroy(gameObject);
    }

    private bool IsWall(
        Collider2D targetCollider)
    {
        Transform current =
            targetCollider.transform;

        while (current != null)
        {
            if (current.CompareTag("Wall"))
            {
                return true;
            }

            current = current.parent;
        }

        return false;
    }
}
