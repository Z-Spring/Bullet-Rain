using System;
using battle_player;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public UnityEvent OnGameOver;


    public enum GameState
    {
        Prepare,
        Playing,
        Pause,
        GameOver
    }

    private GameState gameState;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        // bloodEffect = GetComponent<ParticleSystem>();

        gameState = GameState.Prepare;
        // todo； 这里暂时注释掉，待后面优化
        // Player.Instance.OnPlayerDead.AddListener(() => { gameState = GameState.GameOver; });
    }

    private void Update()
    {
        switch (gameState)
        {
            case GameState.Prepare:
                break;
            case GameState.Playing:
                break;
            case GameState.Pause:
                break;
            case GameState.GameOver:
                OnGameOver.Invoke();
                break;
        }
    }

    public GameState GetGameState()
    {
        return gameState;
    }
}