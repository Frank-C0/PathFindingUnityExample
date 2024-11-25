using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour

    {
        public float velocidad = 5f;
        private Rigidbody2D rb;

        void Start()
        {
            rb = GetComponent<Rigidbody2D>();

            rb.gravityScale = 0;
            rb.freezeRotation = true;
        }

        void Update()
        {
            // Captura las entradas de las teclas de flecha
            float movimientoHorizontal = Input.GetAxis("Horizontal");
            float movimientoVertical = Input.GetAxis("Vertical");

            Vector2 movimiento = new Vector2(movimientoHorizontal, movimientoVertical) * velocidad;
            rb.velocity = movimiento;
        }
    }
