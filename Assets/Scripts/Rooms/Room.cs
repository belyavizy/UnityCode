using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>Enum типов комнат.</summary>
[Serializable] public enum RoomTypeEnum
{
    NullType,
    FirstType,
    SecondType,
    ThirdType
}

/// <summary>Класс комнаты.</summary>
public class Room : MonoBehaviour
{
    public RoomHandler RoomHandler { get; private set; }

    /// <summary>Ссылка на GridManager.</summary>
    public GridManager GridManager { get; private set; }

    /// <summary>Массив изображений дял комнат.</summary>
    public Sprite[] RoomImages { get; private set; }

    /// <summary>Кнопка удаления оборудования.</summary>
    [SerializeField] private Button DelEquipmentButton;

    /// <summary>Панель выбора оборудования.</summary>
    public GameObject EquipmentPanelButton;

    /// <summary>Изображение комнаты.</summary>
    public Image Image { get; private set; }

    /// <summary>Кнопка добавления оборудования.</summary>
    public Button AddEquipmentButton;
    
    /// <summary>Поле: Тип комнаты.</summary>
    private RoomType _roomTypeState;
    /// <summary>Свойство: Тип комнаты.</summary>
    public RoomType RoomTypeState 
    {
        get => _roomTypeState;
        set
        {
            // Доп. логика, если нада
            _roomTypeState = value;
        }
    }

    /// <summary>Конструктор комнаты.</summary>
    /// <param name="rt">Стате комнаты.</param>
    public Room(RoomType rt) => RoomTypeState = rt;

    /// <summary>Стате комнаты из Enum.</summary>
    private RoomTypeEnum _roomTypeEnum;
    public RoomTypeEnum RoomTypeEnum { get => _roomTypeEnum; set => _roomTypeEnum = value; }

    /// <summary>Длина комнаты.</summary>
    public int RoomLength {get; set;}

    /// <summary>Вектор с позицией комнаты на канвасе.</summary>
    public Vector2 position;


    /// <summary>Инициализация переменных.</summary>
    private void Awake()
    {
        RoomTypeState = new NullType();
        GridManager = GridManager.Instance;
        RoomImages = LoadSpriteSheet();

        Image = GetComponentInChildren<Image>();
        RoomHandler = new(this);
    }

    /// <summary>Загрузка спрайтов комнат в массив.</summary>
    /// <returns></returns>
    /// <remarks>Возможно стоит перенести в GridManager.</remarks> //TODO чекнуть addressable assets
    private Sprite[] LoadSpriteSheet()
    {
        Sprite[] sprites = Resources.LoadAll<Sprite>("rooms/");
        return sprites;
    }

    /// <summary>Замена изображения.</summary>
    /// <param name="roomImage">индекс изображения.</param>
    public void ChangeImage(int roomImage)
    {
        Image.sprite = RoomImages[roomImage];
    }

    /// <summary>Активация панельки выбора типа оборудования.</summary>
    public void OnAddEquipmentButtonClick()
    {
        RoomHandler.OnAddEquipmentButtonClick();
    }

    /// <summary>Изменение типа комнаты.</summary>
    /// <param name="typeButton">Тип комнаты в стринге, передаваемый через кнопку.</param>
    public void OnEquipmentButtonClick(string typeStr)
    {
        RoomHandler.OnEquipmentButtonClick(typeStr);
    }

    /// <summary>Удаление оборудования из комнаты по кнопке.</summary>
    public void OnDeleteEquipmentButtonClickFirstVar()
    {
       RoomHandler.OnDeleteEquipmentButtonClickFirstVar();
    }

    /// <summary>Включение режима строительства по кнопке BuildButton.</summary>
    public void SwitchBuildingState()
    {
        if (RoomTypeState is NullType)
            AddEquipmentButton.gameObject.SetActive(!AddEquipmentButton.gameObject.activeSelf);
        else DelEquipmentButton.gameObject.SetActive((!DelEquipmentButton.gameObject.activeSelf));
    }

    /// <summary>Переключение состояния активности кнопки добавления оборудования.</summary>
    /// <param name="state">Булевое состояние.</param>
    public void SetAddEquipmentButtonActivity(bool state)
    {
        AddEquipmentButton.gameObject.SetActive(state);
    }

    /// <summary>Переключение состояния активности панели оборудования.</summary>
    /// <param name="state">Булевое состояние.</param>
    public void SetEquipmentPanelActivity(bool state)
    {
        EquipmentPanelButton.SetActive(state);
    }
}