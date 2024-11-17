# Asteroids
![editedasteroidsclip-ezgif com-video-to-gif-converter](https://github.com/user-attachments/assets/9731532c-f3df-4970-8b69-173d9d2326e5)

## Description
Simpler in overall complexity than SpaceInvaders, but proved to have its own set of challenges, as well as some self-imposed ones.


## A Note about What's Covered

As I move forward in the game dev path, I'm trying not to re-write old information that I've already covered in my earlier games as a lot of it has been very similar. Instead, I like to cover the new topics or methods I've used in each successive game. 

## Movement 

All movement in this game is handled by applying forces to `RigidBody2D` components. Fairly simple until I got to things like handling the asteroids spawning in and slightly randomizing their current velocity and direction based on what their previous velocity/direction was. Ultimately though, this all just came down to some straight forward vector math. 

## Screen Wrapping 
The `ScreenBounds` class handles setting up the screen and all GameObjects can request information on whether or not they're moving out of bounds and if so, where they should "teleport" to instead. This class I had to follow a few guides on for the logic in handling dynamically adjusting the size of the screen based on the camera.

## Projectiles/Object Pooling

For the projectiles this time around, I decided to try out the method of object-pooling. At the start of the game, a calculated amount of projectiles is spawned in (based on their lifespan and the player shooting cooldown). When the player wants to shoot, a Projectile is popped off the stack of inactives and placed at the correct orientation and position for where the player requested it. Once the projectile's lifecycle is completed, it is reset and returned to the stack to be used again when needed.

```
    private void Start()
    {
        defaultPosition = Prefab.transform.position;
        if (Prefab == null) 
            return;
        for (var i = 0; i < MaxProjectiles; i++)
        {
            var newProj = Instantiate(Prefab);
            newProj.name = $"Projectile{i}";
            newProj.transform.position = defaultPosition;
            newProj.GetComponent<Projectile>()._onProjectileFinished += ReturnProjectileToPool; 
            
            newProj.SetActive(false);
            inactiveObjects.Push(newProj);
        }
        Prefab.SetActive(false);
    }

    public void RequestProjectile(Vector3 position, float zRotDeg)
    {
        var newProjectile = GrabProjectileFromPool();

        newProjectile.SetActive(true);
        newProjectile.GetComponent<Projectile>().Initialize(position,zRotDeg);
    }

    private GameObject GrabProjectileFromPool()
    {
        while (inactiveObjects.Count > 0)
        {
            var obj = inactiveObjects.Pop();

            if (obj != null)
            {
                return obj; 
            }
        }
        return null;
    }

    private void ReturnProjectileToPool(GameObject gameObj)
    {
        if (gameObj == null) 
            return;
        gameObj.transform.position = defaultPosition;
        gameObj.SetActive(false);
        inactiveObjects.Push(gameObj);
    }
```

While a small arcade game that's only spawning a maximum of 20 projectiles doesn't really benefit from something like this, each game I develop I try to implement new techniques to learn more about game development. 

## Animation

Unrelated to the programming side, I implemented some animations for both the `Player` and `Projectile`. The Player uses an Animation Blend Tree to move from states of idle, accelerating, and full speed. Within the `PlayerController` class, in every frame the animator is told the current velocity based on some vector calculations and then handles the animations based on thresholds I set. 

```
var vectorVelo = Mathf.Sqrt(Mathf.Pow(playerRigidBody.velocity.x, 2) + Mathf.Pow(playerRigidBody.velocity.y,2));
playerAnimator.SetFloat(Animator_Speed, vectorVelo);
```
To transition to the "death" state, another animator variable is used 
```
playerAnimator.SetBool(Animator_IsDying, true);
```
In addition, a separate variable is used to handle the logic within the player itself for death. 

## Spawning the Player

This time around, I went for a more dynamic approach than previously when spawning the player. In previous games, I would just reset the position and sprite whereas now, I completely destroy the player object and instantiate a new one at the starting coords (0,0). This is managed through the `GameManager` class which is alerted by the player when it is dying. 

```
    public void PlayerDeath()
    {
        CurrentPlayer = null;
        if (curLives == 0)
        {
            uiManager.GameOver();
            isRunning = false;
        }
        else
        {
            CreatePlayer();
            curLives--;
            uiManager.SubtractLife(curLives);
        }

        PlayerController = null;
    }
    void CreatePlayer()
    {
        CurrentPlayer = Instantiate(PlayerPrefab, Vector3.zero, Quaternion.identity);
        PlayerController = CurrentPlayer.GetComponent<PlayerController>();
        PlayerController.isActive = true;
        CurrentPlayer.name = "CurrentPlayer";
    }
```
