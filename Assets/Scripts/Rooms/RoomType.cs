using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ����������� ����� ��� �������� ��������� ��� ������.
/// </summary>
public abstract class RoomType
{
    public abstract void Check();
    public virtual void ChangeTypeNull(Room room)
    {
        room.RoomTypeEnum = RoomTypeEnum.NullType;
        room.RoomTypeState = new NullType();
    }

    public virtual void ChangeTypeFromNull(Room room, RoomTypeEnum type) { }
    public abstract void ChangeSpriteColor(Image image);
}

//C��� �����������, �� ��������� ��� :)
public class RoomTypeFactory
{
    private static readonly Dictionary<RoomTypeEnum, Type> RoomTypeDictionary = new()
    {
        { RoomTypeEnum.FirstType, typeof(FirstType) },
        { RoomTypeEnum.SecondType, typeof(SecondType) },
        { RoomTypeEnum.ThirdType, typeof(ThirdType) }
    };

    public static ColoredRoomType CreateRoomType(RoomTypeEnum type)
    {
        if (RoomTypeDictionary.TryGetValue(type, out Type roomType))
        {
            return (ColoredRoomType)Activator.CreateInstance(roomType);
        }
        return null;
    }
}

/// <summary>
/// ������ ��� �������.
/// </summary>
public class NullType : RoomType
{
    public override void Check()
    {
        Debug.Log("Hi, I'm null type of room");
    }

    public override void ChangeTypeFromNull(Room room, RoomTypeEnum type)
    {
        room.RoomTypeEnum = type;
        room.RoomTypeState = RoomTypeFactory.CreateRoomType(type);
    }

    public override void ChangeSpriteColor(Image image)
    {
        image.color = Color.white;
    }
}

/// <summary>
/// ������� ����� ��� ����� ������ � ������.
/// </summary>
public abstract class ColoredRoomType : RoomType
{
    protected abstract Color GetRoomColor();

    public override void ChangeSpriteColor(Image image)
    {
        image.color = GetRoomColor();
    }
}

/// <summary>
/// ������ ��� �������.
/// </summary>
class FirstType : ColoredRoomType
{
    public override void Check()
    {
        Debug.Log("Hi, I'm first type of room");
    }

    protected override Color GetRoomColor()
    {
        return Color.green;
    }
}

/// <summary>
/// ������ ��� �������.
/// </summary>
class SecondType : ColoredRoomType
{
    public override void Check()
    {
        Debug.Log("Hi, I'm second type of room");
    }

    protected override Color GetRoomColor()
    {
        return Color.yellow;
    }
}

/// <summary>
/// ������ ��� �������.
/// </summary>
class ThirdType : ColoredRoomType
{
    public override void Check()
    {
        Debug.Log("Hi, I'm third type of room");
    }

    protected override Color GetRoomColor()
    {
        return Color.magenta;
    }
}