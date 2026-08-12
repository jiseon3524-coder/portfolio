using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Boss0Hit : MonoBehaviour
{
    private Collider2D hitCollider;

    private int currentDamage;
    private bool isAttackActive;
    private bool hasHitPlayer;

    private void Awake()
    {
        hitCollider =
            GetComponent<Collider2D>();

        hitCollider.isTrigger =
            true;

        hitCollider.enabled =
            false;
    }

    public void BeginAttack(
        int damage
    )
    {
        currentDamage =
            Mathf.Max(
                0,
                damage
            );

        hasHitPlayer =
            false;

        isAttackActive =
            true;

        if (hitCollider != null)
        {
            hitCollider.enabled =
                true;
        }
    }

    public void EndAttack()
    {
        isAttackActive =
            false;

        hasHitPlayer =
            false;

        if (hitCollider != null)
        {
            hitCollider.enabled =
                false;
        }
    }

    private void OnTriggerEnter2D(
        Collider2D other
    )
    {
        TryDamagePlayer(
            other
        );
    }

    private void OnTriggerStay2D(
        Collider2D other
    )
    {
        TryDamagePlayer(
            other
        );
    }

    private void TryDamagePlayer(
        Collider2D other
    )
    {
        if (!isAttackActive ||
            hasHitPlayer ||
            other == null)
        {
            return;
        }

        PlayerStats playerStats =
            other.GetComponentInParent<PlayerStats>();

        if (playerStats == null)
        {
            return;
        }

        hasHitPlayer =
            true;

        playerStats.TakeDamage(
            currentDamage
        );

        Debug.Log(
            $"{gameObject.name}: " +
            $"보스 공격으로 플레이어에게 " +
            $"{currentDamage} 피해"
        );
    }

    private void OnDisable()
    {
        EndAttack();
    }
}
