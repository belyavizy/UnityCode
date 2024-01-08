using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>������������ ������������� �����������.</summary>
public enum Specialization {
    Alchemy,
    Blacksmithing,
    Joinery
}

/// <summary>����� �����������.</summary>
public class Employee : MonoBehaviour
{
    private EmployeeManager manager;

    /// <summary>����� �� �������.</summary>
    [SerializeField] private int levelLimit = 5; //�����-�� ����� ���� ������ 
    /// <summary>����� �� ��������.</summary>
    [SerializeField] private int salaryLimit = 1000;
    /// <summary>����� �� �������� ������������.</summary>
    [SerializeField] private int productionSpeedLimit = 5;
    /// <summary>����� �� ��� � ������������ �������.</summary>
    [SerializeField] private int productionIncreaseLimit = 5;
    /// <summary>����� �� ���������� ��������.</summary>
    [SerializeField] private int resourceCashbackLimit = 5; // ���������� �����

    /// <summary>��������� ���� ��� ���� name.</summary>
    [SerializeField] private TMP_Text nameText;

    /// <summary>��������� ���� ��� ���� level.</summary>
    [SerializeField] private TMP_Text levelText;

    /// <summary>��������� ���� ��� ���� salary.</summary>
    [SerializeField] private TMP_Text salaryText;

    /// <summary>��������� ���� ��� ���� production speed.</summary>
    [SerializeField] private TMP_Text productionSpeedText;

    /// <summary>��������� ���� ��� ���� production quality.</summary>
    [SerializeField] private TMP_Text productionQualityText;

    /// <summary>��������� ���� ��� ���� production increase.</summary>
    [SerializeField] private TMP_Text prodictionIncreaseText;

    /// <summary>��������� ���� ��� ���� resource saving.</summary>
    [SerializeField] private TMP_Text resourceSavingText;
    /// <summary>������� � ���������� ���������� �������� ������.</summary>
    private Dictionary<string, int> productionQuality = new()
    {
        {"Low", 1 },
        {"Medium", 2 },
        {"High", 3 }
    };

    /// <summary>��� ����������.</summary>
    public new string name;
    /// <summary>������� ����������.</summary>
    public int level = 1;
    /// <summary>�������� ����������.</summary>
    public int salary = 500;
    /// <summary>C������� ������������ ����������.</summary>
    public int productionSpeed = 20;
    /// <summary>������� �������� ����������� ������� �����������.</summary>
    public int productionQualityLevel = 1;
    /// <summary>��� � ������������ �������.</summary>
    public int productionIncrease = 0;
    /// <summary>���������� �������� ��� ������������ �����������.</summary>
    public int resourceCashback = 20; // ���������� �����
    /// <summary>������������� ����������.</summary>
    public Specialization specialization = Specialization.Alchemy;
    
    public Image image;

    /// <summary>����� �������� ��� ���.</summary>
    public bool IsHired { get; set; }

    /// <summary>����� ������� ��� �������� �������.</summary>
    private void Start()
    {
        manager = EmployeeManager.Instance;
        UpdateText();
        IsHired = false;
        image = GetComponentInChildren<Image>();
    }

    /// <summary>��������� ������������ ���������� ������ �� �������.</summary>
    public void UpdateText()
    {
        nameText.text = name;
        levelText.text = level.ToString();
        salaryText.text = salary.ToString();
        productionSpeedText.text = productionSpeed.ToString();

        string productionQualityString = productionQuality["Low"].ToString() + "%-" + productionQuality["Medium"].ToString() + "%-" + productionQuality["High"].ToString() + "%";

        productionQualityText.text = productionQualityString.ToString();
        prodictionIncreaseText.text = productionIncrease.ToString() + "%";
        resourceSavingText.text = resourceCashback.ToString();
    }
    
    /// <summary>���������� ��������� ��� �������� �������.</summary>
    /// <param name="level">������� ������� �������� �������.</param>
    public void CalculateProductionQuality(int level)
    {
        productionQuality["High"]  = level;
        productionQuality["Medium"] = level * 5;
        productionQuality["Low"] = 100 - level * 6;
     }

    /// <summary>����� �� ������ ������ ����������.</summary>
    public void OnHiringButtonClick()
    {
        IsHired = true;
        this.gameObject.SetActive(false);
        manager.workersList.Add(this);
        //manager.empList.Remove(this);
    }

    /// <summary>����� �� ������ ������� ����������.</summary>
    public void OnFiringButtonClick()
    {
        IsHired = false;
        this.gameObject.SetActive(false);
        manager.workersList.Remove(this);
        //manager.empList.Add(this);
    }
}
