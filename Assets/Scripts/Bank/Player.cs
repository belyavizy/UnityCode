using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player Instance;
    public Money Money { get; set; }

    void Start()
    {
        Money = new();
        Instance = this;
        Money.Value = 2000;
    }

}
