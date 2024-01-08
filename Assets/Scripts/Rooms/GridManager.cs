using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>Тестовый класс, управляющий комнатами.</summary>
public class GridManager : MonoBehaviour
{
    /// <summary>Инстанс.</summary>
    public static GridManager Instance;
    /// <summary>Матрица комнат.</summary>
    private Room[,] rooms;
    public Room[,] Rooms
    {
        get
        {
            if (rooms == null)
            {
                Debug.LogError("Rooms array is not initialized!");
            }
            return rooms;
        }
        private set => rooms = value;
    }

    /// <summary>Рект трансформ для CalculateMultiplier().</summary>
    private RectTransform rectTransform;
    private RectTransform RectTransform
    {
        get
        {
            if (rectTransform == null)
            {
                rectTransform = GetComponent<RectTransform>();
            }
            return rectTransform;
        }
    }

    /// <summary>Префаб комнаты.</summary>
    [SerializeField] private GameObject RoomPrefab;

    /// <summary>Поле: Высота матрицы.</summary>
    private int height = 5;
    /// <summary>Поле: Ширина матрицы.</summary>
    private int width = 5;
    /// <summary>Свойство: Высота матрицы.</summary>
    public int Height { get => height; set => height = value; }
    /// <summary>Свойство: Ширина матрицы.</summary>
    public int Width { get => width; set => width = value; }

    /// <summary>Создание ссылки на GridManager/</summary>
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        InstantiateRooms();
    }


    /// <summary>Создание комнат.</summary>
    private void InstantiateRooms()
    {
        Rooms = new Room[Width, Height];
        for (int w = 0; w < Width; w++)
        {
            for (int h = 0; h < 3; h++)
            {
                GameObject roomObject = Instantiate(RoomPrefab, CalculatePosition(w, h), Quaternion.identity);
                roomObject.transform.SetParent(transform, false);

                Room room = roomObject.GetComponent<Room>();
                room.position = new Vector2(w, h);
                Rooms[w, h] = room;
            }
        }
    }


    /// <summary> Получение позиции комнаты. </summary>
    /// <param name="w"> Значение по ширине в массиве.</param>
    /// <param name="h"> Значние по выосте в массиве.</param>
    /// <returns> Возвращает 3D вектор с позицией.</returns>
    private Vector3 CalculatePosition(int w, int h)
    {
        int scaler = (int)CalculateMultyplier();
        int halfWidth = Width / 2;
        int halfHeight = Height / 2;

        int xOffset = (Width % 2 == 0) ? scaler / 2 : 0;
        int yOffset = (Height % 2 == 0) ? scaler / 2 : 0;

        int x = scaler * (halfWidth - xOffset);
        int y = scaler * (halfHeight - yOffset);

        w = w * scaler - x;
        h = h * scaler - y;

        return new Vector3(w, h, 0);
    }


    /// <summary> Получение множителя для правильного создания комнаты относительно размеров GridManager.</summary>
    /// <returns> Возвращает множитель.</returns>
    private float CalculateMultyplier()
    {
        Rect gameFieldRect = RectTransform.rect;
        return Mathf.Min(gameFieldRect.height / Height, gameFieldRect.width / Width);
    }


    /// <summary> Возвращает положенние в массиве Rooms относительно координатов на экране.</summary>
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

    /// <summary>Включение режима строительства по кнопке BuildButton.</summary>
    public void OnBuildButtonClick()
    {
        for (int w = 0; w < Width; w++)
        {
            for (int h = 0; h < 3; h++)
            {
                Rooms[w, h].SwitchBuildingState();
            }
        }
    }
    /// <summary>Изменение размеров комнаты в зависимости от ее длины.</summary>
    /// <param name="typeOut">Тип комнаты.</param>
    /// <param name="x">координата х проверяемой комнаты, вызвавшей метод.</param>
    /// <param name="y">координата у комнаты, вызвавшей метод.</param>
    internal void UpdateRooms(RoomTypeEnum typeOut, float x, float y, bool ChangeStateToNull)// я, шибзоид, торжественно клянусь поменять на рекурсию вместо if-else
    {
        if (ChangeStateToNull) print(typeOut);
        List<Room> rooms = new();
        Vector3 pos = CalculateArrayPosition(x, y);
        int h = (int)pos.y;
        int roomsCount = 0;

        for (int i = 0; i < Width; i++)
        {
            if (Rooms[i, h].RoomTypeEnum == typeOut)
            {
                rooms.Add(Rooms[i, h]);
                roomsCount++;
                ChangeRoomSize(rooms, roomsCount);
            }
            else
            {
                rooms.Clear();
                roomsCount = 0;
            }
        }
    }

    /// <summary>Изменение "размера" комнаты.</summary>
    /// <param name="rooms">Лист с комнатами определенного типа.</param>
    /// <param name="count">Количество найденных комнат в строке.</param>
    private void ChangeRoomSize(List<Room> rooms, int count)
    {
        int i = 0;
        if (count >= 3)
        {
            rooms[0].RoomLength = 3;
            rooms[0].ChangeImage(1);
            rooms[1].RoomLength = 3;
            rooms[1].ChangeImage(3);
            rooms[2].RoomLength = 3;
            rooms[2].ChangeImage(2);
            count -= 3;
            i = 3;
        }
        if (count == 2)
        {
            rooms[i].RoomLength = 2;
            rooms[i].ChangeImage(1);
            rooms[i + 1].RoomLength = 2;
            rooms[i + 1].ChangeImage(2);
        }
        if (count == 1)
        {
            rooms[i].RoomLength = 1;
            rooms[i].ChangeImage(0);
        }
    }


    /// <summary>Изменение положения в иерархии комнаты для корректного отображения панели выбора оборудования.</summary>
    /// <param name="position">Позиция комнаты на экране.</param>
    internal void ChangeAddEquipmentButton(Vector2 position)
    {
        Rooms[(int)position.x, (int)position.y].transform.SetAsLastSibling();
        foreach (var room in Rooms)
        {
            if (room == null)
                continue;
            
            room.SetEquipmentPanelActivity(false);
        }
    }
}