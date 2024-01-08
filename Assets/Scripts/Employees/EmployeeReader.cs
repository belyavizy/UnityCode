using UnityEngine;


/// <summary>�����, ����������� ������ ����������� �� JSON.</summary>
public class EmployeeReader : MonoBehaviour
{
    /// <summary>����� JSON.</summary>
    [SerializeField] private TextAsset TextJSON;

    /// <summary>������ �� �����.</summary>
    public static EmployeeReader Instance;

    /// <summary>������ ���������� ��� JSON.</summary>
    public EmployeeArray employeeList = new();


    /// <summary>����� ���������� ��� JSON.</summary>
    [System.Serializable]
    public class EmployeeJson
    {
        public string name;
        public int level = 1;
        public int salary = 500;
        public int productionSpeed = 20;
        public int productionQuality = 2;
        public int productionIncrease = 0;
        public int resourceCashback = 20; // ���������� �����
        public string specialization;
        public string spritePath;
    }

    /// <summary>����� ������� ����������� ��� JSON.</summary>
    /// <remarks>����� FromJson ������� ������ �� JSON, � ��� ����� ��������� �� �������� employee, ������� ����� ��������� ����� ������� �����������.</remarks>
    [System.Serializable]
    public class EmployeeArray
    {
        public EmployeeJson[] employee;

        public int Length { get { return employee.Length; } }
    }

   
    /// <summary>������ �� JSON ����.</summary>
    private void Start()
    {   
        Instance = this;
        employeeList = JsonUtility.FromJson<EmployeeArray>("{\"employee\":" + TextJSON.text + "}");
        //employeeList = JsonUtility.FromJson<EmployeeList>(TextJSON.text);
    }
}
