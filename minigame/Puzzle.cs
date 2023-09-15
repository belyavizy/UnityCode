using System.Collections.Generic;
using UnityEngine;

/// <summary> ����� �����.</summary>
/// <remarks> �������� �� �������� � ����������� �����������.</remarks>
public class Puzzle : MonoBehaviour
{
    /// <summary> ����-��������.</summary>
    GameManager gameManager;

    /// <summary> ������� ������.</summary>
    public static Puzzle Instance;

    /// <summary> ������� �� ����.</summary>
    public Piece[,] Pieces { get; private set; }
    public Piece[,] NonPlayablePieces { get; private set; }
    /// <summary> ���������� ���������� ���� ��� ���������� ��������.</summary>
    public int WinValue { get; private set; }

    /// <summary> ���������� ������� ���������� ����.</summary>
    public int CurValue { get; set; }

    /// <summary> ������ �������� ����.</summary>
    /// <remarks> �������� � ����������.</remarks>
    public GameObject[] PiecePrefabs;

    /// <summary> ������� ��� ������.</summary>
    [SerializeField] private GameObject gameField;

    /// <summary> ������ ������� ������.</summary>
    [SerializeField] private int Width;

    /// <summary> ������ ������� ������.</summary>
    [SerializeField] private int Height;

    /// <summary> ��������� �����������.</summary>
    /// <remarks> ����������� ��������� 0 �� ����� �����.</remarks>
    [SerializeField] private int randomProbability = 40;

    /// <summary> ������������ � ������ ����� � ������.</summary>
    private Vector2 startPiece;

    /// <summary> ��������� ����� ������� UpdateState </summary>
    private bool UpdatedState = false;

    /// <summary> ��������������� ����� ������ ��� ��������� ������.</summary>
    [System.Serializable]
    public class PuzzleGeneration
    {
        public PieceGeneration[,] Pieces;
    }

    /// <summary> ��������������� ����� ����� ��� ��������� ������.</summary>
    [System.Serializable]
    public class PieceGeneration
    {
        public int TypePiece = 0;
        public int[] values = { 0, 0, 0, 0 };
    }

    /// <summary> ���������� ���������������� ������ .</summary>
    public PuzzleGeneration puzzleGeneration;

    /// <summary> ���������� ������ Puzzle.</summary>
    private void Awake()
    {
        Instance = this;
    }

    /// <summary> �����.</summary>
    private void Start()
    {
        gameManager = GameManager.Instance;
        StartGame();
    }

    /// <summary> �������� ������.</summary>
    private void StartGame()
    {
        // �������� �� ��������� ������
        if (gameManager.GenerateRandom)
        {
            if (Width < 2 || Height < 2)
            {
                print("Please check the dimensions");
                Debug.Break();
            }
            else
            {

                GeneratePuzzle();
                InstantiatePuzzle();

                WinValue = GetWinValue();

                Shuffle();

                CurValue = Sweep();


                startPiece = GenerateStartPiece();
                if (startPiece.x > (int)startPiece.x + 0.5)
                    startPiece.x = (int)startPiece.x + 1;
                if (startPiece.y > (int)startPiece.y + 0.5)
                    startPiece.y = (int)startPiece.y + 1;

                CheckEther();

            }
        }
        else
        {
            Vector2 dimensions = CheckDimensions();

            Width = (int)dimensions.x;
            Height = (int)dimensions.y;
            //Debug.Log(dimensions.x);

            Pieces = new Piece[Width, Height];

            foreach (var piece in GameObject.FindGameObjectsWithTag("Piece"))
            {
                Pieces[(int)piece.transform.position.x, (int)piece.transform.position.y] = piece.GetComponent<Piece>();
            }
            if (Pieces.Length > 1)
            {
                print(Pieces.Length);
                WinValue = GetWinValue();

                Shuffle();

                CurValue = Sweep();

                CheckEther();
            }

        }
    }

    /// <summary> �������� �� state.</summary>
    private void Update()
    {
        if ((gameManager.State == GameState.LOOSE || gameManager.State == GameState.WIN) && !UpdatedState)
            UpdateState();
    }

