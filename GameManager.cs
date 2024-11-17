using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject PlayerPrefab;

    [SerializeField] private AsteroidFactory AsteroidFactory;
    [SerializeField] private UIManager uiManager;
    [SerializeField] private AudioSource SourceAudioGameOver;
    [SerializeField] private AudioClip AudioClipGameOver;
    private GameObject CurrentPlayer;
    private PlayerController PlayerController;

    // Start is called before the first frame update
    private const int MaxAsteroids = 5;
    private int curLives = 3;
    private int score = 0;
    private bool isRunning = false; 


    private void InitGame()
    {
        CreatePlayer();
        AsteroidFactory._onAsteroidDestroyed += AsteroidDestroyed; 
        SpawnAsteroids(MaxAsteroids);        
    }

    private void SpawnAsteroids(int count)
    {
        for (var i = 0; i < count; i++)
        {
            var randX = Random.Range(-5f, 5f);
            var randY = Random.Range(-2.5f, 2.5f);
            var randVelocity = new Vector2(Random.Range(-1, 1), Random.Range(-1, 1)).normalized;
            AsteroidFactory.RequestAsteroid(new Vector3(randX,randY,0), randVelocity, AsteroidType.LARGE);
        }              
    }

    void CreatePlayer()
    {
        CurrentPlayer = Instantiate(PlayerPrefab, Vector3.zero, Quaternion.identity);
        PlayerController = CurrentPlayer.GetComponent<PlayerController>();
        PlayerController.isActive = true;
        CurrentPlayer.name = "CurrentPlayer";
    }
    
   

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

    private void AsteroidDestroyed(AsteroidType asteroidType)
    {
        switch (asteroidType)
        {
            case AsteroidType.LARGE:
                score += 1000;
                break;
            case AsteroidType.MEDIUM:
                score += 2000;
                break;
            case AsteroidType.SMALL:
                SpawnAsteroids(1);
                score += 5000; 
                break;
        }
        
        uiManager.UpdateScore(score);
    }


    public void InputActionStartGame(InputAction.CallbackContext context)
    {
        if (context.performed && !isRunning)
        {
            isRunning = true;
            InitGame();            
            uiManager.hideStartGame();
        }

    }
}
