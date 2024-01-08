using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>Перечисление специализации сотрудников.</summary>
public enum Specialization {
    Alchemy,
    Blacksmithing,
    Joinery
}

/// <summary>Класс сотрудников.</summary>
public class Employee : MonoBehaviour
{
    private EmployeeManager manager;

    /// <summary>Лимит на уровень.</summary>
    [SerializeField] private int levelLimit = 5; //какая-то херня если честно 
    /// <summary>Лимит на зарплату.</summary>
    [SerializeField] private int salaryLimit = 1000;
    /// <summary>Лимит на скорость производства.</summary>
    [SerializeField] private int productionSpeedLimit = 5;
    /// <summary>Лимит на баф к производству товаров.</summary>
    [SerializeField] private int productionIncreaseLimit = 5;
    /// <summary>Лимит на сохранение ресурсов.</summary>
    [SerializeField] private int resourceCashbackLimit = 5; // сохранение ресов

    /// <summary>Текстовое поле для поля name.</summary>
    [SerializeField] private TMP_Text nameText;

    /// <summary>Текстовое поле для поля level.</summary>
    [SerializeField] private TMP_Text levelText;

    /// <summary>Текстовое поле для поля salary.</summary>
    [SerializeField] private TMP_Text salaryText;

    /// <summary>Текстовое поле для поля production speed.</summary>
    [SerializeField] private TMP_Text productionSpeedText;

    /// <summary>Текстовое поле для поля production quality.</summary>
    [SerializeField] private TMP_Text productionQualityText;

    /// <summary>Текстовое поле для поля production increase.</summary>
    [SerializeField] private TMP_Text prodictionIncreaseText;

    /// <summary>Текстовое поле для поля resource saving.</summary>
    [SerializeField] private TMP_Text resourceSavingText;
    /// <summary>Словарь с процентыми значениями качества товара.</summary>
    private Dictionary<string, int> productionQuality = new()
    {
        {"Low", 1 },
        {"Medium", 2 },
        {"High", 3 }
    };

    /// <summary>Имя сотрудника.</summary>
    public new string name;
    /// <summary>Уровень сотрудника.</summary>
    public int level = 1;
    /// <summary>Зарплата сотрудника.</summary>
    public int salary = 500;
    /// <summary>Cкорость производства сотрудника.</summary>
    public int productionSpeed = 20;
    /// <summary>Уровень качества создаваемых товаров сотрудником.</summary>
    public int productionQualityLevel = 1;
    /// <summary>Баф к производству товаров.</summary>
    public int productionIncrease = 0;
    /// <summary>Сохранение ресурсов при производстве сотрудником.</summary>
    public int resourceCashback = 20; // сохранение ресов
    /// <summary>Специализация сотрудника.</summary>
    public Specialization specialization = Specialization.Alchemy;
    
    public Image image;

    /// <summary>Нанят сотудник или нет.</summary>
    public bool IsHired { get; set; }

    /// <summary>Вызов функций при создании объекта.</summary>
    private void Start()
    {
        manager = EmployeeManager.Instance;
        UpdateText();
        IsHired = false;
        image = GetComponentInChildren<Image>();
    }

    /// <summary>Изменение отображаемых переменных класса на канвасе.</summary>
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
    
    /// <summary>Вычисление процентов для качества товаров.</summary>
    /// <param name="level">Текущий уровень качества товаров.</param>
    public void CalculateProductionQuality(int level)
    {
        productionQuality["High"]  = level;
        productionQuality["Medium"] = level * 5;
        productionQuality["Low"] = 100 - level * 6;
     }

    /// <summary>Вызов по кнопке нанять сотрудника.</summary>
    public void OnHiringButtonClick()
    {
        IsHired = true;
        this.gameObject.SetActive(false);
        manager.workersList.Add(this);
        //manager.empList.Remove(this);
    }

    /// <summary>Вызов по кнопке уволить сотрудника.</summary>
    public void OnFiringButtonClick()
    {
        IsHired = false;
        this.gameObject.SetActive(false);
        manager.workersList.Remove(this);
        //manager.empList.Add(this);
    }
}
