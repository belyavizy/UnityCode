using System;
using UnityEngine;

public class RoomHandler
{
    private readonly Room _room;

    public RoomHandler(Room room)
    {
        _room = room;
    }

    public void OnAddEquipmentButtonClick()
    {
        _room.GridManager.ChangeAddEquipmentButton(_room.position);
        _room.EquipmentPanelButton.SetActive(!_room.EquipmentPanelButton.activeSelf);
    }

    public void OnEquipmentButtonClick(string typeStr)
    {
        if (!Enum.TryParse(typeStr, out RoomTypeEnum typeOut))
        {
            Debug.LogError("Неправильный тип оборудования: " + typeStr);
            return;
        }

        _room.RoomTypeState.ChangeTypeFromNull(_room, typeOut);
        _room.RoomTypeState.Check();
        _room.RoomTypeState.ChangeSpriteColor(_room.Image);
        _room.EquipmentPanelButton.SetActive(false);
        _room.AddEquipmentButton.gameObject.SetActive(false);
        _room.GridManager.UpdateRooms(typeOut, _room.transform.localPosition.x, _room.transform.localPosition.y, false);
    }

    public void OnDeleteEquipmentButtonClickFirstVar()
    {
        RoomTypeEnum temp = _room.RoomTypeEnum;
        _room.RoomTypeState.ChangeTypeNull(_room);
        _room.RoomTypeState.Check();
        _room.RoomTypeState.ChangeSpriteColor(_room.Image);
        _room.Image.sprite = _room.RoomImages[0];
        _room.RoomLength = 1;
        _room.GridManager.UpdateRooms(temp, _room.transform.localPosition.x, _room.transform.localPosition.y, true);
    }
}