using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>Enum ����� ������.</summary>
[Serializable] public enum RoomTypeEnum
{
    NullType,
    FirstType,
    SecondType,
    ThirdType
}

/// <summary>����� �������.</summary>
public class Room : MonoBehaviour
{
    public RoomHandler RoomHandler { get; private set; }

    /// <summary>������ �� GridManager.</summary>
    public GridManager GridManager { get; private set; }

    /// <summary>������ ����������� ��� ������.</summary>
    public Sprite[] RoomImages { get; private set; }

    /// <summary>������ �������� ������������.</summary>
    [SerializeField] private Button DelEquipmentButton;

    /// <summary>������ ������ ������������.</summary>
    public GameObject EquipmentPanelButton;

    /// <summary>����������� �������.</summary>
    public Image Image { get; private set; }

    /// <summary>������ ���������� ������������.</summary>
    public Button AddEquipmentButton;
    
    /// <summary>����: ��� �������.</summary>
    private RoomType _roomTypeState;
    /// <summary>��������: ��� �������.</summary>
    public RoomType RoomTypeState 
    {
        get => _roomTypeState;
        set
        {
            // ���. ������, ���� ����
            _roomTypeState = value;
        }
    }

    /// <summary>����������� �������.</summary>
    /// <param name="rt">����� �������.</param>
    public Room(RoomType rt) => RoomTypeState = rt;

    /// <summary>����� ������� �� Enum.</summary>
    private RoomTypeEnum _roomTypeEnum;
    public RoomTypeEnum RoomTypeEnum { get => _roomTypeEnum; set => _roomTypeEnum = value; }

    /// <summary>����� �������.</summary>
    public int RoomLength {get; set;}

    /// <summary>������ � �������� ������� �� �������.</summary>
    public Vector2 position;


    /// <summary>������������� ����������.</summary>
    private void Awake()
    {
        RoomTypeState = new NullType();
        GridManager = GridManager.Instance;
        RoomImages = LoadSpriteSheet();

        Image = GetComponentInChildren<Image>();
        RoomHandler = new(this);
    }

    /// <summary>�������� �������� ������ � ������.</summary>
    /// <returns></returns>
    /// <remarks>�������� ����� ��������� � GridManager.</remarks> //TODO ������� addressable assets
    private Sprite[] LoadSpriteSheet()
    {
        Sprite[] sprites = Resources.LoadAll<Sprite>("rooms/");
        return sprites;
    }

    /// <summary>������ �����������.</summary>
    /// <param name="roomImage">������ �����������.</param>
    public void ChangeImage(int roomImage)
    {
        Image.sprite = RoomImages[roomImage];
    }

    /// <summary>��������� �������� ������ ���� ������������.</summary>
    public void OnAddEquipmentButtonClick()
    {
        RoomHandler.OnAddEquipmentButtonClick();
    }

    /// <summary>��������� ���� �������.</summary>
    /// <param name="typeButton">��� ������� � �������, ������������ ����� ������.</param>
    public void OnEquipmentButtonClick(string typeStr)
    {
        RoomHandler.OnEquipmentButtonClick(typeStr);
    }

    /// <summary>�������� ������������ �� ������� �� ������.</summary>
    public void OnDeleteEquipmentButtonClickFirstVar()
    {
       RoomHandler.OnDeleteEquipmentButtonClickFirstVar();
    }

    /// <summary>��������� ������ ������������� �� ������ BuildButton.</summary>
    public void SwitchBuildingState()
    {
        if (RoomTypeState is NullType)
            AddEquipmentButton.gameObject.SetActive(!AddEquipmentButton.gameObject.activeSelf);
        else DelEquipmentButton.gameObject.SetActive((!DelEquipmentButton.gameObject.activeSelf));
    }

    /// <summary>������������ ��������� ���������� ������ ���������� ������������.</summary>
    /// <param name="state">������� ���������.</param>
    public void SetAddEquipmentButtonActivity(bool state)
    {
        AddEquipmentButton.gameObject.SetActive(state);
    }

    /// <summary>������������ ��������� ���������� ������ ������������.</summary>
    /// <param name="state">������� ���������.</param>
    public void SetEquipmentPanelActivity(bool state)
    {
        EquipmentPanelButton.SetActive(state);
    }
}