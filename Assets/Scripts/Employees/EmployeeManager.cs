using UnityEngine;
using System.Linq;
using System;
using UnityEngine.UI;
using System.Collections.Generic;
//using System.Reflection;

/// <summary>Класс управляющий сотрудниками на панели гильдии и управлении нанятами сотрудниками.</summary>
public class EmployeeManager : MonoBehaviour
{
    /// <summary>Ссылка на EmployeeManager.</summary>
    public static EmployeeManager Instance;

    /// <summary>Массив ненанятых сотрудников.</summary>
    public List<Employee> empList;

    /// <summary>Массив нанятых сотрудников.</summary>
    public List<Employee> workersList;

    /// <summary>Префаб с сотрудником.</summary>
    [SerializeField] private GameObject empPrefab;

    /// <summary>Поле для вывода сотрудников.</summary>
    [SerializeField] private GameObject scrollArea;

    /// <summary>Ссылка на ридер json.</summary>
    private EmployeeReader reader;


    /// <summary>Cостояние панельки: гильдия-false, нанятые сотрудники-true .</summary>
    public bool State = false;

    /// <summary>Инстанс EmployeeManager.</summary>
    public void Awake()
    {
        Instance = this;
    }


    /// <summary>Вызов функций при создании панели.</summary>
    private void Start()
    {
        reader = EmployeeReader.Instance;

        empList = new();

        for (int i = 0; i < reader.employeeList.Length; i++)
        {
            GameObject go = (GameObject)Instantiate(empPrefab); //создание вспомогательного объекта
            go.transform.SetParent(this.transform, false);

            empList.Add(go.GetComponent<Employee>());
            print(empList);
            SetEmployeeData(empList[i], i);
            empList[i].UpdateText();
        }
        //scrollArea.SetActive(false);
    }


    /// <summary>Перенос из ридера ниформации в класс сотрудника.</summary>
    /// <param name="emp">Ссылка на объект сотрудника</param>
    /// <param name="index">Индекс сотрудника.</param>
    private void SetEmployeeData(Employee emp, int index)
    {
        emp.name = reader.employeeList.employee[index].name;
        emp.level = reader.employeeList.employee[index].level;
        emp.salary = reader.employeeList.employee[index].salary;
        emp.productionSpeed = reader.employeeList.employee[index].productionSpeed;
        emp.productionQualityLevel = reader.employeeList.employee[index].productionQuality;
        emp.productionIncrease = reader.employeeList.employee[index].productionIncrease;
        emp.resourceCashback = reader.employeeList.employee[index].resourceCashback;
        if (!Enum.TryParse(reader.employeeList.employee[index].specialization, out Specialization typeOut)) //парсинг строки в enum
        {
            print("ты ебеобо, " + typeOut + "тип специализации не тот");
        }
        else
        {
            emp.specialization = typeOut;
        }
        Sprite sprite = Resources.Load<Sprite>(reader.employeeList.employee[index].spritePath);
        //emp.image.sprite = sprite;
        print(emp.GetComponentInChildren<Image>().sprite);
        print(sprite);
    }

    /// <summary>Апдейт по сотруднику в зависимости от состояния нанятости.</summary>
    /// <param name="emp">Ссылка на сотрудника.</param>
    /// <param name="needHired">False - нужны из гильдии; true - нужны нанятые.</param>
    private void CheckIfHired(Employee emp, bool needHired)
    {
        //print(State);
        if (!needHired)
        {
            if (emp.IsHired)
                emp.gameObject.SetActive(false);
        }
        else
            if (!emp.IsHired)
                emp.gameObject.SetActive(false);
    }

    /// <summary>Апдейт по всему массиву сотрудников в зависимости от состояния нанятости.</summary>
    /// <param name="needHired">False - нужны из гильдии; true - нужны нанятые.</param>
    public void CheckIfHiredArray(bool needHired)
    {
       for (int i = 0; i < empList.Count; i++)
            CheckIfHired(empList[i], needHired);  
    }

    /// <summary>Проверка на специализацию сотрудника и скрытия не алхимиков.</summary>
    /// <param name="emp">Ссылка на сотрудника.</param>
    private void CheckIsAlchemist(Employee emp)
    {
        if (emp.specialization != Specialization.Alchemy)
        {
            emp.gameObject.SetActive(false);
        }
    }

    /// <summary>Проверка на специализацию сотрудника и скрытия не кузнецов.</summary>
    /// <param name="emp">Ссылка на сотрудника.</param>
    private void CheckIsBlacksmith(Employee emp)
    {
        //print("aaaa");
        if (emp.specialization != Specialization.Blacksmithing)
        {
            emp.gameObject.SetActive(false);
        }
    }

