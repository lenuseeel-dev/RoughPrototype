using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public float speed = 5f;

    [Header("Shoot")]
    public GameObject arrowPrefab;
    public Transform firePoint;

    private InputAction movementAction;
    private InputAction shootAction;

    private Animator anim;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        // 이동 입력
        if (movementAction == null)
        {
            movementAction = new InputAction("Move", InputActionType.Value);
            movementAction.AddCompositeBinding("2DVector")
                .With("Up", "<Keyboard>/w")
                .With("Down", "<Keyboard>/s")
                .With("Left", "<Keyboard>/a")
                .With("Right", "<Keyboard>/d");
        }

        // 공격 입력
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

    // 🔥 스페이스바 누르면 발사
    private void OnShoot(InputAction.CallbackContext context)
    {
        Shoot();
    }

    void Shoot()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        mousePos.z = 0f;

        Vector2 direction = mousePos - firePoint.position;

        GameObject arrowObj = Instantiate(arrowPrefab, firePoint.position, Quaternion.identity);
        arrowObj.GetComponent<Arrow>().SetDirection(direction);

        // 공격 애니메이션
        anim.SetTrigger("Attack");
    }
}