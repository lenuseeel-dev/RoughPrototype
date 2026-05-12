using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    private const string DeathSoundResourcePath = "Sound/PlayerDeathSound";

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

    [Header("Audio")]
    public AudioClip deathSound;
    [Range(0f, 1f)]
    public float deathSoundVolume = 1f;

    bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();

        if (deathSound == null)
        {
            deathSound = Resources.Load<AudioClip>(DeathSoundResourcePath);
        }

        UpdateHearts();
    }

    public void TakeDamage(int damage)
    {
        if (isInvincible || isDead) return;

        currentHealth -= damage;
        anim.SetTrigger("Hurt");

        // 카메라 흔들림 효과
        CameraShake cameraShake = Camera.main?.GetComponent<CameraShake>();
        if (cameraShake != null)
        {
            cameraShake.Shake();
        }

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
        PlayDeathSound();

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

    void PlayDeathSound()
    {
        if (deathSound == null)
        {
            Debug.LogWarning($"PlayerHealth: Resources/{DeathSoundResourcePath} 사망 사운드를 찾을 수 없습니다.");
            return;
        }

        GameObject soundObject = new GameObject("PlayerDeathSound");
        DontDestroyOnLoad(soundObject);

        AudioSource audioSource = soundObject.AddComponent<AudioSource>();
        audioSource.clip = deathSound;
        audioSource.volume = deathSoundVolume;
        audioSource.spatialBlend = 0f;
        audioSource.Play();

        Destroy(soundObject, deathSound.length);
    }
}
