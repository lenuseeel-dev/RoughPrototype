using UnityEngine;
using System;
using System.Collections;

public class Enemy : MonoBehaviour
{
    private static readonly int IsWalkHash = Animator.StringToHash("isWalk");
    private static readonly int OrcAttackHash = Animator.StringToHash("OrcAttack");

    [Header("Movement")]
    public float speed = 3f;

    [Header("Attack")]
    public float attackCooldown = 1f;

    [Header("Separation")]
    public float separationRadius = 1.2f;
    public float separationForce = 2f;

    private bool isAttacking;
    private Coroutine attackRoutine;
    private PlayerHealth attackTarget;

    private Transform player;
    private Animator anim;
    private SpriteRenderer spriteRenderer;

    public Action OnDeath;

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

        anim.SetBool(IsWalkHash, true);

        spriteRenderer.flipX = player.position.x < transform.position.x;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        attackTarget = collision.GetComponent<PlayerHealth>();

        if (attackRoutine == null)
        {
            attackRoutine = StartCoroutine(AttackLoop());
        }
    }

    IEnumerator AttackLoop()
    {
        isAttacking = true;
        anim.SetBool(IsWalkHash, false);

        while (attackTarget != null)
        {
            anim.Play(OrcAttackHash, 0, 0f);
            attackTarget.TakeDamage(1);

            yield return new WaitForSeconds(attackCooldown);
        }

        isAttacking = false;
        attackRoutine = null;
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        attackTarget = null;

        if (attackRoutine != null)
        {
            StopCoroutine(attackRoutine);
            attackRoutine = null;
        }

        isAttacking = false;
    }

    public void TakeDamage()
    {
        Die();
    }

    void Die()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.AddKill();
        }

        OnDeath?.Invoke();

        Destroy(gameObject);
    }
}
