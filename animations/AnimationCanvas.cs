using System.Collections;
using UnityEngine;

/// <summary> �������� �������.</summary>
/// <remarks> ������ ����� ��������� � ��������� ������� � ������� ���������.</remarks>
public class AnimationCanvas : MonoBehaviour
{
    /// <summary> ������ � ������ ��������.</summary>
    [SerializeField] private GameObject canvas;

    /// <summary> ����� ��������.</summary>
    private const float openTime = 0.2f;

    /// <summary> ����� ��������.</summary>
    private const float closeTime = 0.1f;

    /// <summary> ��������� �������.</summary>
    private new Transform transform;

    /// <summary> ����������� ������ �������.</summary>
    private readonly Vector3 minimalSize = new(0.02f, 0.02f, 0.02f);

    /// <summary> �������� �������� �������.</summary>
    private IEnumerator resizeCoroutine;
    
    /// <summary> ��� ������ ������������� ��������� �������.</summary>
    private void Start()
    {
        transform = canvas.transform;
    }

    /// <summary> �������� �� ������.</summary>
    public void OnClickOpen()
    {
        ResizeOpen(openTime, minimalSize);
    }

    /// <summary> �������� �� ������.</summary>
    public void OnClickClose()
    {
        transform = canvas.transform;
        StartCoroutine(ResizeCloseCoroutine(closeTime, transform.localScale));
    }

    /// <summary> ������� ��� �������� ������ ��� �������� �������.</summary>
    /// <param name="time"> ����� ��������.</param>
    /// <param name="size"> ��������� ������ ��������.</param>
    public void ResizeOpen(float time, Vector3 size)
    {
        // ������������� ������, ���� �� ����������
        if (resizeCoroutine != null)
            StopCoroutine(resizeCoroutine);

        // ��������� �������� �������
        Vector3 sizeOpen = canvas.transform.localScale;
        canvas.transform.localScale = size;
        Vector3 startScale = size;

        resizeCoroutine = ResizeCoroutine(time, startScale, sizeOpen);
        canvas.SetActive(true);

        StartCoroutine(resizeCoroutine);
    }

    /// <summary> �������� ��� �������� ������ ��� �������� �������.</summary>
    /// <param name="time"> ����� ��������.</param>
    /// <param name="size"> �������� ������ �������.</param>
    /// <remarks> �������� �����, ����� ������ �� ���������� �� ��������� ��������.</remarks>
    public IEnumerator ResizeCloseCoroutine(float time, Vector3 size)
    {
        // ������������� ������, ���� �� ����������
        if (resizeCoroutine != null)
            StopCoroutine(resizeCoroutine);

        // ��������� �������� �������
        Vector3 startScale = size;
        Vector3 sizeClose = minimalSize;
        resizeCoroutine = ResizeCoroutine(time, startScale, sizeClose);
        yield return(StartCoroutine(resizeCoroutine));

        canvas.SetActive(false);
        canvas.transform.localScale = size;
    }

    /// <summary> �������� ��� ��������� ������� �������.</summary>
    /// <param name="time"> ����� ���������.</param>
    /// <param name="startScale"> ��������� ������ �������.</param>
    /// <param name="target"> �������� ������ �������.</param>
    private IEnumerator ResizeCoroutine(float time, Vector3 startScale, Vector3 target)
    {
        float Timer = 0;
       
        while (Timer < time)
        {
            canvas.transform.localScale = Vector3.Lerp(startScale, target, Timer / time);
            yield return null; // �������� ����� �� ���������� �����
            Timer += Time.deltaTime;
        }
        canvas.transform.localScale = target;
        resizeCoroutine = null;
    }
}
