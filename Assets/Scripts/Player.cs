using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    private const string ArrowShootSoundResourcePath = "Sound/BowSound";

    public float speed = 5f;

    [Header("Shoot")]
    public GameObject arrowPrefab;
    public Transform firePoint;
    public float attackCooldown = 0.5f;

    [Header("Audio")]
    public AudioClip arrowShootSound;
    [Range(0f, 1f)]
    public float arrowShootVolume = 1f;

    private InputAction movementAction;
    private InputAction shootAction;

    private Animator anim;
    private SpriteRenderer spriteRenderer;
    private AudioSource audioSource;
    private float lastAttackTime;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();

        if (arrowShootSound == null)
        {
            arrowShootSound = Resources.Load<AudioClip>(ArrowShootSoundResourcePath);
        }
    }

    private void OnEnable()
    {
        if (movementAction == null)
        {
            movementAction = new InputAction("Move", InputActionType.Value);
            movementAction.AddCompositeBinding("2DVector")
                .With("Up", "<Keyboard>/w")
                .With("Down", "<Keyboard>/s")
                .With("Left", "<Keyboard>/a")
                .With("Right", "<Keyboard>/d");
        }

        if (shootAction == null)
        {
            shootAction = new InputAction("Shoot", binding: "<Keyboard>/space");
        }

        movementAction.Enable();
        shootAction.Enable();

        shootAction.performed += OnShoot;
    }

    private void OnDisable()
    {
        movementAction.Disable();
        shootAction.Disable();

        shootAction.performed -= OnShoot;
    }

    void Update()
    {
        Move();
        LookAtMouse();
    }

    void Move()
    {
        Vector2 input = movementAction.ReadValue<Vector2>();
        Vector3 movement = new Vector3(input.x, input.y, 0f);

        transform.Translate(movement * speed * Time.deltaTime, Space.World);

        anim.SetBool("isWalk", input != Vector2.zero);
    }

    void LookAtMouse()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        mousePos.z = 0f;

        spriteRenderer.flipX = mousePos.x < transform.position.x;
    }

    // 🔥 스페이스 누르면 애니메이션만 실행
    private void OnShoot(InputAction.CallbackContext context)
    {
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            anim.SetTrigger("Attack");
            lastAttackTime = Time.time;
        }
    }

    // 🔥 이 함수는 Animation Event로 호출됨
    public void SpawnArrow()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        mousePos.z = 0f;

        Vector2 direction = mousePos - firePoint.position;

        GameObject arrowObj = Instantiate(arrowPrefab, firePoint.position, Quaternion.identity);
        arrowObj.GetComponent<Arrow>().SetDirection(direction);

        PlayArrowShootSound();
    }

    private void PlayArrowShootSound()
    {
        if (arrowShootSound == null)
        {
            Debug.LogWarning($"Player: Resources/{ArrowShootSoundResourcePath} arrow shoot sound was not found.");
            return;
        }

        if (audioSource != null)
        {
            audioSource.PlayOneShot(arrowShootSound, arrowShootVolume);
            return;
        }

        AudioSource.PlayClipAtPoint(arrowShootSound, firePoint.position, arrowShootVolume);
    }
}
