using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary> Усиление и исчезание тени.</summary>
/// <remarks> Данный класс используется как анимация перехода между сценами.</remarks>
public class FadeInOut : MonoBehaviour
{
    /// <summary> Скорость "загрузки" сцены.</summary>
    [SerializeField] private float fadeSpeedStartScene = 2.5f;

    /// <summary> Скорость "закрытия" сцены.</summary>
    [SerializeField] private float fadeSpeedEndScene = 1.5f;

    /// <summary> Плашка для затемнения.</summary>
    private Image image;

    /// <summary> Получение данных о сценах.</summary>
    private void Start()
    {
        image = GetComponent<Image>();
        image.enabled = true;
        //Cursor.visible = false;
        StartCoroutine(StartScene());
    }

    /// <summary> Запуск сцены.</summary>
    /// <returns> Возвращает увеличенное значение прозрачности картинки.</returns>
    public IEnumerator StartScene()
    {
        while (image.color.a > 0.01f)
        {
            image.color = Color.Lerp(image.color, Color.clear, fadeSpeedStartScene * Time.deltaTime);
            yield return null;
        }

        image.color = Color.clear;
        image.enabled = false;
        //Cursor.visible = true;
    }

    /// <summary> Закрытие сцены.</summary>
    /// <param name="nextSceneName"> Название следующей сцены.</param>
    /// <returns> Возвращает уменьшенное значение прозрачности картинки.</returns>
    public IEnumerator EndScene(string nextSceneName)
    {
        image.enabled = true;
        while (image.color.a < 0.95f)
        {
            image.color = Color.Lerp(image.color, Color.black, fadeSpeedEndScene * Time.deltaTime);
            yield return null;
        }

        image.color = Color.black;
        //Cursor.visible = false;
        SceneManager.LoadScene(nextSceneName);
    }
}