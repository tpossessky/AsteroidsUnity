using System;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Rigidbody2D), typeof(CircleCollider2D))]
public class Asteroid : MonoBehaviour
{
    // Start is called before the first frame update
    private AsteroidType asteroidType;
    private readonly float[] speeds = { 0.5f, 1f, 1.2f };
    private readonly float[] colliderRadi = { 0.6f, 0.33f, 0.14f };
    private readonly float[] SpriteScalers = { 2, 1, 0.5f };
    private readonly float[] AngularVelocityMax = {50, 100, 360};
    public Action<AsteroidData> _onAsteroidHitByProjectile;
    
    
    [SerializeField] private SpriteRenderer AsteroidRenderer;
    [SerializeField] private CircleCollider2D AsteroidCollider;
    [SerializeField] private Rigidbody2D AsteroidRigidBody;
    [SerializeField] private ScreenBounds screenBounds;
    private float curSpeed;
    private bool isActive;
    private float invincibilityTimer = 0.5f;
    private float currentLife;
    private void Start()
    {
        AsteroidRigidBody = GetComponent<Rigidbody2D>();
        AsteroidCollider = GetComponent<CircleCollider2D>();
    }

    public void Initialize(AsteroidType type, Vector3 position, Vector3 startingVelocity)
    {
        asteroidType = type;
        isActive = true;
        // Set random Z rotation
        float randomRotationZ = Random.Range(0, 360f); // Random rotation between 0 and 360 degrees
        transform.rotation = Quaternion.Euler(0, 0, randomRotationZ);
        // Set the asteroid's position
        transform.position = position;
        SetupAsteroid();
        // Apply force based on the starting velocity
        ApplyForceBasedOnVelocity(startingVelocity);
    }

    private void ApplyForceBasedOnVelocity(Vector3 startingVelocity)
    {
        Vector3 velo = startingVelocity;

        // If velocity is zero, assign a constant velocity
        if (startingVelocity == Vector3.zero)
        {
            velo = new Vector3(1, 1, 0);
        }

        // Normalize the velocity to get direction
        Vector2 direction = velo.normalized;

        // Calculate the magnitude
        float magnitude = velo.magnitude * curSpeed;

        // Apply the force
        AsteroidRigidBody.AddForce(direction * magnitude, ForceMode2D.Impulse);
    }

    private void SetupAsteroid()
    {
        float randomRotationZ; 
        switch (asteroidType)
        {
            case AsteroidType.LARGE:
                //apply random spin (angular velo)
                randomRotationZ = Random.Range(AngularVelocityMax[0] * -1f, AngularVelocityMax[0]);
                AsteroidRigidBody.angularVelocity = randomRotationZ;
                //adjust sprite size and collider
                AsteroidRenderer.transform.localScale =
                    new Vector3(SpriteScalers[0], SpriteScalers[0], SpriteScalers[0]);
                AsteroidCollider.radius = colliderRadi[0];
                //adjust speed
                curSpeed = speeds[0];
                break;
            case AsteroidType.MEDIUM:
                randomRotationZ = Random.Range(AngularVelocityMax[1] * -1f, AngularVelocityMax[1]); 
                AsteroidRigidBody.angularVelocity = randomRotationZ;
                AsteroidRenderer.transform.localScale =
                    new Vector3(SpriteScalers[1], SpriteScalers[1], SpriteScalers[1]);
                AsteroidCollider.radius = colliderRadi[1];
                curSpeed = speeds[1];
                break;
            case AsteroidType.SMALL:
                randomRotationZ = Random.Range(AngularVelocityMax[2] * -1f, AngularVelocityMax[2]);
                AsteroidRigidBody.angularVelocity = randomRotationZ;
                AsteroidRenderer.transform.localScale =
                    new Vector3(SpriteScalers[2], SpriteScalers[2], SpriteScalers[2]);
                AsteroidCollider.radius = colliderRadi[2];
                curSpeed = speeds[2];
                break;
        }
    }

    private void FixedUpdate()
    {
        if(!isActive)
            return;

        currentLife += Time.fixedDeltaTime;
        var position = AsteroidRigidBody.position;
        if (screenBounds.IsPositionOutOfBounds(position))
        {
            var newPosition = screenBounds.CalculateWrappedPosition(position);
            AsteroidRigidBody.transform.position = newPosition;
        }
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        
        if (other.CompareTag("Projectile") && currentLife > invincibilityTimer)
        {
            isActive = false; 
            Destroy(gameObject);
            AsteroidRigidBody.velocity = Vector2.zero;
            AsteroidRigidBody.angularVelocity = 0f;
            currentLife = 0f; 
            _onAsteroidHitByProjectile?.Invoke(new AsteroidData(AsteroidRigidBody.position, AsteroidRigidBody.velocity, asteroidType));
        }
    }
}