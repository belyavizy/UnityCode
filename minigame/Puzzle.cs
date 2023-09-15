using System.Collections.Generic;
using UnityEngine;

/// <summary> Класс Паззл.</summary>
/// <remarks> Отвечает за создание и управлением головоломки.</remarks>
public class Puzzle : MonoBehaviour
{
    /// <summary> Гейм-менеджер.</summary>
    GameManager gameManager;

    /// <summary> Инстанс Паззла.</summary>
    public static Puzzle Instance;

    /// <summary> Матрица из труб.</summary>
    public Piece[,] Pieces { get; private set; }
    public Piece[,] NonPlayablePieces { get; private set; }
    /// <summary> Количество соединений труб при выигрышной ситуации.</summary>
    public int WinValue { get; private set; }

    /// <summary> Количество текущих соединений труб.</summary>
    public int CurValue { get; set; }

    /// <summary> Массив префабов труб.</summary>
    /// <remarks> Задается в инспекторе.</remarks>
    public GameObject[] PiecePrefabs;

    /// <summary> Область для паззла.</summary>
    [SerializeField] private GameObject gameField;

    /// <summary> Ширина матрицы паззла.</summary>
    [SerializeField] private int Width;

    /// <summary> Высота матрицы паззла.</summary>
    [SerializeField] private int Height;

    /// <summary> Кастомная вероятность.</summary>
    /// <remarks> Вероятность выпадения 0 на выход трубы.</remarks>
    [SerializeField] private int randomProbability = 40;

    /// <summary> Расположение в паззле трубы с цветом.</summary>
    private Vector2 startPiece;

    /// <summary> Единичный вызов функции UpdateState </summary>
    private bool UpdatedState = false;

    /// <summary> Вспомогательный класс Паззла для генерации паззла.</summary>
    [System.Serializable]
    public class PuzzleGeneration
    {
        public PieceGeneration[,] Pieces;
    }

    /// <summary> Вспомогательный класс Трубы для генерации паззла.</summary>
    [System.Serializable]
    public class PieceGeneration
    {
        public int TypePiece = 0;
        public int[] values = { 0, 0, 0, 0 };
    }

    /// <summary> Объявление вспомогательного паззла .</summary>
    public PuzzleGeneration puzzleGeneration;

    /// <summary> Инициирует объект Puzzle.</summary>
    private void Awake()
    {
        Instance = this;
    }

    /// <summary> Старт.</summary>
    private void Start()
    {
        gameManager = GameManager.Instance;
        StartGame();
    }

