using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class detectCollision : MonoBehaviour
{
    //Las flechas detectan si dan en el centro de la diana para indicar que el jugador ha obtenido puntos

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Center"))
        {
            Debug.Log("Le diste en el centro!! Obtienes 9 puntos!");
        }
    }
}
