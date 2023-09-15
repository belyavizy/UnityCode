using System.Collections;
using UnityEngine;
using TMPro;
/// <summary> Класс таймер.</summary>
public class CorutineTimer : MonoBehaviour
{
    /// <summary> Переменная для установки времени.</summary>
    [SerializeField] private float time;

    /// <summary> Текстовый объект для отрисовки таймера.</summary>
    [SerializeField] private TextMeshProUGUI timerText;

    /// <summary> Переменная - оставшееся время.</summary>
    private float TimeLeft;

    /// <summary> Гейм-менеджер.</summary>
    GameManager gameManager;

    /// <summary> Корутина начала таймера.</summary>
    private IEnumerator StartTimer()
    {
        while (TimeLeft > 0)
        {
            TimeLeft -= Time.deltaTime;
            UpdateTimeText();
            yield return null;
        }
    }

    /// <summary> Инстанс гейм-менеджера, установка времени и запуск таймера.</summary>
    private void Start()
    {
        gameManager = GameManager.Instance;
        TimeLeft = time;

        StartCoroutine(StartTimer());
    }

    /// <summary> Проверка оставшегося времени и обновление таймера в игре.</summary>
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

    /// <summary> Переход в проигрышное состояние.</summary>
    private void TimeIsNull()
    {
        gameManager.UpdateGameState(GameState.LOOSE);
    }
}
