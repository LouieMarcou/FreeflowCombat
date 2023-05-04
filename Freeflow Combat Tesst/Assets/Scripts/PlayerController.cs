using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;
using System;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private CharacterStats stats;
    private float currentHealth;
    private float maxHealth;
    private float damage;

    private enum PlayerStates { idle, attacking};
    private PlayerStates currentState = PlayerStates.idle;
    [SerializeField] private Camera cam;
    private Vector2 movementInput = Vector2.zero;
    private Vector3 playerVelocity;
    private bool groundedPlayer;
    private bool jumped;

    private Vector3 inputDirection;

    private float currentSpeed = 5f;

    private float jumpHeight = 1.0f;
    private float gravityValue = -9.81f;

    private CharacterController controller;

    [SerializeField] private MouseLook mouseLook;
    private RaycastHit info;

    private EnemyScript currentTarget;

    private float attackTimer = 0f;
    public void OnMove(InputAction.CallbackContext context)
    {
        movementInput = context.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        mouseLook.mouseInput = context.ReadValue<Vector2>();
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed && currentState != PlayerStates.attacking)
        {

            var forward = cam.transform.forward;
            var right = cam.transform.right;

            forward.y = 0f;
            right.y = 0f;

            forward.Normalize();
            right.Normalize();

            inputDirection = forward * movementInput.y + right * movementInput.x;
            //inputDirection = cam.transform.forward * movementInput.y;
            inputDirection = inputDirection.normalized;

            if (Physics.SphereCast(transform.position, 3f, inputDirection, out info, 10))
            {
                if (info.collider.transform.GetComponent<EnemyScript>().GetIsAttackable())
                {
                    
                    //Vector3 temp = new Vector3(inputDirection.x, inputDirection.y, transform.position.z);
                    Debug.DrawRay(transform.position, inputDirection * 10f, Color.black, 100f);
                    currentTarget = info.collider.transform.GetComponent<EnemyScript>();
                    currentState = PlayerStates.attacking;
                    MoveTowardsTarget(currentTarget, 0.5f);
                }
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        maxHealth = stats.Health;
        currentHealth = maxHealth;
        damage = stats.Damage;
    }

    // Update is called once per frame
    void Update()
    {
        MovePlayer();
    }

    private void MovePlayer()
    {
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }

        Vector3 move = (transform.right * movementInput.x + transform.forward * movementInput.y);
        controller.Move(move * Time.deltaTime * currentSpeed);

        if (jumped && groundedPlayer)
        {
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
        }

        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);


    }

    private void MoveTowardsTarget(EnemyScript target, float duration)
    {
        transform.DOLookAt(target.transform.position, 0.2f);
        transform.DOMove(TargetOffset(target.transform), duration);

        StartCoroutine(TimeToWaitBeforeAttackLands(duration * 0.5f));
    }

    private Vector3 TargetOffset(Transform target)
    {
        Vector3 position;
        position = target.position;
        return Vector3.MoveTowards(position, transform.position, 0.95f);
    }

    private IEnumerator TimeToWaitBeforeAttackLands(float duration)
    {
        yield return new WaitForSeconds(duration);
        //Debug.Log("target reached");
        currentTarget.TakeDamage(damage);
        currentState = PlayerStates.idle;
    }

    public EnemyScript CurrentTarget()
    {
        return currentTarget;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawRay(transform.position, inputDirection);
        Gizmos.DrawWireSphere(transform.position, 1);
        if (CurrentTarget() != null)
            Gizmos.DrawSphere(CurrentTarget().transform.position, .5f);
    }

}
