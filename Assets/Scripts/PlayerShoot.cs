using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShoot : MonoBehaviour
{
    public GameObject arrowPrefab;
    public Transform firePoint;

    private InputAction shootAction;
    private Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        if (shootAction == null)
        {
            shootAction = new InputAction("Shoot", binding: "<Keyboard>/space");
        }

        shootAction.Enable();
        shootAction.performed += OnShoot;
    }

    private void OnDisable()
    {
        shootAction.performed -= OnShoot;
        shootAction.Disable();
    }

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

        Arrow arrow = arrowObj.GetComponent<Arrow>();
        arrow.SetDirection(direction);

        // 공격 애니메이션
        if (anim != null)
            anim.SetTrigger("Attack");
    }
}