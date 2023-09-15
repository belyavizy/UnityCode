using System.Collections;
using UnityEngine;

/// <summary> Класс enum для состояний игры.</summary>
public enum GameState
{
    START,
    PLAYING,
    WIN,
    LOOSE
}

/// <summary> Класс-менеджер для управления состояниями.</summary>
public class GameManager : MonoBehaviour   // TODO добавить систему сообщений (статья на хабре в закладках)
{
    /// <summary> Создание GameManager.</summary>
    public static GameManager Instance;

    /// <summary> Выбор между автогенерацией уровня или предварительно созданными.</summary>
    public bool GenerateRandom;

    /// <summary> Состояние игры.</summary>
    public GameState State { get; private set; }

    /// <summary> Объект для отрисовки WinState.</summary>
    [SerializeField] private GameObject WinCanvas;

    /// <summary> Объект для отрисовки LooseState.</summary>
    [SerializeField] private GameObject LooseCanvas;

    /// <summary> Таймер.</summary>
    [SerializeField] private GameObject timer;

    /// <summary> Канвас с игрой.</summary>
    [SerializeField] private GameObject puzzle;


    /// <summary> Инициирует объект GameManager.</summary>
    private void Awake()
    {
        Instance = this;
    }

    /// <summary> Устанавливает канвасы в неработующее состояние и состояние игры в игровое.</summary>
    private void Start()
    {
        UpdateGameState(GameState.START);

        WinCanvas.SetActive(false);
        LooseCanvas.SetActive(false);
        timer.SetActive(false);

        // для проверки 
        LaunchGame();
    }
    /// <summary> Запуск игры.</summary>
    private void LaunchGame()
    {
        UpdateGameState(GameState.PLAYING);
        timer.SetActive(true);
        puzzle.SetActive(true);
    }
    /// <summary> Обновление состояния игры.</summary>
    /// <param name="newState"> Новое состояние.</param>
    public void UpdateGameState(GameState newState)
    {
        State = newState;

        switch (newState)
        {
            case GameState.PLAYING:
                break;
            case GameState.WIN:
                Win();
                break;
            case GameState.LOOSE:
                Lose();
                break;
        }
    }

    /// <summary> Включение канваса с сообщением о проигрыше.</summary>
    public void Lose()
    {
        
    }

    /// <summary> Включение канваса с сообщением о выигрыше.</summary>
    public void Win()
    {
        
    }
}
