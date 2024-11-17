using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class AsteroidFactory : MonoBehaviour
{
    [SerializeField] private GameObject Prefab;

    public Action<AsteroidType> _onAsteroidDestroyed;
    
    // Start is called before the first frame update
    private int NumAsteroids = 500;
    
    public void RequestAsteroid(Vector3 position, Vector3 velocity, AsteroidType type)
    {
        
        var asteroid = Instantiate(Prefab, new Vector3(7f,-4.1f,0f), Quaternion.identity);
        var toid = asteroid.GetComponent<Asteroid>();
        toid._onAsteroidHitByProjectile += AsteroidDestroyed;
        toid.Initialize(
            type: type, 
            position: position, 
            startingVelocity: velocity
            );
    }
    
    private void AsteroidDestroyed(AsteroidData asteroidData)
    {
        _onAsteroidDestroyed?.Invoke(asteroidData.type);
        if (asteroidData.type == AsteroidType.SMALL)
        {
            return; 
        }

        //slightly randomize the positions and velocities 
        var position1 = asteroidData.position + 
                        new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), 0);
        var position2 = asteroidData.position + 
                        new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), 0);

        var velocity1 = asteroidData.velocity + 
                        new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0);
        var velocity2 = asteroidData.velocity + 
                        new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0);

        switch (asteroidData.type)
        {
            case AsteroidType.MEDIUM:
                RequestAsteroid(position1, velocity1, AsteroidType.SMALL);
                RequestAsteroid(position2, velocity2, AsteroidType.SMALL);
                break;
            case AsteroidType.LARGE:
                RequestAsteroid(position1, velocity1, AsteroidType.MEDIUM);
                RequestAsteroid(position2, velocity2, AsteroidType.MEDIUM);
                break;
        }
    }

    
}
