using UnityEngine;
using System.Linq;
using System;
using UnityEngine.UI;
using System.Collections.Generic;
//using System.Reflection;

/// <summary>����� ����������� ������������ �� ������ ������� � ���������� �������� ������������.</summary>
public class EmployeeManager : MonoBehaviour
{
    /// <summary>������ �� EmployeeManager.</summary>
    public static EmployeeManager Instance;

    /// <summary>������ ��������� �����������.</summary>
    public List<Employee> empList;

    /// <summary>������ ������� �����������.</summary>
    public List<Employee> workersList;

    /// <summary>������ � �����������.</summary>
    [SerializeField] private GameObject empPrefab;

    /// <summary>���� ��� ������ �����������.</summary>
    [SerializeField] private GameObject scrollArea;

    /// <summary>������ �� ����� json.</summary>
    private EmployeeReader reader;


    /// <summary>C�������� ��������: �������-false, ������� ����������-true .</summary>
    public bool State = false;

    /// <summary>������� EmployeeManager.</summary>
    public void Awake()
    {
        Instance = this;
    }


    /// <summary>����� ������� ��� �������� ������.</summary>
    private void Start()
    {
        reader = EmployeeReader.Instance;

        empList = new();

        for (int i = 0; i < reader.employeeList.Length; i++)
        {
            GameObject go = (GameObject)Instantiate(empPrefab); //�������� ���������������� �������
            go.transform.SetParent(this.transform, false);

            empList.Add(go.GetComponent<Employee>());
            print(empList);
            SetEmployeeData(empList[i], i);
            empList[i].UpdateText();
        }
        //scrollArea.SetActive(false);
    }


    /// <summary>������� �� ������ ���������� � ����� ����������.</summary>
    /// <param name="emp">������ �� ������ ����������</param>
    /// <param name="index">������ ����������.</param>
    private void SetEmployeeData(Employee emp, int index)
    {
        emp.name = reader.employeeList.employee[index].name;
        emp.level = reader.employeeList.employee[index].level;
        emp.salary = reader.employeeList.employee[index].salary;
        emp.productionSpeed = reader.employeeList.employee[index].productionSpeed;
        emp.productionQualityLevel = reader.employeeList.employee[index].productionQuality;
        emp.productionIncrease = reader.employeeList.employee[index].productionIncrease;
        emp.resourceCashback = reader.employeeList.employee[index].resourceCashback;
        if (!Enum.TryParse(reader.employeeList.employee[index].specialization, out Specialization typeOut)) //������� ������ � enum
        {
            print("�� ������, " + typeOut + "��� ������������� �� ���");
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

    /// <summary>������ �� ���������� � ����������� �� ��������� ���������.</summary>
    /// <param name="emp">������ �� ����������.</param>
    /// <param name="needHired">False - ����� �� �������; true - ����� �������.</param>
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

    /// <summary>������ �� ����� ������� ����������� � ����������� �� ��������� ���������.</summary>
    /// <param name="needHired">False - ����� �� �������; true - ����� �������.</param>
    public void CheckIfHiredArray(bool needHired)
    {
       for (int i = 0; i < empList.Count; i++)
            CheckIfHired(empList[i], needHired);  
    }

    /// <summary>�������� �� ������������� ���������� � ������� �� ���������.</summary>
    /// <param name="emp">������ �� ����������.</param>
    private void CheckIsAlchemist(Employee emp)
    {
        if (emp.specialization != Specialization.Alchemy)
        {
            emp.gameObject.SetActive(false);
        }
    }

    /// <summary>�������� �� ������������� ���������� � ������� �� ��������.</summary>
    /// <param name="emp">������ �� ����������.</param>
    private void CheckIsBlacksmith(Employee emp)
    {
        //print("aaaa");
        if (emp.specialization != Specialization.Blacksmithing)
        {
            emp.gameObject.SetActive(false);
        }
    }

    /// <summary>�������� �� ������������� ���������� � ������� �� ��������.</summary>
    /// <param name="emp">������ �� ����������.</param>
    private void CheckIsJoiner(Employee emp)
    {
        //print("aaaa");
        if (emp.specialization != Specialization.Joinery)
        {
            emp.gameObject.SetActive(false);
        }
    }

    /// <summary>����� ��������� ����������� �� ������ �������.</summary>
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

    /// <summary>����� ������� ����������� �� ������ ����������.</summary>
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

    /// <summary>���������� �� ������� �������������.</summary>
    /// <param name="spec">������������ �������� � ������ �������������.</param>
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


    /// <summary>���������� �� ��������.</summary>
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
    /// <summary>���������� �� ��������.</summary>
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
    /// <summary>���������� �� ���������.</summary>
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

    /// <summary>���������� �� "������" �����������.</summary>
    /// <param name="field">����������� ���� �� ������.</param>
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

    /// <summary>���������� ����������� �� ������ � ����������� �� ������������ � �������.</summary>
    private void SortOnCanvas()
    {
        for (int i = 0; i < empList.Count; i++)
        {
            empList[i].transform.SetAsLastSibling();
            //print(empArray[i].level);
        }
    }
}
