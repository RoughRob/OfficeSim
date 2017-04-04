using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    public float walkspeed = 2;
    public float runspeed = 6;
    public float gravity = -12;
    public float jumpHeight = 1;
    [Range(0,1)]
    public float airControlPercent;

    public GameObject Spear;

    public float turnSmoothTime = 0.2f;
    float turnSmoothVelocity;

    public float speedSmoothTime = 0.1f;
    float speedSmoothVelocity;
    float currentSpeed;
    float velocityY;

    Animator animator;
    Transform cameraT;
    CharacterController contorller;
   

    // Use this for initialization
    void Start () {
        animator = GetComponent<Animator>();
        cameraT = Camera.main.transform;
        contorller = GetComponent<CharacterController>();
    }
	
	// Update is called once per frame
	void Update () {
        //input
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw ("Vertical"));
        Vector2 inputDir = input.normalized;
        bool running = Input.GetKey(KeyCode.LeftShift);

        Move(inputDir, running);

        if (Input.GetKeyDown (KeyCode.Space)) {
            Jump();
        }



        // animator
        float animationSpeedPercent = ((running) ? currentSpeed / runspeed : currentSpeed / walkspeed * 1f);
        //1 : 1f) * inputDir.magnitude;// this will need to be changed back when you actually create a run animation ((running) ? 1 : .5f)
        animator.SetFloat("speed%", animationSpeedPercent, speedSmoothTime, Time.deltaTime);

    }

    void Move(Vector2 inputDir, bool running) {

        if (inputDir != Vector2.zero)
        {
            float targetRotation = Mathf.Atan2(inputDir.x, inputDir.y) * Mathf.Rad2Deg + cameraT.eulerAngles.y;
            transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref turnSmoothVelocity, GetModififedSmoothTime(turnSmoothTime));
        }

        float targetSpeed = ((running) ? runspeed : walkspeed) * inputDir.magnitude;
        currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref speedSmoothVelocity, GetModififedSmoothTime(speedSmoothTime));

        velocityY += Time.deltaTime * gravity;

        Vector3 velocity = transform.forward * currentSpeed + Vector3.up * velocityY;

        contorller.Move(velocity * Time.deltaTime);
        currentSpeed = new Vector2(contorller.velocity.x, contorller.velocity.z).magnitude;

        if (contorller.isGrounded)
        {
            velocityY = 0;
        }


   


    }

    void Jump() {
        if (contorller.isGrounded) {
            float jumpVelocity = Mathf.Sqrt(-2 * gravity * jumpHeight);
            velocityY = jumpVelocity;
        }
    }

   

    float GetModififedSmoothTime(float smoothTime) {
        if (contorller.isGrounded) {
            return smoothTime;
        }

        if (airControlPercent == 0) {
            return float.MaxValue;
            }

        return smoothTime / airControlPercent;
    }


}
