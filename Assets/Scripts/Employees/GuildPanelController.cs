using UnityEngine;

/// <summary>����� �������� ��� ������� �����������.</summary>
public class GuildPanelController : MonoBehaviour
{
    /// <summary>������ �����������.</summary>
    [SerializeField] private GameObject Employees;

    /// <summary>������ �� ����� EmployeeManager.</summary>
    private EmployeeManager emps;

    /// <summary>������ �� ����� EmployeeManager ����� ������.</summary>
    private EmployeeManager empClass;


    /// <summary>������������� EmployeeManager.</summary>
    private void Awake()
    {
        emps = EmployeeManager.Instance;
        empClass = Employees.GetComponent<EmployeeManager>();

    }

    /// <summary>�����, ������������� ��� �������� �������.</summary>
    private void Start()
    {
        gameObject.SetActive(false); // ��� ����� �� �������� ��������� EmployeeManager
    }

    /// <summary>����� ������� �� ESC.</summary>
    private void Update()// TODO ���������� ��� input system 
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if(this.gameObject.activeSelf)
            {
                this.gameObject.SetActive(false);
            }
        }
    }

    /// <summary>���� �� ������ �������.</summary>
    public void OnGuildButtonClick()
    {
        this.gameObject.SetActive(!this.gameObject.activeSelf);
        empClass.State = false;
        empClass.SetGuildActive();
    }

    /// <summary>���� �� ������ ������� �����������.</summary>
    public void OnWorkerButtonClick()
    {
        //print(emps);
        this.gameObject.SetActive(!this.gameObject.activeSelf);
        empClass.State = true;
        empClass.SetWorkersActive();

        //print("fbeog");
    }

    /// <summary>�������� ������ �� ������.</summary>
    public void OnExitButtonClick()
    {
        this.gameObject.SetActive(false);
    }
}