    private void UpdateState()
    {
        UpdatedState = true;
        foreach (var piece in Pieces)
        {
            piece.IsRotatable = false;
            if (gameManager.State == GameState.LOOSE)
                piece.ChangePipeMaterial(piece.IsLiquid, GameState.LOOSE);
            if (gameManager.State == GameState.WIN)
                piece.ChangePipeMaterial(piece.IsLiquid, GameState.WIN);
        }
    }


    /// <summary> ��������� ������ � ������� ���������������� ������.</summary>
    private void GeneratePuzzle() //TODO ������������ ������� ���������������� ������� 
    {
        // ������������� ���������������� ������
        puzzleGeneration.Pieces = new PieceGeneration[Width, Height];

        for (int i = 0; i < Width; i++)
            for (int j = 0; j < Height; j++)
                puzzleGeneration.Pieces[i, j] = new PieceGeneration();
        int attempts = 0;
        // ��������� ������
        do
        {
            for (int h = 0; h < Height; h++)
            {
                for (int w = 0; w < Width; w++)
                {
                    // 1 ����� ������ ����� ���� �����
                    if (h == 0 && w == 0)
                    {
                        puzzleGeneration.Pieces[w, h].values[0] = 1;
                        puzzleGeneration.Pieces[w, h].values[1] = 1;
                        puzzleGeneration.Pieces[w, h].values[2] = 0;
                        puzzleGeneration.Pieces[w, h].values[3] = 0;
                    }
                    else
                    {
                        // ����������� �� ������ 
                        if (w == 0)
                            puzzleGeneration.Pieces[w, h].values[3] = 0;
                        else
                            puzzleGeneration.Pieces[w, h].values[3] = puzzleGeneration.Pieces[w - 1, h].values[1];

                        if (w == Width - 1)
                            puzzleGeneration.Pieces[w, h].values[1] = 0;
                        else
                            puzzleGeneration.Pieces[w, h].values[1] = RandomProbability();


                        // ����������� �� ������
                        if (h == 0)
                            puzzleGeneration.Pieces[w, h].values[2] = 0;
                        else
                            puzzleGeneration.Pieces[w, h].values[2] = puzzleGeneration.Pieces[w, h - 1].values[0];

                        if (h == Height - 1)
                            puzzleGeneration.Pieces[w, h].values[0] = 0;
                        else
                            puzzleGeneration.Pieces[w, h].values[0] = RandomProbability();
                    }

                    CheckPieceType(w, h);
                }
            }
            attempts++;
            // ���� ����� ��������� ����� � ���������� ������ ��������� > ��������� ��������
        } while ((CheckFirstPiece() || SumNullPiece() > Width * Height * 0.2 + 1 || CheckPuzzle()));/////////////////////////////////////////// TODO ��������� �������� � CheckPuzzle

        //|| CheckPuzzle()
        print("attempts=" + attempts);
    }

    /// <summary> �������� ���� �����.</summary>
    /// <param name="w"> ���������� �� ������.</param>
    /// <param name="h"> ���������� �� ������.</param>
    private void CheckPieceType(int w, int h)
    {
        // ������������ ���� �� ������ ���������� �������
        puzzleGeneration.Pieces[w, h].TypePiece = puzzleGeneration.Pieces[w, h].values[0] + puzzleGeneration.Pieces[w, h].values[1]
                        + puzzleGeneration.Pieces[w, h].values[2] + puzzleGeneration.Pieces[w, h].values[3];

        // ����� ���� ��� l-�������� � c-�������� �����
        if (puzzleGeneration.Pieces[w, h].TypePiece == 2 && puzzleGeneration.Pieces[w, h].values[0] != puzzleGeneration.Pieces[w, h].values[2])
            puzzleGeneration.Pieces[w, h].TypePiece = 5;

        // ���� �������� ��������� ����� -> ���������� � ���� � ������������ �������
        if (puzzleGeneration.Pieces[w, h].TypePiece == 1)
        {
            puzzleGeneration.Pieces[w, h].TypePiece = 0;
            for (int i = 0; i < 4; i++)
                puzzleGeneration.Pieces[w, h].values[i] = 0;
            if (w != 0)
            {
                if (puzzleGeneration.Pieces[w - 1, h].TypePiece != 0 && puzzleGeneration.Pieces[w - 1, h].values[1] != 0)
                {
                    puzzleGeneration.Pieces[w - 1, h].values[1] = 0;
                    puzzleGeneration.Pieces[w - 1, h].TypePiece -= 1;
                    CheckPieceType(w - 1, h);
                }
            }

            if (h != 0)
            {
                if (puzzleGeneration.Pieces[w, h - 1].TypePiece != 0 && puzzleGeneration.Pieces[w, h - 1].values[0] != 0)
                {
                    puzzleGeneration.Pieces[w, h - 1].TypePiece -= 1;
                    puzzleGeneration.Pieces[w, h - 1].values[0] = 0;
                    CheckPieceType(w, h - 1);
                }
            }
        }
    }

