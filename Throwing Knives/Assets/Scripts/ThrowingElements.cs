using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowingElements : MonoBehaviour
{
    #region Variables
    [Header("References")]
    public Transform thirdPersonCamera;
    public Transform firstPersonCamera;
    public Transform attackPoint;
    public GameObject objectToThrow;
    private PlayerMovements playerMovements;

    [Header("Settings")]
    public int totalThrows;
    public float throwCooldown;

    [Header("Throwing")]
    public KeyCode throwKey = KeyCode.Mouse0;
    public float throwForce; //Fuerza de lanzamiento
    public float throwUpwardForce; //Fuerza vertical

    bool readyToThrow; 
    #endregion

    #region Unity Methods
    private void Awake()
    {
        playerMovements = this.GetComponent<PlayerMovements>();

    }
    private void Start()
    {
        //Al comenzar siempre estará listo el personaje para disparar
        readyToThrow = true;

    }
    private void Update()
    {
        //Si el personaje pulsa el botón izquierdo del mouse aplica el metodo de lanzar
        if (Input.GetKeyDown(throwKey) && readyToThrow && totalThrows > 0)
        {
            ThrowElement();
        }

        if (totalThrows == 0)
        {
            Debug.Log("Te quedaste sin flechas");
        }
    }
    #endregion

    #region Throw Methods
    private void ThrowElement() //Lanza cualquier elemento que se añada como referencia, en este caso flechas (En el proyecto se creó el prefab de las flechas)
    {
        readyToThrow = false;

        GameObject projectile = Instantiate(objectToThrow, attackPoint.position, thirdPersonCamera.rotation); //Crea una nueva flecha

        Rigidbody projectileRb = projectile.GetComponent<Rigidbody>();

        Vector3 forceDirection = thirdPersonCamera.transform.forward;

        RaycastHit hit;

        //Si esta en 3era persona calcula la posicion con la camara de tercera persona y viceversa
        if (playerMovements.is3rdPerson)
        {
            if (Physics.Raycast(thirdPersonCamera.position, thirdPersonCamera.forward, out hit, 500f)) //Verifica con un raycast la posicion central de la camara
            {
                forceDirection = (hit.point - attackPoint.position).normalized; //Se normaliza la direccion del raycast por si el jugador ve en diagonal
            }
        }
        else if (!playerMovements.is3rdPerson)
        {
            if (Physics.Raycast(firstPersonCamera.position, firstPersonCamera.forward, out hit, 500f)) //Verifica con un raycast la posicion central de la camara
            {
                forceDirection = (hit.point - attackPoint.position).normalized; //Se normaliza la direccion del raycast por si el jugador ve en diagonal
            }
        }


        Vector3 forceToAdd = forceDirection * throwForce + transform.up * throwUpwardForce; //Se calcula la direccion de la fecha junto con la fuerza de lanzamiento y la fuerza vertical

        projectileRb.AddForce(forceToAdd, ForceMode.Impulse); //Se añade la fuerza al rigidbody del prefab de la flecha para ser lanzada hacia adelante

        totalThrows--; //Se resta la cantidad de elementos a lanzar

        Invoke(nameof(ResetThrow), throwCooldown); //Cada lanzamiento tiene un tiempo de enfriamiento, y es aplicado con el metodo reset usando el metodo invoke para asignar el timer
    }
    private void ResetThrow()
    {
        readyToThrow = true;
    } 
    #endregion
}
