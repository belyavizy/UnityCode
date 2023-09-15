using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary> Класс, отвечающий за каждую трубу.</summary>
public class Piece : MonoBehaviour, IPointerClickHandler
{
    /// <summary> Гейм-менеджер.</summary>
    GameManager gameManager;

    /// <summary> Ссылка на паззл, в котором находятся детали.</summary>
    Puzzle puzzler;

    /// <summary> Массив материалов для анимации труб.</summary>
    [SerializeField] private Material[] materialArray;
    /// <summary> Заглушка для вращения.</summary>
    private bool isInteractable = true;

    /// <summary> Массив с выходами трубы.</summary>
    /// <remarks> 0 элемент - вверх; 1 - право; 2 - низ; 3 - лево. Задается через инспектор.</remarks>
    public int[] values;

    /// <summary> Булевая переменная для возможности поворота объекта.</summary>
    public bool IsRotatable = true;

    public bool IsLiquid { get; private set; }////////////////////////////////////////////////////////////////////////////////////////

    /// <summary> Инстанс Гейм-менеджера и паззла.</summary>
    private void Start()
    {
        gameManager = GameManager.Instance;
        puzzler = Puzzle.Instance;
    }

    /// <summary> Корутина поворота спрайта трубы оп часовой стрелке.</summary>
    /// <param name="realRotation"> Вектор, отвечающий за конечное положение.</param>
    /// <returns></returns>
    private IEnumerator Rotation()
    {
        Vector3 realRotation = transform.eulerAngles;
        realRotation.z += 90;
        if (realRotation.z == 360)
            realRotation.z = 0;
        Quaternion startPosition = transform.rotation;
        for (float t = 0; t < 1; t += Time.deltaTime * 3)
        {
            transform.rotation = Quaternion.Slerp(startPosition, Quaternion.Euler(realRotation), t);
            yield return null;
        }
        transform.rotation = Quaternion.Euler(realRotation);
        isInteractable = true;
    }

    /// <summary> Поворот значений выходов трубы по часовой стрелке.</summary>
    private void RotateValues()
    {
        int tmp = values[0];
        for (int i = 0; i < values.Length - 1; i++)
        {
            values[i] = values[i + 1];
        }
        values[3] = tmp;
    }

    /// <summary> Проверка на возможность поворота.</summary>
    /// <returns>Возвращает булевре значение, где true - труба может повернуться, false - труба не поворачивается.</returns>
    private bool IsRotateble()
    {
        if (IsRotatable)
            return true;
        else
            return false;
    }

    /// <summary> Поворот объекта трубы.</summary>
    private void RotatePiece()
    {
        if (IsRotateble())
        {
            StartCoroutine(Rotation());
            RotateValues();
            puzzler.CheckEther();
        }
    }

    /// <summary> Поворт спрайтов при запуске сцены.</summary>
    public void RotatePieceStart()
    {
        Vector3 rotate = transform.eulerAngles;

        if (IsRotateble())
        {
            rotate.z += 90;
            if (rotate.z == 270)
                rotate.z = -90;
            transform.rotation = Quaternion.Euler(rotate);
            RotateValues();
        }
    }

    /// <summary> Изменение спрайта трубы.</summary>
    /// <param name="state">Булевое значение, где true - эфир течет, false - эфир не течет.</param>
    public void ChangePipeMaterial(bool state, GameState gameState)
    {
        switch (gameState)
        {
            case GameState.PLAYING:
                if (state)
                {
                    GetComponent<Image>().material = materialArray[1];
                    IsLiquid = true;
                }
                else
                {
                    GetComponent<Image>().material = materialArray[0];
                    IsLiquid = false;
                }
                break;
            case GameState.LOOSE:
                if (state)
                    GetComponent<Image>().material = materialArray[3];
                
                break;
                
            case GameState.WIN:
                if (state)
                    GetComponent<Image>().material = materialArray[2];
                break;


        }
    }

    /// <summary> Запуск при нажатии на мышь.</summary>
    /// <remarks> Высчитываение текущего значения соединений до и после поворота и поворот самой детали.</remarks>
    void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
    {
        if (isInteractable)
        {
            isInteractable = false;
            int difference = -puzzler.QuickSweep(transform.localPosition.x, transform.localPosition.y);

            RotatePiece();

            difference += puzzler.QuickSweep(transform.localPosition.x, transform.localPosition.y);

            puzzler.CurValue += difference;

            if (puzzler.CurValue == puzzler.WinValue)
                gameManager.UpdateGameState(GameState.WIN);
        }
    }
}

