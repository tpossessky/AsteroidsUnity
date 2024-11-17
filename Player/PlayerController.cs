using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D playerRigidBody;

    [SerializeField] private ScreenBounds screenBounds;
    [SerializeField] private ProjectileFactory projectileFactory;
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private CollisionManager collisionManager;
    [SerializeField] private AudioSource playerAudio;
    [SerializeField] private AudioClip AudioShotClip;
    [SerializeField] private AudioClip AudioDeathClip;
    private float rotationSpeed = 200f;
    private int rotationDirection = 0;
    
    private int thrust;
    private float movementSpeed = 15f;
    float maxVelocity = 3.5f;

    private float maxShootTimer = 0.25f;
    private float curShootTimer;
    private bool shotCooldown;
    
    private bool isDying;
    public bool isActive; 
    
    private static readonly int Animator_IsDying = Animator.StringToHash("isDying");
    private static readonly int Animator_Speed = Animator.StringToHash("Speed");

    private readonly float invincibilityTimer = 1.5f;
    private float curLife; 
    void Start()
    {
        playerAnimator.SetBool(Animator_IsDying, false);
        playerRigidBody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isActive)
            return;
        if (isDying)
        {
            var animState = playerAnimator.GetCurrentAnimatorStateInfo(0);
            if (animState.IsTag("PlayerDeath") && animState.normalizedTime > 1)
            {
                playerAnimator.SetBool(Animator_IsDying, false);
                collisionManager.OnPlayerCollided();
                Destroy(gameObject);
            }
            return;
        }
        
        if(curLife < invincibilityTimer)
            curLife += Time.deltaTime;

        
        if (shotCooldown)
        {
            curShootTimer += Time.deltaTime;
            if (curShootTimer >= maxShootTimer)
            {
                shotCooldown = false;
                curShootTimer = 0f;
            }
               
        }
    }

    private void FixedUpdate()
    {
        if(!isActive || isDying)
            return;

        RotatePlayer();
        MovePlayer();
        var position = playerRigidBody.position;
        if (screenBounds.IsPositionOutOfBounds(position))
        {
            var newPosition = screenBounds.CalculateWrappedPosition(position);
            playerRigidBody.transform.position = newPosition;
        }
      
    }

    private void RotatePlayer()
    {
        var rotationalMovement = rotationSpeed * rotationDirection; 
        playerRigidBody.MoveRotation(playerRigidBody.rotation + rotationalMovement * Time.fixedDeltaTime);        
    }

    private void MovePlayer()
    {
        if (thrust != 0)
        {
            var theta = GetPlayerThetaRads();
            
            var forceVector = new Vector2(
                thrust * movementSpeed * Mathf.Sin(theta) * -1,  
                thrust * movementSpeed * Mathf.Cos(theta)
            );

            playerRigidBody.AddForce(forceVector);
        }
        
        Vector2 velocity = playerRigidBody.velocity;

        if (velocity.magnitude > maxVelocity)
        {
            playerRigidBody.velocity = velocity.normalized * maxVelocity; // Scale velocity to max
        }
        var vectorVelo = Mathf.Sqrt(Mathf.Pow(playerRigidBody.velocity.x, 2) + Mathf.Pow(playerRigidBody.velocity.y,2));
        playerAnimator.SetFloat(Animator_Speed, vectorVelo);
    }

    
    public void HandlePlayerMovementInput(InputAction.CallbackContext callbackContext)
    {
        if (!isActive)
            return;
        
        //x vals for rotation
        //y vals for add force in forward direction
        if (callbackContext.performed)
        {
            var input = callbackContext.ReadValue<Vector2>();
            
            if (input.x != 0)
                rotationDirection = input.x > 0 ? 1 : -1;
            else
                rotationDirection = 0;            
            
            thrust = Mathf.RoundToInt(input.y);
        }
        else
        {
            rotationDirection = 0;
            thrust = 0; 
        }
    }

    public void HandlePlayerShoot(InputAction.CallbackContext callbackContext)
    {
        if(!isActive)
            return;
        if (callbackContext.performed && !shotCooldown)
        {
            var playerTransform = playerRigidBody.transform;
            projectileFactory.RequestProjectile(playerTransform.position, playerTransform.eulerAngles.z);
            playerAudio.PlayOneShot(AudioShotClip, 0.5f);
            shotCooldown = true;
        }
    }


    private float GetPlayerThetaRads()
    {
       return playerRigidBody.transform.eulerAngles.z * Mathf.Deg2Rad;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(!isActive)
            return;

        if(other.gameObject.CompareTag("Bounds") || other.gameObject.CompareTag("Untagged"))
            return;
        if (!shotCooldown && curLife > invincibilityTimer)
        {
            playerAudio.PlayOneShot(AudioDeathClip, 0.5f);

            playerAnimator.SetBool(Animator_IsDying, true);
            isDying = true;
            curLife = 0f; 
        }

    }
}
