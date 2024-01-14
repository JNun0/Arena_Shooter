using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public float walkSpeed = 8f;
    public float sprintSpeed = 14f;
    public float maxVelocityChange = 10f;

    [Space]
    public float airControl = 0.5f;

    [Space]
    public float jumpHeight = 5f;

    private Vector2 input;
    private Rigidbody rb;

    private bool sprinting;
    private bool jumping;

    private bool grounded = false;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        input.Normalize();

        sprinting = Input.GetButton("Sprint");
        jumping = Input.GetButton("Jump");
    }

    private void OnTriggerStay(Collider other)
    {
        grounded = true;
    }

    void FixedUpdate()
    {
        if (grounded)
        {
            if (jumping)
            {
                //Aplica uma for�a para cima
                rb.velocity = new Vector3(rb.velocity.x, jumpHeight, rb.velocity.z);
            } 
            else if (input.magnitude > 0.5f)
            {
                //Aplica uma for�a na dire��o do movimento
                rb.AddForce(CalculateMovement(sprinting ? sprintSpeed : walkSpeed), ForceMode.VelocityChange);
            }
            else
            {
                //Deslize pequeno ao deixar de andar
                var velocity1 = rb.velocity;
                velocity1 = new Vector3(velocity1.x * 0.2f * Time.fixedDeltaTime, velocity1.y, velocity1.z * 0.2f * Time.fixedDeltaTime);
            }
        }
        else
        {
            if (input.magnitude > 0.5f)
            {
                //Aplica uma for�a na dire��o do movimento no ar
                rb.AddForce(CalculateMovement(sprinting ? sprintSpeed * airControl : walkSpeed * airControl), ForceMode.VelocityChange);
            }
            else
            {
                //Reduz a velocidade no ar
                var velocity1 = rb.velocity;
                velocity1 = new Vector3(velocity1.x * 0.2f * Time.fixedDeltaTime, velocity1.y, velocity1.z * 0.2f * Time.fixedDeltaTime);
            }
        }

        grounded = false;
    }

    Vector3 CalculateMovement(float _speed)
    {
        //Determina a dire��o do movimento
        Vector3 targetVelocity = new Vector3(input.x, 0, input.y);
        targetVelocity = transform.TransformDirection(targetVelocity);

        //Multiplica a dire��o pelo valor da velocidade desejada
        targetVelocity *= _speed;

        //Obt�m a velocidade atual
        Vector3 velocity = rb.velocity;

        if (input.magnitude > 0.5f)
        {
            //Calcula a mudan�a de velocidade necess�ria para atingir a velocidade desejada
            Vector3 velocityChange = targetVelocity - velocity;

            //Limita a mudan�a de velocidade para evitar movimentos bruscos
            velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
            velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);

            velocityChange.y = 0;

            return (velocityChange);
        }
        else
        {
            return new Vector3();
        }
    }
}
