using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary> �������� � ��������� ����.</summary>
/// <remarks> ������ ����� ������������ ��� �������� �������� ����� �������.</remarks>
public class FadeInOut : MonoBehaviour
{
    /// <summary> �������� "��������" �����.</summary>
    [SerializeField] private float fadeSpeedStartScene = 2.5f;

    /// <summary> �������� "��������" �����.</summary>
    [SerializeField] private float fadeSpeedEndScene = 1.5f;

    /// <summary> ������ ��� ����������.</summary>
    private Image image;

    /// <summary> ��������� ������ � ������.</summary>
    private void Start()
    {
        image = GetComponent<Image>();
        image.enabled = true;
        //Cursor.visible = false;
        StartCoroutine(StartScene());
    }

    /// <summary> ������ �����.</summary>
    /// <returns> ���������� ����������� �������� ������������ ��������.</returns>
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

    /// <summary> �������� �����.</summary>
    /// <param name="nextSceneName"> �������� ��������� �����.</param>
    /// <returns> ���������� ����������� �������� ������������ ��������.</returns>
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