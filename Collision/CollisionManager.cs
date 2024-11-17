using UnityEngine;
using UnityEngine.Events;

public class CollisionManager : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private Projectile projectile;
    [SerializeField] private PlayerController player;

    public UnityEvent PlayerCollision;
    public UnityEvent ProjectileCollision;
    
    void Start()
    {
        projectile._projectileCollision += OnProjectileCollision;
    }

    private void OnProjectileCollision(Collider2D col)
    {
    }

    public void OnPlayerCollided()
    {
        PlayerCollision?.Invoke();
    }



    // Update is called once per frame
    void Update()
    {
        
    }
}
