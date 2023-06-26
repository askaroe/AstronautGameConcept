using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonMovement : MonoBehaviour
{
    private Animator _anim;
    
    [SerializeField]
    private CharacterController controller;
    [SerializeField]
    private Transform cam;

    [SerializeField]
    private float _speed = 6.0f;
    [SerializeField]
    private float _jumpForce = 20f;
    [SerializeField]
    private float turnSmoothTime = 0.1f;
    private float turnSmoothVelocity;

    [SerializeField]
    private Transform _groundCheck;
    private float _groundDistance = 0.4f;
    [SerializeField]
    private LayerMask _groundMask;
    private bool _isGrounded;

    private Vector3 _velocity;

    [SerializeField]
    private float _gravity = -9.8f;


    private void Start()
    {
        _anim = gameObject.GetComponentInChildren<Animator>();
    }

    void Update()
    {
        _isGrounded = Physics.CheckSphere(_groundCheck.position, _groundDistance, _groundMask);
        if(_isGrounded && _velocity.y < 0)
        {
            _velocity.y = -2f;
        }

        if (Input.GetKeyDown(KeyCode.Space) && _isGrounded)
        {
            _velocity.y = _jumpForce;
            _anim.SetTrigger("JumpStart");
        }

        if(_anim.GetCurrentAnimatorStateInfo(0).IsName("Jump_loop") && _isGrounded)
        {
            _anim.SetTrigger("JumpEnd");
        }

        _velocity.y += _gravity * Time.deltaTime;
        controller.Move(_velocity * Time.deltaTime);

        Movement();
    }

    void Movement()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        _anim.SetInteger("AnimationPar", (int)(Mathf.Abs(horizontalInput) + Mathf.Abs(verticalInput)));

        Vector3 direction = new Vector3(horizontalInput, 0f, verticalInput);

        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

            controller.Move(moveDirection * _speed * Time.deltaTime);
        }
    }
}
