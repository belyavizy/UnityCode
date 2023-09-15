using System.Collections;
using UnityEngine;

/// <summary> ����� enum ��� ��������� ����.</summary>
public enum GameState
{
    START,
    PLAYING,
    WIN,
    LOOSE
}

/// <summary> �����-�������� ��� ���������� �����������.</summary>
public class GameManager : MonoBehaviour   // TODO �������� ������� ��������� (������ �� ����� � ���������)
{
    /// <summary> �������� GameManager.</summary>
    public static GameManager Instance;

    /// <summary> ����� ����� �������������� ������ ��� �������������� ����������.</summary>
    public bool GenerateRandom;

    /// <summary> ��������� ����.</summary>
    public GameState State { get; private set; }

    /// <summary> ������ ��� ��������� WinState.</summary>
    [SerializeField] private GameObject WinCanvas;

    /// <summary> ������ ��� ��������� LooseState.</summary>
    [SerializeField] private GameObject LooseCanvas;

    /// <summary> ������.</summary>
    [SerializeField] private GameObject timer;

    /// <summary> ������ � �����.</summary>
    [SerializeField] private GameObject puzzle;


    /// <summary> ���������� ������ GameManager.</summary>
    private void Awake()
    {
        Instance = this;
    }

    /// <summary> ������������� ������� � ������������ ��������� � ��������� ���� � �������.</summary>
    private void Start()
    {
        UpdateGameState(GameState.START);

        WinCanvas.SetActive(false);
        LooseCanvas.SetActive(false);
        timer.SetActive(false);

        // ��� �������� 
        LaunchGame();
    }
    /// <summary> ������ ����.</summary>
    private void LaunchGame()
    {
        UpdateGameState(GameState.PLAYING);
        timer.SetActive(true);
        puzzle.SetActive(true);
    }
    /// <summary> ���������� ��������� ����.</summary>
    /// <param name="newState"> ����� ���������.</param>
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

    /// <summary> ��������� ������� � ���������� � ���������.</summary>
    public void Lose()
    {
        
    }

    /// <summary> ��������� ������� � ���������� � ��������.</summary>
    public void Win()
    {
        
    }
}
