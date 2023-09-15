using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary> �����, ���������� �� ������ �����.</summary>
public class Piece : MonoBehaviour, IPointerClickHandler
{
    /// <summary> ����-��������.</summary>
    GameManager gameManager;

    /// <summary> ������ �� �����, � ������� ��������� ������.</summary>
    Puzzle puzzler;

    /// <summary> ������ ���������� ��� �������� ����.</summary>
    [SerializeField] private Material[] materialArray;
    /// <summary> �������� ��� ��������.</summary>
    private bool isInteractable = true;

    /// <summary> ������ � �������� �����.</summary>
    /// <remarks> 0 ������� - �����; 1 - �����; 2 - ���; 3 - ����. �������� ����� ���������.</remarks>
    public int[] values;

    /// <summary> ������� ���������� ��� ����������� �������� �������.</summary>
    public bool IsRotatable = true;

    public bool IsLiquid { get; private set; }////////////////////////////////////////////////////////////////////////////////////////

    /// <summary> ������� ����-��������� � ������.</summary>
    private void Start()
    {
        gameManager = GameManager.Instance;
        puzzler = Puzzle.Instance;
    }

    /// <summary> �������� �������� ������� ����� �� ������� �������.</summary>
    /// <param name="realRotation"> ������, ���������� �� �������� ���������.</param>
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

    /// <summary> ������� �������� ������� ����� �� ������� �������.</summary>
    private void RotateValues()
    {
        int tmp = values[0];
        for (int i = 0; i < values.Length - 1; i++)
        {
            values[i] = values[i + 1];
        }
        values[3] = tmp;
    }

    /// <summary> �������� �� ����������� ��������.</summary>
    /// <returns>���������� ������� ��������, ��� true - ����� ����� �����������, false - ����� �� ��������������.</returns>
    private bool IsRotateble()
    {
        if (IsRotatable)
            return true;
        else
            return false;
    }

    /// <summary> ������� ������� �����.</summary>
    private void RotatePiece()
    {
        if (IsRotateble())
        {
            StartCoroutine(Rotation());
            RotateValues();
            puzzler.CheckEther();
        }
    }

    /// <summary> ������ �������� ��� ������� �����.</summary>
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

    /// <summary> ��������� ������� �����.</summary>
    /// <param name="state">������� ��������, ��� true - ���� �����, false - ���� �� �����.</param>
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

    /// <summary> ������ ��� ������� �� ����.</summary>
    /// <remarks> ������������� �������� �������� ���������� �� � ����� �������� � ������� ����� ������.</remarks>
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