    /// <summary> �������� �� ���������� ��������� ����.</summary>
    /// <returns> ���������� ������� ��������.</returns>
    private bool CheckFirstPiece()
    {
        for (int h = 0; h < Height; h++)
        {
            for (int w = 0; w < Width; w++)
            {
                if (puzzleGeneration.Pieces[w, h].TypePiece == 1)
                    return true;
                if (w != 0)
                    if (puzzleGeneration.Pieces[w, h].values[3] != puzzleGeneration.Pieces[w - 1, h].values[1])
                        return true;
                if (h != 0)
                    if (puzzleGeneration.Pieces[w, h].values[2] != puzzleGeneration.Pieces[w, h - 1].values[0])
                        return true;
            }
        }
        return false;
    }

    /// <summary> ������� �������/��������� ����.</summary>
    /// <param name="state"> True: ���������� ������� �����; false: ���������� ��������� �����.</param>
    /// <returns>���������� ���������� ����.</returns>
    private int SumNullPiece()
    {
        int sumNull = 0;
        for (int h = 0; h < Height; h++)
        {
            for (int w = 0; w < Width; w++)
            {
                if (puzzleGeneration.Pieces[w, h].TypePiece == 0)
                    sumNull++;
            }
        }
        return sumNull;
    }

    private int SumNotNullPiece()
    {
        int sumNotNull = 0;
        for (int h = 0; h < Height; h++)
        {
            for (int w = 0; w < Width; w++)
            {
                if (puzzleGeneration.Pieces[w, h].TypePiece != 0)
                    sumNotNull++;
            }
        }
        return sumNotNull;
    }

    /// <summary> �������� ������ �� ���������.</summary>
    /// <returns>True: ��� ��; False: ���� ��������� �����.</returns>
    private bool CheckPuzzle()
    {
        Dictionary<PieceGeneration, bool> ExploredPipes = new();
        Dictionary<int, Vector2> NotNullPiece = DictNotNullPiece();
        CheckPath((int)NotNullPiece[0].x, (int)NotNullPiece[0].y, ExploredPipes);
        if (ExploredPipes.Count != SumNotNullPiece())
            return true;

        return false;
    }


