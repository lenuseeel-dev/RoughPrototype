using UnityEngine;
using System;

public class Enemy : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 3f;

    [Header("Attack")]
    public float attackCooldown = 1f;

    [Header("Separation")]
    public float separationRadius = 1.2f;
    public float separationForce = 2f;

    private float lastAttackTime;
    private bool isAttacking;

    private Transform player;
    private Animator anim;
    private SpriteRenderer spriteRenderer;

    public Action OnDeath; // ⭐ 스포너 연결용

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (player == null || isAttacking) return;

        Vector3 dirToPlayer = (player.position - transform.position).normalized;

        // 🔥 분리 로직 (오크끼리 안 겹치게)
        Collider2D[] nearby = Physics2D.OverlapCircleAll(transform.position, separationRadius);
        Vector3 separation = Vector3.zero;

        foreach (var col in nearby)
        {
            if (col.gameObject != gameObject && col.CompareTag("Enemy"))
            {
                Vector3 away = transform.position - col.transform.position;
                float dist = away.magnitude;

                if (dist > 0.01f)
                    separation += away.normalized / dist;
            }
        }

        Vector3 finalDir = (dirToPlayer * 0.7f + separation * separationForce).normalized;

        transform.position += finalDir * speed * Time.deltaTime;

        anim.SetBool("isWalk", true);

        // 방향
        spriteRenderer.flipX = player.position.x < transform.position.x;
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        isAttacking = true;
        anim.SetBool("isWalk", false);

        if (Time.time >= lastAttackTime + attackCooldown)
        {
            anim.SetTrigger("Attack");
            lastAttackTime = Time.time;
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        isAttacking = false;
    }

    // 🔥 화살 맞으면 호출됨
    public void TakeDamage()
    {
        Die();
    }

    void Die()
    {
        OnDeath?.Invoke(); // ⭐ 스포너에 알림
        Destroy(gameObject);
    }
}