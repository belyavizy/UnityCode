using System.Collections;
using UnityEngine;

/// <summary> Анимация канваса.</summary>
/// <remarks> Данный класс открывает и закрывает вкладки с плавной анимацией.</remarks>
public class AnimationCanvas : MonoBehaviour
{
    /// <summary> Объект с нужным канвасом.</summary>
    [SerializeField] private GameObject canvas;

    /// <summary> Время открытия.</summary>
    private const float openTime = 0.2f;

    /// <summary> Время закрытия.</summary>
    private const float closeTime = 0.1f;

    /// <summary> Трансформ объекта.</summary>
    private new Transform transform;

    /// <summary> Минимальный размер объекта.</summary>
    private readonly Vector3 minimalSize = new(0.02f, 0.02f, 0.02f);

    /// <summary> Итератор корутины ресайза.</summary>
    private IEnumerator resizeCoroutine;
    
    /// <summary> При старте устанавливаем трансформ объекта.</summary>
    private void Start()
    {
        transform = canvas.transform;
    }

    /// <summary> Открытие по кнопке.</summary>
    public void OnClickOpen()
    {
        ResizeOpen(openTime, minimalSize);
    }

    /// <summary> Закрытие по кнопке.</summary>
    public void OnClickClose()
    {
        transform = canvas.transform;
        StartCoroutine(ResizeCloseCoroutine(closeTime, transform.localScale));
    }

    /// <summary> Функция для передачи данных для открытия канваса.</summary>
    /// <param name="time"> Время открытия.</param>
    /// <param name="size"> Начальный размер открытия.</param>
    public void ResizeOpen(float time, Vector3 size)
    {
        // останавливаем ресайз, если он происходит
        if (resizeCoroutine != null)
            StopCoroutine(resizeCoroutine);

        // запускаем корутину ресайза
        Vector3 sizeOpen = canvas.transform.localScale;
        canvas.transform.localScale = size;
        Vector3 startScale = size;

        resizeCoroutine = ResizeCoroutine(time, startScale, sizeOpen);
        canvas.SetActive(true);

        StartCoroutine(resizeCoroutine);
    }

    /// <summary> Корутина для передачи данных для закрытия канваса.</summary>
    /// <param name="time"> Время закрытия.</param>
    /// <param name="size"> Исходный размер канваса.</param>
    /// <remarks> Корутина нужна, чтобы канвас не выключился до окончания закрытия.</remarks>
    public IEnumerator ResizeCloseCoroutine(float time, Vector3 size)
    {
        // останавливаем ресайз, если он происходит
        if (resizeCoroutine != null)
            StopCoroutine(resizeCoroutine);

        // запускаем корутину ресайза
        Vector3 startScale = size;
        Vector3 sizeClose = minimalSize;
        resizeCoroutine = ResizeCoroutine(time, startScale, sizeClose);
        yield return(StartCoroutine(resizeCoroutine));

        canvas.SetActive(false);
        canvas.transform.localScale = size;
    }

    /// <summary> Корутина для изменения размера канваса.</summary>
    /// <param name="time"> Время изменения.</param>
    /// <param name="startScale"> Начальный размер канваса.</param>
    /// <param name="target"> Конечный размер канваса.</param>
    private IEnumerator ResizeCoroutine(float time, Vector3 startScale, Vector3 target)
    {
        float Timer = 0;
       
        while (Timer < time)
        {
            canvas.transform.localScale = Vector3.Lerp(startScale, target, Timer / time);
            yield return null; // задержка цикла до следующего кадра
            Timer += Time.deltaTime;
        }
        canvas.transform.localScale = target;
        resizeCoroutine = null;
    }
}