    /// <summary> ����������� ������� ���������� ���� ����� ��� ��������� ��������� ���������.</summary>
    /// <param name="w"> ���������� �� ������.</param>
    /// <param name="h"> ���������� �� ������.</param>
    /// <param name="ExploredrPipes"> ���� � ���������� �������.</param>
    private void CheckPath(int w, int h, Dictionary<PieceGeneration, bool> ExploredPipes)
    {
        int fool = 4;

        if (ExploredPipes.ContainsKey(puzzleGeneration.Pieces[w, h]))
            return;  // ������ �� ������
        else
            ExploredPipes.Add(puzzleGeneration.Pieces[w, h], false);
        // ��������� ������
        if (w != Width - 1)
            if (puzzleGeneration.Pieces[w, h].values[1] == 1 && puzzleGeneration.Pieces[w + 1, h].values[3] == 1)
            {
                ExploredPipes[puzzleGeneration.Pieces[w, h]] = true;
                CheckPath(w + 1, h, ExploredPipes);
            }
            else fool--;

        // ��������� ������
        if (h != Height - 1)
            if (puzzleGeneration.Pieces[w, h].values[0] == 1 && puzzleGeneration.Pieces[w, h + 1].values[2] == 1)
            {
                ExploredPipes[puzzleGeneration.Pieces[w, h]] = true;
                CheckPath(w, h + 1, ExploredPipes);
            }
            else fool--;

        // ��������� �����
        if (h != 0)
            if (puzzleGeneration.Pieces[w, h].values[2] == 1 && puzzleGeneration.Pieces[w, h - 1].values[0] == 1)
            {
                ExploredPipes[puzzleGeneration.Pieces[w, h]] = true;
                CheckPath(w, h - 1, ExploredPipes);
            }
            else fool--;

        // ��������� �����
        if (w != 0)
            if (puzzleGeneration.Pieces[w, h].values[3] == 1 && puzzleGeneration.Pieces[w - 1, h].values[1] == 1)
            {
                ExploredPipes[puzzleGeneration.Pieces[w, h]] = true;
                CheckPath(w - 1, h, ExploredPipes);
            }
            else fool--;

        if (fool == 0)
            ExploredPipes[puzzleGeneration.Pieces[w, h]] = false;
    }


    /// <summary> ��������� ������.</summary>
    /// <returns> ���������� 0 � ��������� ����������� (randomProbability) �������.</returns>
    private int RandomProbability()
    {
        int rnd = Random.Range(1, 101);
        if (rnd < randomProbability)
            return 0;
        else return 1;
    }

    /// <summary> �������� ������.</summary>
    private void InstantiatePuzzle()
    {
        Pieces = new Piece[Width, Height];

        // ��������� � ������ ���������� ����
        for (int h = 0; h < Height; h++)
        {
            for (int w = 0; w < Width; w++)
            {
                // ��������������� ������ ��� �������� ������
                GameObject go = (GameObject)Instantiate(PiecePrefabs[puzzleGeneration.Pieces[w, h].TypePiece], CalculatePosition(w, h), Quaternion.identity);

                go.transform.SetParent(gameField.transform, false);

                go.transform.localScale *= (CalculateMultyplier() / 100);
                // ������������� �����, ���� �������� ������� ������� �� �������� � ���������� �����.
                while (go.GetComponent<Piece>().values[0] != puzzleGeneration.Pieces[w, h].values[0] ||
                      go.GetComponent<Piece>().values[1] != puzzleGeneration.Pieces[w, h].values[1] ||
                      go.GetComponent<Piece>().values[2] != puzzleGeneration.Pieces[w, h].values[2] ||
                      go.GetComponent<Piece>().values[3] != puzzleGeneration.Pieces[w, h].values[3])
                {
                    go.GetComponent<Piece>().RotatePieceStart();
                }
                Pieces[w, h] = go.GetComponent<Piece>();
            }
        }
    }


    /// <summary> ��������� ��������� ��� ����������� �������� ���� ������������ �������� ������ 1920*1080.</summary>
    /// <returns> ���������� ���������.</returns>
    private float CalculateMultyplier()
    {

        Rect gameFieldRect = gameField.GetComponent<RectTransform>().rect;
        //print("height " + gameFieldRect.height + " width " + gameFieldRect.width);
        if (gameFieldRect.height / (float)Height < gameFieldRect.width / (float)Width)
            return gameFieldRect.height / (float)Height;
        else
            return gameFieldRect.width / (float)Width;
    }