    /// <summary>Проверка на специализацию сотрудника и скрытия не столяров.</summary>
    /// <param name="emp">Ссылка на сотрудника.</param>
    private void CheckIsJoiner(Employee emp)
    {
        //print("aaaa");
        if (emp.specialization != Specialization.Joinery)
        {
            emp.gameObject.SetActive(false);
        }
    }

    /// <summary>Вывод ненанятых сотрудников на панель гильдии.</summary>
    public void SetGuildActive()
    {
        //scrollArea.SetActive(true);
        //print("egnoirgn");
        empList = empList.OrderByDescending(i => i.level).ToList();
        SortOnCanvas();

        for (int i = 0; i < empList.Count; i++)
        {
            empList[i].gameObject.SetActive(true);
            CheckIfHired(empList[i], State);
            CheckIsAlchemist(empList[i]);
        }
    }

    /// <summary>Вывод нанятых сотрудников на панель работников.</summary>
    public void SetWorkersActive()
    {
        //scrollArea.SetActive(true);
        empList = empList.OrderByDescending(i => i.level).ToList();
        SortOnCanvas();

        for (int i = 0; i < empList.Count; i++)
        {
            empList[i].gameObject.SetActive(true);
            CheckIfHired(empList[i], State);
            CheckIsAlchemist(empList[i]);
        }
    }

    /// <summary>Сортировка по кнопкам специализаций.</summary>
    /// <param name="spec">Передаваемое значение в строке специализации.</param>
    public void OnSpecializationButtonClick(string spec)
    {
        empList = empList.OrderByDescending(i => i.level).ToList();
        for (int i = 0; i < empList.Count; i++)
        {
            empList[i].gameObject.SetActive(true);
            switch (spec)
            {
                case "joiner":
                    CheckIsJoiner(empList[i]);
                    break;
                case "alchemist":
                    CheckIsAlchemist(empList[i]);
                    break;
                case "blacksmith":
                    CheckIsBlacksmith(empList[i]);
                    break;
            }
            CheckIfHired(empList[i], State);
        }
        SortOnCanvas();
    }


    /// <summary>Сортировка по кузнецам.</summary>
    public void SortByBlacksmithings()
    {
        empList = empList.OrderByDescending(i => i.level).ToList();
        for (int i = 0; i < empList.Count; i++)
        {
            empList[i].gameObject.SetActive(true);

            CheckIsJoiner(empList[i]);
            CheckIfHired(empList[i], State);
        }
        SortOnCanvas();
    }
    /// <summary>Сортировка по столярам.</summary>
    public void SortByJoiners()
    {
        empList = empList.OrderByDescending(i => i.level).ToList();
        for (int i = 0; i < empList.Count; i++)
        {
            empList[i].gameObject.SetActive(true);
            CheckIsBlacksmith(empList[i]);
            CheckIfHired(empList[i], State);
        }
        SortOnCanvas();
    }
    /// <summary>Сортировка по алхимикам.</summary>
    public void SortByAlchemics()
    {
        empList = empList.OrderByDescending(i => i.level).ToList();
        for (int i = 0; i < empList.Count; i++)
        {
            empList[i].gameObject.SetActive(true);
            CheckIfHired(empList[i], State);
            CheckIsAlchemist(empList[i]);
        }
        SortOnCanvas();
    }

    /// <summary>Сортировка по "статам" сотрудников.</summary>
    /// <param name="field">Сортируемый стат из кнопки.</param>
    public void OnSortButtonClick(string field)
    {
        //Type myClassType = typeof(Employee);

        //var fiel = myClassType.GetField(field);
        //empArray = empArray.OrderBy(i => i.fiel).ToArray();

        switch (field)
        {
            case "level":
                empList = empList.OrderByDescending(i => i.level).ToList();
                SortOnCanvas();
                break;
            case "salary":
                empList = empList.OrderByDescending(i => i.salary).ToList();
                SortOnCanvas();
                break;
            case "productionSpeed":
                empList = empList.OrderByDescending(i => i.productionSpeed).ToList();
                SortOnCanvas();
                break;
            case "productionIncrease":
                empList = empList.OrderByDescending(i => i.productionIncrease).ToList();
                SortOnCanvas();
                break;
            case "resourceSaving":
                empList = empList.OrderByDescending(i => i.resourceCashback).ToList();
                SortOnCanvas();
                break;
        }
    }

    /// <summary>Сортировка сотрудников на панели в зависимости от расположения в массиве.</summary>
    private void SortOnCanvas()
    {
        for (int i = 0; i < empList.Count; i++)
        {
            empList[i].transform.SetAsLastSibling();
            //print(empArray[i].level);
        }
    }
}
