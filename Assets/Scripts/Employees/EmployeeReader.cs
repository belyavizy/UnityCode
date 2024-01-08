using UnityEngine;


/// <summary>Класс, считывающий массив сотрудников из JSON.</summary>
public class EmployeeReader : MonoBehaviour
{
    /// <summary>Ассет JSON.</summary>
    [SerializeField] private TextAsset TextJSON;

    /// <summary>Ссылка на ридер.</summary>
    public static EmployeeReader Instance;

    /// <summary>Массив сотруднков для JSON.</summary>
    public EmployeeArray employeeList = new();


    /// <summary>Класс сотрудника для JSON.</summary>
    [System.Serializable]
    public class EmployeeJson
    {
        public string name;
        public int level = 1;
        public int salary = 500;
        public int productionSpeed = 20;
        public int productionQuality = 2;
        public int productionIncrease = 0;
        public int resourceCashback = 20; // сохранение ресов
        public string specialization;
        public string spritePath;
    }

    /// <summary>Класс массива сотрудников для JSON.</summary>
    /// <remarks>Метод FromJson создает объект из JSON, а там лежит массивчик из объектов employee, поэтому нужен отдельный класс массива сотрудников.</remarks>
    [System.Serializable]
    public class EmployeeArray
    {
        public EmployeeJson[] employee;

        public int Length { get { return employee.Length; } }
    }

   
    /// <summary>Импорт из JSON инфо.</summary>
    private void Start()
    {   
        Instance = this;
        employeeList = JsonUtility.FromJson<EmployeeArray>("{\"employee\":" + TextJSON.text + "}");
        //employeeList = JsonUtility.FromJson<EmployeeList>(TextJSON.text);
    }
}