    /// <summary> ��������� ������� �����. </summary>
    /// <param name="w"> �������� �� ������ � �������.</param>
    /// <param name="h"> ������� �� ������ � �������.</param>
    /// <returns> ���������� 3D ������ � ��������.</returns>
    private Vector3 CalculatePosition(int w, int h)
    {
        int scaler = (int)(CalculateMultyplier());
        int x;
        int y;

        w *= scaler;
        h *= scaler;
        if (Width % 2 == 0)
            x = scaler * (Width / 2) - scaler / 2;
        else
            x = scaler * (Width / 2);

        if (Height % 2 == 0)
            y = scaler * (Height / 2) - scaler / 2;
        else
            y = scaler * (Height / 2);

        w -= x;
        h -= y;

        return new Vector3(w, h, 0);
    }
    /// <summary> ���������� ���������� � ������� puzzle ������������ ����������� �� ������.</summary>
    /// <param name="w"> ���������� �� ������.</param>
    /// <param name="h"> ���������� �� ������.</param>
    /// <returns> ���������� 3D ������ c ��������.</returns>
    private Vector3 CalculateArrayPosition(float w, float h)
    {
        float scaler = CalculateMultyplier();

        float x;
        float y;
        if (Width % 2 == 0)
            x = scaler * (Width / 2) - scaler / 2;
        else
            x = scaler * (Width / 2);

        if (Height % 2 == 0)
            y = scaler * (Height / 2) - scaler / 2;
        else
            y = scaler * (Height / 2);

        w += x;
        h += y;

        w /= scaler;
        h /= scaler;

        w = Mathf.Round(w);
        h = Mathf.Round(h);
        return new Vector3(w, h, 0);
    }



    private Dictionary<int, Vector2> DictNotNullPiece()
    {
        Dictionary<int, Vector2> NotNullPiece = new();

        int i = 0;
        for (int h = 0; h < Height; h++)
        {
            for (int w = 0; w < Width; w++)
            {
                if (puzzleGeneration.Pieces[w, h].TypePiece != 0)
                {
                    NotNullPiece[i] = new Vector2(w, h);
                    i++;
                }

            }
        }
        return NotNullPiece;
    }



    /// <summary> ��������� ���� �� � ������ X-����� �, ���� ����, �� ����� �� � �������� ���������, 
    /// ���� ���, �� ������� ��������� ��������� �����.</summary>
    /// <returns>���������� ������������ � ������.</returns>
    private Vector2 GenerateStartPiece()
    {
        foreach (var piece in Pieces)
        {
            if (QuickSweep(piece.transform.localPosition.x, piece.transform.localPosition.y) == 4)
            {
                return new Vector2(CalculateArrayPosition(piece.transform.localPosition.x, piece.transform.localPosition.y).x,
                    CalculateArrayPosition(piece.transform.localPosition.x, piece.transform.localPosition.y).y);
            }
        }

        Dictionary<int, Vector2> NotNullPiece = DictNotNullPiece();
        int i = Random.Range(1, NotNullPiece.Count);

        return new Vector2(NotNullPiece[i].x, NotNullPiece[i].y);
    }


    /// <summary> ��������� ���������� ����������.</summary>
    /// <returns> ���������� ���������� ����������.</returns>
    private int Sweep()
    {
        int value = 0;

        for (int h = 0; h < Height; h++)
        {
            for (int w = 0; w < Width; w++)
            {
                // ��������� ������
                if (h != Height - 1)
                    if (Pieces[w, h].values[0] == 1 && Pieces[w, h + 1].values[2] == 1)
                        value++;

                // ��������� ������
                if (w != Width - 1)
                    if (Pieces[w, h].values[1] == 1 && Pieces[w + 1, h].values[3] == 1)
                        value++;
            }
        }
        return value;
    }

    /// <summary> ��������� ����������� �������� ����������.</summary>
    /// <returns> ���������� ���������� �������� ����������.</returns>
    private int GetWinValue()
    {
        int winValue = 0;
        foreach (var piece in Pieces)
            foreach (var j in piece.values)
                winValue += j;
        winValue /= 2;
        return winValue;
    }

    /// <summary>  ��������� ������������� ���� ����� ������.</summary>
    private void Shuffle()
    {
        foreach (var piece in Pieces)
        {
            int k = Random.Range(0, 4);

            for (int i = 0; i < k; i++)
                piece.RotatePieceStart();
        }
    }

