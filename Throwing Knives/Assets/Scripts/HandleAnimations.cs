using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandleAnimations : MonoBehaviour
{
    
    [SerializeField] private Animator animator;
    private PlayerMovements playerMovements;
    public float acceleration = 0.1f; //Aceleracion del movimiento
    public float deceleration = 0.1f; //Desaceleracion del movimiento
    public float velocity = 0.0f; //Velocidad variable
    private void Awake()
    {
        playerMovements = this.GetComponent<PlayerMovements>();
    }
    private void Update()
    {
        AnimateAim();
        AnimateRecoil();
        AnimateMovement();
        AnimateRolling();
    }
    private void AnimateAim() //Activa la animacion de apuntado
    {
        if (playerMovements.IsAiming)
        {
            animator.SetBool("isAiming", true);

        }
        else
        {
            animator.SetBool("isAiming", false);
        }
    }
    private void AnimateRecoil() //Activa la animacion de apuntado
    {
        if (playerMovements.IsRecoil)
        {
            animator.SetBool("recoil", true);

        }
        else
        {
            animator.SetBool("recoil", false);
        }
    }

    private void AnimateRolling() //Activa la animacion de rotacion
    {
        if (playerMovements.IsRolling) animator.SetBool("isRolling", true);
        else animator.SetBool("isRolling", false);
    }

    private void AnimateMovement() //Activa la animacion de movimiento
    {
        if (playerMovements.IsWalking && velocity < 0.5f)
        {
            velocity += Time.deltaTime * acceleration * 7;

        }

        if (playerMovements.IsSprinting && playerMovements.IsWalking && velocity < 1f)
        {
            velocity += Time.deltaTime * acceleration * 7;


        }
        if (!playerMovements.IsSprinting && playerMovements.IsWalking && velocity > 0.5f)
        {
            velocity -= Time.deltaTime * deceleration * 2;

        }
        if (!playerMovements.IsWalking && velocity > 0.0f)
        {
            velocity -= Time.deltaTime * deceleration * 2;

        }
        if (!playerMovements.IsWalking && velocity < 0.0f)
        {
            velocity = 0.0f;

        }
        animator.SetFloat("Speed", velocity);

    }
}
