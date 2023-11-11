using UnityEngine;

[RequireComponent(typeof(CharacterController), typeof(Camera))]
public class PlayerControls : MonoBehaviour
{
    [SerializeField] 
    private float _speed;
    [SerializeField]
    private float _jumpSpeed;
    [SerializeField]
    private float _rotateSpeed;
    [SerializeField]    
    private float _gravity;

    private Vector3 _moveDirection = Vector3.zero;

    private CharacterController _controller;
    private Transform _playerCamera;

    private void Start()
    {
        _controller = GetComponent<CharacterController>();
        _playerCamera = GetComponentInChildren<Camera>().transform;
    }

    void Update()
    {
        transform.Rotate(0, Input.GetAxis("Mouse X") * _rotateSpeed, 0);
        _playerCamera.Rotate(-Input.GetAxis("Mouse Y") * _rotateSpeed, 0, 0);

        if (_playerCamera.localRotation.eulerAngles.y != 0)
        {
            _playerCamera.Rotate(Input.GetAxis("Mouse Y") * _rotateSpeed, 0, 0);
        }

        _moveDirection = new Vector3(Input.GetAxis("Horizontal") * _speed, _moveDirection.y, Input.GetAxis("Vertical") * _speed);
        _moveDirection = transform.TransformDirection(_moveDirection);

        if (_controller.isGrounded)
        {
            if (Input.GetButton("Jump"))
                _moveDirection.y = _jumpSpeed;
            else 
                _moveDirection.y = 0;
        }

        _moveDirection.y -= _gravity * Time.deltaTime;
        _controller.Move(_moveDirection * Time.deltaTime);
    }
}

