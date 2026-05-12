using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

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

    [Header("하트 UI")]
    public SpriteRenderer[] hearts;
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
        if (hearts == null) return;

        for (int i = 0; i < hearts.Length; i++)
        {
            if (hearts[i] == null) continue;

            hearts[i].sprite = (i < currentHealth) ? heartOn : heartOff;
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

        // ⭐ 입력 차단용 딜레이
        yield return new WaitForSeconds(1.2f);

        // ⭐ 먼저 타이머/게임 종료 처리
        if (GameManager.instance != null)
        {
            GameManager.instance.GameOver();
        }

        // ⭐ 씬 이동은 마지막
        SceneManager.LoadScene("GameOverScene");
    }
}