    /// <summary> ��������� �������� ���� ��� ������������� ������.</summary>
    /// <remarks> ���� ������ ���� �������������.</remarks>
    /// <returns> ���������� ���������� ������� �����.</returns>
    private Vector2 CheckDimensions()
    {
        Vector2 coord = Vector2.zero;
        GameObject[] pieces = GameObject.FindGameObjectsWithTag("Piece");

        foreach (var p in pieces)
        {
            if (p.transform.position.x > coord.x)
            {
                coord.x = p.transform.position.x;
            }
            if (p.transform.position.y > coord.y)
            {
                coord.y = p.transform.position.y;
            }
        }
        coord.x++;
        coord.y++;
        return coord;
    }

    /// <summary> ����������� ������� ���������� ���� ����� ��� ����.</summary>
    /// <param name="w"> ���������� �� ������.</param>
    /// <param name="h"> ���������� �� ������.</param>
    /// <param name="WaterPipes"> ���� � ���������� �������.</param>
    private void PathFinding(int w, int h, List<Piece> ExploredPipes)
    {
        int fool = 4;
        if (ExploredPipes.Contains(Pieces[w, h]))
            return;  // ������ �� ������
        else
            ExploredPipes.Add(Pieces[w, h]);

        // ��������� ������
        if (w != Width - 1)
            if (Pieces[w, h].values[1] == 1 && Pieces[w + 1, h].values[3] == 1)
            {
                Pieces[w + 1, h].ChangePipeMaterial(true, GameState.PLAYING);
                PathFinding(w + 1, h, ExploredPipes);
            }
            else fool--;

        // ��������� ������
        if (h != Height - 1)
            if (Pieces[w, h].values[0] == 1 && Pieces[w, h + 1].values[2] == 1)
            {
                Pieces[w, h + 1].ChangePipeMaterial(true, GameState.PLAYING);
                PathFinding(w, h + 1, ExploredPipes);
            }
            else fool--;

        // ��������� �����
        if (h != 0)
            if (Pieces[w, h].values[2] == 1 && Pieces[w, h - 1].values[0] == 1)
            {
                Pieces[w, h - 1].ChangePipeMaterial(true, GameState.PLAYING);
                PathFinding(w, h - 1, ExploredPipes);
            }
            else fool--;

        // ��������� �����
        if (w != 0)
            if (Pieces[w, h].values[3] == 1 && Pieces[w - 1, h].values[1] == 1)
            {
                Pieces[w - 1, h].ChangePipeMaterial(true, GameState.PLAYING);
                PathFinding(w - 1, h, ExploredPipes);
            }
            else fool--;

        if (fool == 0)
            Pieces[w - 1, h].ChangePipeMaterial(false, GameState.PLAYING);
    }

    /// <summary> ��������� ���������� ���������� ������.</summary>
    /// <param name="w"> ���������� �� ������.</param>
    /// <param name="h"> ���������� �� ������.</param>
    /// <returns> ���������� ���������� ����������.</returns>
    public int QuickSweep(float x, float y)
    {
        Vector3 temp = CalculateArrayPosition(x, y);
        int w = (int)temp.x;
        int h = (int)temp.y;

        int value = 0;

        // ��������� ������
        if (h != Height - 1)
            if (Pieces[w, h].values[0] == 1 && Pieces[w, h + 1].values[2] == 1)
                value++;

        // ��������� ������
        if (w != Width - 1)
            if (Pieces[w, h].values[1] == 1 && Pieces[w + 1, h].values[3] == 1)
                value++;

        // ��������� �����
        if (w != 0)
            if (Pieces[w, h].values[3] == 1 && Pieces[w - 1, h].values[1] == 1)
                value++;

        // ��������� �����
        if (h != 0)
            if (Pieces[w, h].values[2] == 1 && Pieces[w, h - 1].values[0] == 1)
                value++;
        //print(value);
        return value;
    }

    /// <summary> �������� �� ���� � �����.</summary>
    public void CheckEther()
    {
        List<Piece> ExploredPipes = new();
        foreach (var piece in Pieces)
            piece.ChangePipeMaterial(false, GameState.PLAYING);

        Pieces[(int)startPiece.x, (int)startPiece.y].ChangePipeMaterial(true, GameState.PLAYING);
        PathFinding((int)startPiece.x, (int)startPiece.y, ExploredPipes);
    }
}