    /// <summary> Создание паззла.</summary>
    private void StartGame()
    {
        // проверка на генерацию уровня
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

    /// <summary> Проверка на state.</summary>
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


    /// <summary> Генерация уровня с помощью вспомогательного класса.</summary>
    private void GeneratePuzzle() //TODO пересмотреть границы вспомогательного массива 
    {
        // Инициирование вспомогательного паззла
        puzzleGeneration.Pieces = new PieceGeneration[Width, Height];

        for (int i = 0; i < Width; i++)
            for (int j = 0; j < Height; j++)
                puzzleGeneration.Pieces[i, j] = new PieceGeneration();
        int attempts = 0;
        // Генерация уровня
        do
        {
            for (int h = 0; h < Height; h++)
            {
                for (int w = 0; w < Width; w++)
                {
                    // 1 труба должна иметь вход слева
                    if (h == 0 && w == 0)
                    {
                        puzzleGeneration.Pieces[w, h].values[0] = 1;
                        puzzleGeneration.Pieces[w, h].values[1] = 1;
                        puzzleGeneration.Pieces[w, h].values[2] = 0;
                        puzzleGeneration.Pieces[w, h].values[3] = 0;
                    }
                    else
                    {
                        // Ограничения по ширине 
                        if (w == 0)
                            puzzleGeneration.Pieces[w, h].values[3] = 0;
                        else
                            puzzleGeneration.Pieces[w, h].values[3] = puzzleGeneration.Pieces[w - 1, h].values[1];

                        if (w == Width - 1)
                            puzzleGeneration.Pieces[w, h].values[1] = 0;
                        else
                            puzzleGeneration.Pieces[w, h].values[1] = RandomProbability();


                        // Ограничения по высоте
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
            // пока будут одинарные трубы и количество пустых элементов > заданного значения
        } while ((CheckFirstPiece() || SumNullPiece() > Width * Height * 0.2 + 1 || CheckPuzzle()));/////////////////////////////////////////// TODO проверить рекурсию в CheckPuzzle

        //|| CheckPuzzle()
        print("attempts=" + attempts);
    }

    /// <summary> Проверка типа трубы.</summary>
    /// <param name="w"> Координата по ширине.</param>
    /// <param name="h"> Координата по высоте.</param>
    private void CheckPieceType(int w, int h)
    {
        // высчитывание типа на основе количества выходов
        puzzleGeneration.Pieces[w, h].TypePiece = puzzleGeneration.Pieces[w, h].values[0] + puzzleGeneration.Pieces[w, h].values[1]
                        + puzzleGeneration.Pieces[w, h].values[2] + puzzleGeneration.Pieces[w, h].values[3];

        // выбор типа для l-образной и c-образной трубы
        if (puzzleGeneration.Pieces[w, h].TypePiece == 2 && puzzleGeneration.Pieces[w, h].values[0] != puzzleGeneration.Pieces[w, h].values[2])
            puzzleGeneration.Pieces[w, h].TypePiece = 5;

        // если попалась одинарная труба -> превращаем в ноль и переделываем соседей
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

    /// <summary> Проверка на присутсвие одинарных труб.</summary>
    /// <returns> Возвращает булевое значение.</returns>
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

    /// <summary> Подсчет нулевых/ненулевых труб.</summary>
    /// <param name="state"> True: возвращает нулевые трубы; false: возвращает ненулевые трубы.</param>
    /// <returns>Возвращает количество труб.</returns>
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

    /// <summary> Проверка паззла на цельность.</summary>
    /// <returns>True: все ок; False: есть отдельный кусок.</returns>
    private bool CheckPuzzle()
    {
        Dictionary<PieceGeneration, bool> ExploredPipes = new();
        Dictionary<int, Vector2> NotNullPiece = DictNotNullPiece();
        CheckPath((int)NotNullPiece[0].x, (int)NotNullPiece[0].y, ExploredPipes);
        if (ExploredPipes.Count != SumNotNullPiece())
            return true;

        return false;
    }


    /// <summary> Рекурсивная функция нахождения всех путей для выявления отдельных элементов.</summary>
    /// <param name="w"> Координата по ширине.</param>
    /// <param name="h"> Координата по высоте.</param>
    /// <param name="ExploredrPipes"> Лист с найденными трубами.</param>
    private void CheckPath(int w, int h, Dictionary<PieceGeneration, bool> ExploredPipes)
    {
        int fool = 4;

        if (ExploredPipes.ContainsKey(puzzleGeneration.Pieces[w, h]))
            return;  // ничего не делаем
        else
            ExploredPipes.Add(puzzleGeneration.Pieces[w, h], false);
        // сравнение справа
        if (w != Width - 1)
            if (puzzleGeneration.Pieces[w, h].values[1] == 1 && puzzleGeneration.Pieces[w + 1, h].values[3] == 1)
            {
                ExploredPipes[puzzleGeneration.Pieces[w, h]] = true;
                CheckPath(w + 1, h, ExploredPipes);
            }
            else fool--;

        // сравнение сверху
        if (h != Height - 1)
            if (puzzleGeneration.Pieces[w, h].values[0] == 1 && puzzleGeneration.Pieces[w, h + 1].values[2] == 1)
            {
                ExploredPipes[puzzleGeneration.Pieces[w, h]] = true;
                CheckPath(w, h + 1, ExploredPipes);
            }
            else fool--;

        // сравнение снизу
        if (h != 0)
            if (puzzleGeneration.Pieces[w, h].values[2] == 1 && puzzleGeneration.Pieces[w, h - 1].values[0] == 1)
            {
                ExploredPipes[puzzleGeneration.Pieces[w, h]] = true;
                CheckPath(w, h - 1, ExploredPipes);
            }
            else fool--;

        // сравнение слева
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


    /// <summary> Кастомный рандом.</summary>
    /// <returns> Возвращает 0 в указанных количествах (randomProbability) случаев.</returns>
    private int RandomProbability()
    {
        int rnd = Random.Range(1, 101);
        if (rnd < randomProbability)
            return 0;
        else return 1;
    }

    /// <summary> Создание паззла.</summary>
    private void InstantiatePuzzle()
    {
        Pieces = new Piece[Width, Height];

        // добаление в массив полученных труб
        for (int h = 0; h < Height; h++)
        {
            for (int w = 0; w < Width; w++)
            {
                // вспомогательный объект для создания паззла
                GameObject go = (GameObject)Instantiate(PiecePrefabs[puzzleGeneration.Pieces[w, h].TypePiece], CalculatePosition(w, h), Quaternion.identity);

                go.transform.SetParent(gameField.transform, false);

                go.transform.localScale *= (CalculateMultyplier() / 100);
                // прокручивание трубы, пока значения массива выходов не совпадут с положением трубы.
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


    /// <summary> Получение множителя для правильного создания труб относительно размеров экрана 1920*1080.</summary>
    /// <returns> Возвращает множитель.</returns>
    private float CalculateMultyplier()
    {

        Rect gameFieldRect = gameField.GetComponent<RectTransform>().rect;
        //print("height " + gameFieldRect.height + " width " + gameFieldRect.width);
        if (gameFieldRect.height / (float)Height < gameFieldRect.width / (float)Width)
            return gameFieldRect.height / (float)Height;
        else
            return gameFieldRect.width / (float)Width;
    }

    /// <summary> Получение позиции трубы. </summary>
    /// <param name="w"> Значение по ширине в массиве.</param>
    /// <param name="h"> Значние по выосте в массиве.</param>
    /// <returns> Возвращает 3D вектор с позицией.</returns>
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
    /// <summary> Возвращает положенние в массиве puzzle относительно координатов на экране.</summary>
    /// <param name="w"> Координата по ширине.</param>
    /// <param name="h"> Координата по высоте.</param>
    /// <returns> Возвращает 3D вектор c позицией.</returns>
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



    /// <summary> Проверяет есть ли в паззле X-труба и, если есть, то берет ее в качестве стартовой, 
    /// если нет, то берется рандомная ненулевая труба.</summary>
    /// <returns>Возвращает расположение в паззле.</returns>
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


    /// <summary> Получение количества соединений.</summary>
    /// <returns> Возвращает количества соединений.</returns>
    private int Sweep()
    {
        int value = 0;

        for (int h = 0; h < Height; h++)
        {
            for (int w = 0; w < Width; w++)
            {
                // сравнение сверху
                if (h != Height - 1)
                    if (Pieces[w, h].values[0] == 1 && Pieces[w, h + 1].values[2] == 1)
                        value++;

                // сравнение справа
                if (w != Width - 1)
                    if (Pieces[w, h].values[1] == 1 && Pieces[w + 1, h].values[3] == 1)
                        value++;
            }
        }
        return value;
    }

    /// <summary> Получение выигрышного значения соединений.</summary>
    /// <returns> ВОзвращает выигрышное значение соединений.</returns>
    private int GetWinValue()
    {
        int winValue = 0;
        foreach (var piece in Pieces)
            foreach (var j in piece.values)
                winValue += j;
        winValue /= 2;
        return winValue;
    }

    /// <summary>  Рандомное прокручивание труб после старта.</summary>
    private void Shuffle()
    {
        foreach (var piece in Pieces)
        {
            int k = Random.Range(0, 4);

            for (int i = 0; i < k; i++)
                piece.RotatePieceStart();
        }
    }

    /// <summary> Получение размеров поля для предсозданого паззла.</summary>
    /// <remarks> Поле должно быть прямоугольное.</remarks>
    /// <returns> Возвращает координату крайней трубы.</returns>
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

    /// <summary> Рекурсивная функция нахождения всех путей для воды.</summary>
    /// <param name="w"> Координата по ширине.</param>
    /// <param name="h"> Координата по высоте.</param>
    /// <param name="WaterPipes"> Лист с найденными трубами.</param>
    private void PathFinding(int w, int h, List<Piece> ExploredPipes)
    {
        int fool = 4;
        if (ExploredPipes.Contains(Pieces[w, h]))
            return;  // ничего не делаем
        else
            ExploredPipes.Add(Pieces[w, h]);

        // сравнение справа
        if (w != Width - 1)
            if (Pieces[w, h].values[1] == 1 && Pieces[w + 1, h].values[3] == 1)
            {
                Pieces[w + 1, h].ChangePipeMaterial(true, GameState.PLAYING);
                PathFinding(w + 1, h, ExploredPipes);
            }
            else fool--;

        // сравнение сверху
        if (h != Height - 1)
            if (Pieces[w, h].values[0] == 1 && Pieces[w, h + 1].values[2] == 1)
            {
                Pieces[w, h + 1].ChangePipeMaterial(true, GameState.PLAYING);
                PathFinding(w, h + 1, ExploredPipes);
            }
            else fool--;

        // сравнение снизу
        if (h != 0)
            if (Pieces[w, h].values[2] == 1 && Pieces[w, h - 1].values[0] == 1)
            {
                Pieces[w, h - 1].ChangePipeMaterial(true, GameState.PLAYING);
                PathFinding(w, h - 1, ExploredPipes);
            }
            else fool--;

        // сравнение слева
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

    /// <summary> Получение соединений конкретной детали.</summary>
    /// <param name="w"> Координата по ширине.</param>
    /// <param name="h"> Координата по высоте.</param>
    /// <returns> Возвращает количество соединений.</returns>
    public int QuickSweep(float x, float y)
    {
        Vector3 temp = CalculateArrayPosition(x, y);
        int w = (int)temp.x;
        int h = (int)temp.y;

        int value = 0;

        // сравнение сверху
        if (h != Height - 1)
            if (Pieces[w, h].values[0] == 1 && Pieces[w, h + 1].values[2] == 1)
                value++;

        // сравнение справа
        if (w != Width - 1)
            if (Pieces[w, h].values[1] == 1 && Pieces[w + 1, h].values[3] == 1)
                value++;

        // сравнение слева
        if (w != 0)
            if (Pieces[w, h].values[3] == 1 && Pieces[w - 1, h].values[1] == 1)
                value++;

        // сравнение снизу
        if (h != 0)
            if (Pieces[w, h].values[2] == 1 && Pieces[w, h - 1].values[0] == 1)
                value++;
        //print(value);
        return value;
    }

    /// <summary> Проверка на воду в трубе.</summary>
    public void CheckEther()
    {
        List<Piece> ExploredPipes = new();
        foreach (var piece in Pieces)
            piece.ChangePipeMaterial(false, GameState.PLAYING);

        Pieces[(int)startPiece.x, (int)startPiece.y].ChangePipeMaterial(true, GameState.PLAYING);
        PathFinding((int)startPiece.x, (int)startPiece.y, ExploredPipes);
    }
}