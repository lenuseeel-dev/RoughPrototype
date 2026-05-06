using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    [Header("체력")]
    public int maxHealth = 3;
    int currentHealth;

    [Header("무적")]
    public float invincibleTime = 1f;
    bool isInvincible = false;

    Animator anim;
    SpriteRenderer sr;

    [Header("하트 UI (왼쪽부터 순서대로 넣기)")]
    public SpriteRenderer[] hearts;   // 3개 넣기
    public Sprite heartOn;
    public Sprite heartOff;

    bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();

        UpdateHearts();
    }

    public void TakeDamage(int damage)
    {
        if (isInvincible || isDead) return;

        currentHealth -= damage;
        anim.SetTrigger("Hurt");

        UpdateHearts();

        if (currentHealth <= 0)
        {
            isDead = true;
            StartCoroutine(DeathSequence());
        }
        else
        {
            StartCoroutine(InvincibleCoroutine());
        }
    }

    void UpdateHearts()
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < currentHealth)
                hearts[i].sprite = heartOn;
            else
                hearts[i].sprite = heartOff;
        }
    }

    IEnumerator InvincibleCoroutine()
    {
        isInvincible = true;
        yield return new WaitForSeconds(invincibleTime);
        isInvincible = false;
    }

    IEnumerator DeathSequence()
    {
        anim.SetTrigger("Die");

        yield return new WaitForSeconds(1.2f);

        Color c = sr.color;
        c.a = 0f;
        sr.color = c;

        Time.timeScale = 0f;
    }
}