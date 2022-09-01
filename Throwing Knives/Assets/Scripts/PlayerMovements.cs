using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovements : MonoBehaviour
{

    #region My Variables

    [Header("Global References")]
    private InputPlayer inputPlayer;
    private CharacterController characterController;


    [Header("Camera Management")]
    public Transform firstPersonCamera;
    public Transform aimingCamera;
    public GameObject cameraManager;
    [SerializeField] private readonly float sensitivity = 100f;

    private CameraActions cameraActions;
    public bool is3rdPerson = false;


    [Header("Movement Values")]
    [SerializeField] private float speed = 1.5f;
    [SerializeField] private float rotationSmoothTime = 0.1f;
    [SerializeField] private float sprintSpeed = 4f;
    [SerializeField] private float walkingSpeed = 1.5f;


    private float currentSmoothVelocity;

    private Vector2 moveValues;
    private Vector2 lookValues;
    private Vector3 direction;
    private float yRotation;

    private bool isSprinting = false;
    private bool isWalking = false;
    private bool isAiming = false;
    private bool isRecoil = false;
    private bool isRolling = false;


    [Header("Jump Values")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float jumpHeight = 5f;
    [SerializeField] private float gravityScale = 1;
    private float gravity
    {
        get { return 9.81f * gravityScale; }
    }

    public bool IsWalking { get => isWalking; set => isWalking = value; }
    public bool IsSprinting { get => isSprinting; set => isSprinting = value; }
    public bool IsAiming { get => isAiming; set => isAiming = value; }
    public bool IsRecoil { get => isRecoil; set => isRecoil = value; }
    public bool IsRolling { get => isRolling; set => isRolling = value; }


    #endregion

    #region Unity Methods
    //Métodos con la secuencia de unity
    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        inputPlayer = new InputPlayer();
        cameraActions = cameraManager.GetComponent<CameraActions>();
    }
    private void OnEnable()
    {
        //Aqui se encuentran todos los eventos para el movimiento del jugador con el nuevo input system

        inputPlayer.Enable();

        //Saltar
        inputPlayer.Player.Jump.performed += _ => OnJump("Saltar");

        //Mover
        inputPlayer.Player.Move.performed += _ => IsWalking = true;
        inputPlayer.Player.Move.canceled += _ => IsWalking = false;

        //Apuntar
        inputPlayer.Player.Aim.performed += _ => OnAim();
        inputPlayer.Player.Aim.canceled += _ => OnAim();

        //Disparar
        inputPlayer.Player.Fire.performed += _ => IsRecoil = !IsRecoil; ;
        inputPlayer.Player.Fire.canceled += _ => IsRecoil = !IsRecoil; ;

        //Rodar
        inputPlayer.Player.Roll.performed += _ => IsRolling = !IsRolling;
        inputPlayer.Player.Roll.canceled += _ => IsRolling = !IsRolling;    

        //Sprintar
        inputPlayer.Player.Sprint.started += _ => OnSprint();
        inputPlayer.Player.Sprint.canceled += _ => OnSprint();

        //Cambiar de camara
        inputPlayer.Player.Special.performed += _ => SwitchTo3rdPersonCamera();
    }
    private void OnDisable()
    {
        inputPlayer.Disable();
    }
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        cameraActions.SwitchCamera(0);
    }
    private void Update()
    {
        GetInputValues();
        OnMove();
        Rotate();
        ApplyGravity();
    }
    #endregion
   
    #region Basic Movements
    private void OnMove() //Mueve al jugador
    {
        
        //Mueve al jugador cuando está en primera persona o apuntando
        if ((!is3rdPerson || IsAiming) && IsWalking) {

            characterController.Move((transform.right * direction.x + transform.forward * direction.z));

        }

        //Mueve al jugador cuando está en tercera persona
        if (is3rdPerson && !IsAiming && IsWalking)
        {
            direction = new Vector3(moveValues.x, 0f, moveValues.y).normalized;

            if (direction.magnitude >= 0.1f)
            {
                float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + Camera.main.transform.eulerAngles.y; //Esto obtiene el angulo del mouse en el eje X para rotar el personaje
                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref currentSmoothVelocity, rotationSmoothTime); //Suaviza la rotacion del personaje

                transform.rotation = Quaternion.Euler(0f, angle, 0f); //Rota al personaje
                Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

                characterController.Move(moveDir.normalized * speed * Time.deltaTime);


            }
        }


    }
    private void OnSprint()
    {

        if (!IsSprinting) speed = sprintSpeed;

        if (IsSprinting) speed = walkingSpeed;

        //Como el evento es llamado dos veces, se le asigna el valor contrario al booleano
        IsSprinting = !IsSprinting;

    }
    private void Rotate()
    {
        //Obtiene la posicion en la que el mouse se mueve en el eje y
        yRotation -= lookValues.y;
        yRotation = Mathf.Clamp(yRotation, -90f, 90f); //Esto hace que la vista se limite a 90 grados hacia arriba y hacia abajo

        //Aplica la rotacion de la camara cuando está en primera persona
        if (!is3rdPerson)
        {
            firstPersonCamera.localRotation = Quaternion.Euler(yRotation, 0f, 0f);

            transform.Rotate(0f, lookValues.x, 0f);
        }

        //Aplica la rotacion de la camara cuando está en 3era persona y apuntando a la vez
        if (is3rdPerson && IsAiming)
        {
            aimingCamera.localRotation = Quaternion.Euler(yRotation / 2, 0f, 0f);
            lookValues.x /= 2;

            transform.Rotate(0f, lookValues.x, 0f);
        }

    }
    private void OnJump(string test) //Hace saltar al personaje
    {
        Debug.Log(test);
        if (isGrounded()) direction.y = jumpHeight;
   
    } 
    private void OnAim() //Apuntar
    {
        //Hace que el personaje apunte unicamente cuando esté en 3era persona
        if (!IsAiming && is3rdPerson)
        {

            float targetAngle = Mathf.Atan2(lookValues.x, transform.eulerAngles.y) * Mathf.Rad2Deg + Camera.main.transform.eulerAngles.y; //Obtiene el angulo de la posicion del mouse en el eje Y para rotar la camra
            transform.rotation = Quaternion.Euler(0f, targetAngle, 0f); //Rotate the player

            cameraActions.SwitchCamera(2);
        } 

        if (IsAiming && is3rdPerson) cameraActions.SwitchCamera(1);

        IsAiming = !IsAiming;
    }

    #endregion
   
    #region Calls
    private void GetInputValues() //Obtiene los valores con el nuevo input system cuando el jugador presione las teclas
    {
        moveValues = inputPlayer.Player.Move.ReadValue<Vector2>();

        direction.x = moveValues.x * speed * Time.deltaTime;
        direction.z = moveValues.y * speed * Time.deltaTime;

        lookValues = inputPlayer.Player.Look.ReadValue<Vector2>() * sensitivity * Time.deltaTime;

    }
    private bool isGrounded()
    {
        return Physics.CheckSphere(groundCheck.position, .1f, groundLayer);  
    }
    #endregion
    
    #region General
    private void SwitchTo3rdPersonCamera() //Este metodo cambia la vista de las camaras
    {
        if (!is3rdPerson) cameraActions.SwitchCamera(1);

        if (is3rdPerson) cameraActions.SwitchCamera(0);

        is3rdPerson = !is3rdPerson;
    }
    private void ApplyGravity() //Aplica gravedad al personaje
    {
        direction.y -= gravity * Time.deltaTime;

        characterController.Move(Vector3.up * direction.y * Time.deltaTime);

    }
    #endregion

}
