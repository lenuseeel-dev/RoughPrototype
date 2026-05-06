using UnityEngine;

public class OrcAttack : MonoBehaviour
{
    [Header("공격 설정")]
    public int damage = 1;
    public float attackRange = 1.5f;
    public float attackCooldown = 1.5f;

    float lastAttackTime;
    Transform player;
    Animator anim;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= attackRange && Time.time >= lastAttackTime + attackCooldown)
        {
            anim.SetTrigger("Attack");
            lastAttackTime = Time.time;
        }
    }

    // ⭐ 애니메이션 이벤트에서 호출
    public void DealDamage()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= attackRange)
        {
            PlayerHealth ph = player.GetComponent<PlayerHealth>();
            if (ph != null)
            {
                ph.TakeDamage(damage);
            }
        }
    }
}