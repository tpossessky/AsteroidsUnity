using System;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(Rigidbody2D),typeof(BoxCollider2D))]
public class Projectile : MonoBehaviour
{
    // Start is called before the first frame update
    public bool isActive;
    public bool isFirstFrame = true;
    
    public float speed = 2f;
    private Vector3 globalStartPosition; 
    
    [FormerlySerializedAs("particleSystem")] [SerializeField] private ParticleSystem projParticleSystem;
    [SerializeField] private Rigidbody2D projectileBody;
    [SerializeField] private ScreenBounds screenBounds;
    [SerializeField] private Animator projectileAnimator;
    
    private Vector2 forceVector = Vector2.one;
    private float lifeSpan = 2.5f;    
    private float curLifeSpan;
    private bool isDying; 
    public Action<Collider2D> _projectileCollision;
    private static readonly int AnimatorIsDying = Animator.StringToHash("isDying");

    public Action<GameObject> _onProjectileFinished; 
    
    public void Initialize(Vector3 startPos, float zRot)
    {
        globalStartPosition = startPos;
        isActive = true;
        projectileBody.velocity = Vector2.zero;
        projectileBody.angularVelocity = 0f;
        gameObject.transform.position = startPos;
        projectileBody.transform.position = startPos;
        projectileBody.SetRotation(zRot);

        curLifeSpan = 0f;
        isDying = false;

        projectileAnimator.enabled = true;
        projectileAnimator.SetBool(AnimatorIsDying, false);
        projectileAnimator.Play("Default",0,0);
        
        projParticleSystem.Play();

        CalculateForceVector(zRot);
        projectileBody.AddForce(forceVector, ForceMode2D.Impulse);
    }

    private void Update()
    {
        if (!isActive) 
            return;

        curLifeSpan += Time.deltaTime;

        if (curLifeSpan >= lifeSpan)
        {
            var anim = projectileAnimator.GetCurrentAnimatorStateInfo(0);

            // Trigger the death animation if it hasn't started
            if (!anim.IsTag("Death"))
            {
                isDying = true;
                projectileAnimator.SetBool(AnimatorIsDying, true);
                projParticleSystem.Stop();        
                return; // Exit here to allow animation to play
            }

            // Check if the death animation has finished
            if (anim.IsTag("Death") && anim.normalizedTime >= 1f)
            {
                projectileAnimator.enabled = false;
                isFirstFrame = true;
                projectileAnimator.SetBool(AnimatorIsDying, false);
                _onProjectileFinished?.Invoke(gameObject);
            }
        }
    }

    private void FixedUpdate()
    {
        if(!isActive || isDying)
            return;

        //hack for the first requested projectile where it doesn't spawn properly 
        if (isFirstFrame)
        {
            projectileBody.position = globalStartPosition; 
            isFirstFrame = false;
            globalStartPosition = Vector3.zero; 
        }
        
        
        var position = projectileBody.position;
        if (screenBounds.IsPositionOutOfBounds(position))
        {
            var newPosition = screenBounds.CalculateWrappedPosition(position);
            projectileBody.transform.position = newPosition;
        }
    }


    private void CalculateForceVector(float ZRot)
    {
        var theta = GetPlayerThetaRads(ZRot);
        forceVector = new Vector2(
             speed * Mathf.Sin(theta) * -1,  
             speed * Mathf.Cos(theta)
        );
    }

    private float GetPlayerThetaRads(float angle)
    {
        return angle * Mathf.Deg2Rad;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        _projectileCollision?.Invoke(other);
    }
}
