using System.Collections;
using UnityEngine;

/// <summary> �������� ������������ ������.</summary>
public class CrystalAltarAnimation : MonoBehaviour
{
    /// <summary> ������ �������� �����.</summary>
    [SerializeField] private Transform outerCircle;

    /// <summary> ������ �������� �����.</summary>
    [SerializeField] private Transform middleCircle;

    /// <summary> �������� �������� �����.</summary>
    private const float CircleRotaionSpeed = 100;

    /// <summary> ����������������� ��������.</summary>
    private const float AnimationDuration = 3f;

    /// <summary> ���������� �����.</summary>
    private float timeLeft;

    /// <summary> ��������������� id �������.</summary>
    public int GeneratedShardId { get; private set; }

    /// <summary> ������� ��������� �������� ��� ������.</summary>
    public void StartAnimation()
    {
        StartCoroutine(Animate());
    }

    /// <summary> �������� �������� ������������ ������.</summary>
    private IEnumerator Animate()
    {
        timeLeft = AnimationDuration;
        StartRotation();

        while (timeLeft > 0)
        {
            timeLeft -= Time.deltaTime;
            yield return null;
        }

        timeLeft = 0;
        GeneratedShardId = CountShardId(CountShardColor(outerCircle.localEulerAngles.z), CountShardType(middleCircle.localEulerAngles.z));
        print("GeneratedShardId " + GeneratedShardId);
    }

    /// <summary> ������� ������ �������� �������� � �������� ������.</summary>
    private void StartRotation()
    {
        StartCoroutine(RotateOuterCircle(CircleRotaionSpeed));
        StartCoroutine(RotateMiddleCircle(CircleRotaionSpeed));
    }

    /// <summary> �������� �������� �������� �����.</summary>
    private IEnumerator RotateOuterCircle(float speedOuterCircle)
    {
        int rand = Random.Range(1, 3);
        float t = 0;
        bool isRotateOuter = true; // ���� �������� �������� �����.
        while (isRotateOuter)
        {
            outerCircle.Rotate(0, 0, speedOuterCircle * Time.deltaTime);
            yield return null;
            t += Time.deltaTime;

            if (t > timeLeft - 24 * rand / speedOuterCircle)
                isRotateOuter = false;
        }

        Vector3 eulerAngles = outerCircle.localEulerAngles;
        eulerAngles.z = Mathf.Round(eulerAngles.z / 24f) * 24f + 12 + 24 * rand;
        if (eulerAngles.z > 360)
            eulerAngles.z %= 360;
        Quaternion start = outerCircle.rotation;
        if (rand > 1) rand--;
        for (t = 0; t < 1; t += Time.deltaTime * rand)
        {
            outerCircle.rotation = Quaternion.LerpUnclamped(start, Quaternion.Euler(eulerAngles), t);
            yield return null;
        }
        outerCircle.rotation = Quaternion.Euler(new Vector3(0, 0, Mathf.Round(outerCircle.localEulerAngles.z)));
    }

    /// <summary> �������� �������� �������� �����.</summary>
    private IEnumerator RotateMiddleCircle(float speedMiddleCircle)
    {
        int rand = Random.Range(1, 3);
        float t = 0;
        bool isRotateMiddle = true; // ���� �������� �������� �����.
        while (isRotateMiddle)
        {
            middleCircle.Rotate(0, 0, -speedMiddleCircle * Time.deltaTime);
            yield return null;
            t += Time.deltaTime;

            if (t > timeLeft - 30 * rand / speedMiddleCircle - 1)
                isRotateMiddle = false;
        }

        Vector3 eulerAngles = middleCircle.localEulerAngles;
        eulerAngles.z = Mathf.Round(eulerAngles.z / 30f) * 30f - 15 - 30 * rand;
        if (eulerAngles.z > 360)
            eulerAngles.z %= 360;
        Quaternion start = middleCircle.rotation;
        if (rand > 1) rand--;
        for (t = 0; t < 1; t += Time.deltaTime * rand)
        {
            middleCircle.rotation = Quaternion.LerpUnclamped(start, Quaternion.Euler(eulerAngles), t);
            yield return null;
        }
        middleCircle.rotation = Quaternion.Euler(new Vector3(0, 0, Mathf.Round(middleCircle.localEulerAngles.z)));
    }

    /// <summary> ��������� id �������.</summary>
    /// <returns></returns>
    private int CountShardId(ShardColor shardColor, ShardType shardType)
    {
        return (int)shardColor * 3 + (int)shardType;
    }

    /// <summary> ���� �������.</summary>
    private enum ShardColor
    {
        /// <summary> �����.</summary>
        Blue = 0,

        /// <summary> �������.</summary>
        Red = 1,

        /// <summary> �������.</summary>
        Pink = 2,

        /// <summary> �������������� ���������.</summary>
        Undefined = -1
    }

    /// <summary> ��� �������.</summary>
    private enum ShardType
    {
        /// <summary> �����.</summary>
        Book = 1,

        /// <summary> ����.</summary>
        Stylus = 2,

        /// <summary> �������.</summary>
        Vial = 3,

        /// <summary> �������������� ���������.</summary>
        Undefined = -1
    }

    /// <summary> ��������� ���� �������.</summary>
    /// <param name="circleAngle"> ���� �������� ����� �� ���������.</param>
    /// <returns> ���������� ���� �������.</returns>
    private ShardColor CountShardColor(float circleAngle)
    {
        int color = (int)Mathf.Round((circleAngle - 12) / 12);
        switch (color % 3)
        {
            case 0:
                return ShardColor.Pink;
            case 1:
                return ShardColor.Blue;
            case 2:
                return ShardColor.Red;
            default:
                return ShardColor.Undefined;
        }
    }

    /// <summary> ��������� ��� �������.</summary>
    /// <param name="circleAngle"> ���� �������� ����� �� ���������.</param>
    /// <returns> ���������� ��� �������.</returns>
    private ShardType CountShardType(float circleAngle)
    {
        int sign = (int)Mathf.Round((circleAngle - 15) / 30);
        switch (sign % 3)
        {
            case 0:
                return ShardType.Book;
            case 1:
                return ShardType.Vial;
            case 2:
                return ShardType.Stylus;
            default:
                return ShardType.Undefined;
        }
    }
}