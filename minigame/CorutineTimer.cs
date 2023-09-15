using System.Collections;
using UnityEngine;
using TMPro;
/// <summary> ����� ������.</summary>
public class CorutineTimer : MonoBehaviour
{
    /// <summary> ���������� ��� ��������� �������.</summary>
    [SerializeField] private float time;

    /// <summary> ��������� ������ ��� ��������� �������.</summary>
    [SerializeField] private TextMeshProUGUI timerText;

    /// <summary> ���������� - ���������� �����.</summary>
    private float TimeLeft;

    /// <summary> ����-��������.</summary>
    GameManager gameManager;

    /// <summary> �������� ������ �������.</summary>
    private IEnumerator StartTimer()
    {
        while (TimeLeft > 0)
        {
            TimeLeft -= Time.deltaTime;
            UpdateTimeText();
            yield return null;
        }
    }

    /// <summary> ������� ����-���������, ��������� ������� � ������ �������.</summary>
    private void Start()
    {
        gameManager = GameManager.Instance;
        TimeLeft = time;

        StartCoroutine(StartTimer());
    }

    /// <summary> �������� ����������� ������� � ���������� ������� � ����.</summary>
    private void UpdateTimeText()
    {
        if (gameManager.State == GameState.PLAYING)
        {
            if (TimeLeft < 0)
            {
                TimeLeft = 0;
                TimeIsNull();
            }

            float minutes = Mathf.FloorToInt(TimeLeft / 60);
            float seconds = Mathf.FloorToInt(TimeLeft % 60);

            timerText.text = string.Format("{0:00} : {1:00}", minutes, seconds);
        }
    }

    /// <summary> ������� � ����������� ���������.</summary>
    private void TimeIsNull()
    {
        gameManager.UpdateGameState(GameState.LOOSE);
    }
}
