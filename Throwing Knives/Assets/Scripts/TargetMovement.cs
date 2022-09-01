using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetMovement : MonoBehaviour
{
    Vector3 initialPosition; //Guarda la posicion inicial de la diana
    private bool isDecreasing = false; //Indica si la diana se está regresando
    public Collider centerCollider;
    float depth = -0.10F; //Profundidad de clavado de la flecha en la diana

    void Start()
    {
        initialPosition = transform.position;
    }


    void Update()
    {
        if (transform.position.x < initialPosition.x + 3.1f && !isDecreasing)
        {
            
            transform.Translate(Vector3.right * Time.deltaTime, Space.World);

        }
        else
        {
            isDecreasing = true;
        }

        if (transform.position.x > initialPosition.x - 2.8f && isDecreasing)
        {
            transform.Translate(Vector3.left * Time.deltaTime, Space.World);

        }
        else
        {
            isDecreasing = false;

        }

    }


    private void OnTriggerEnter(Collider other) //Detecta si un objeto colisiona con la diana
    {
        //Guarda el rigidbody y el collider del elemento que chocó con la diana
        Rigidbody temporalRigidBody = other.GetComponent<Rigidbody>();
        Collider temporalCollider = other.GetComponent<Collider>();

        //Se guarda el elemento que chocó dentro de la diana, es decir la diana se convierte en padre de las flechas
        other.transform.parent = this.transform;
        other.transform.Translate(depth * -Vector3.forward); //Se aplica la profundidad


        //Se elimina el rigidbody y el collider
        Destroy(temporalRigidBody);
        Destroy(temporalCollider);
    }

}
