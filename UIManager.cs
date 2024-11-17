using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private Image[] healthIcons;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI startGame;
    [SerializeField] private TextMeshProUGUI gameOver;
    
    private int curLives = 2;
    void Start()
    {
        curLives = healthIcons.Length;
        hideGameOver();
    }

    private void hideGameOver()
    { 
        gameOver.alpha = 0f;
    }

    private void showGameOver()
    {
        gameOver.alpha = 255f;
    }

    private void showStartGame()
    {
        startGame.alpha = 255f;
    }

    public void hideStartGame()
    {
        startGame.alpha = 0f;
    }

    public void GameOver()
    {
        showGameOver();
    }

    public void StartGame()
    {
        scoreText.text = 0.ToString("D8");
        foreach (var live in healthIcons)
        {
            live.enabled = true;
        }
        hideStartGame();
        hideGameOver();
    }

    public void UpdateScore(int newScore)
    {
        scoreText.text = newScore.ToString("D8");
    }

    public void SubtractLife(int index)
    {
        healthIcons[index].enabled = false;

    }

    // Update is called once per frame

}
