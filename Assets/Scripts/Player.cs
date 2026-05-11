using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public float speed = 5f;

    [Header("Shoot")]
    public GameObject arrowPrefab;
    public Transform firePoint;
    public float attackCooldown = 0.5f;

    private InputAction movementAction;
    private InputAction shootAction;

    private Animator anim;
    private SpriteRenderer spriteRenderer;
    private float lastAttackTime;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
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
    }
}