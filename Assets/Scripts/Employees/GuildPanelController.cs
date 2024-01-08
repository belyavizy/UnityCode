using UnityEngine;

/// <summary>Класс контроля над панелью сотрудников.</summary>
public class GuildPanelController : MonoBehaviour
{
    /// <summary>Объект сотрудников.</summary>
    [SerializeField] private GameObject Employees;

    /// <summary>Ссылка на класс EmployeeManager.</summary>
    private EmployeeManager emps;

    /// <summary>Ссылка на класс EmployeeManager через объект.</summary>
    private EmployeeManager empClass;


    /// <summary>Инициализация EmployeeManager.</summary>
    private void Awake()
    {
        emps = EmployeeManager.Instance;
        empClass = Employees.GetComponent<EmployeeManager>();

    }

    /// <summary>Метод, срабатывающий при создании объекта.</summary>
    private void Start()
    {
        gameObject.SetActive(false); // без этого не работает адекватно EmployeeManager
    }

    /// <summary>Ловим нажатие на ESC.</summary>
    private void Update()// TODO посмотреть про input system 
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if(this.gameObject.activeSelf)
            {
                this.gameObject.SetActive(false);
            }
        }
    }

    /// <summary>Клик на кнопку гильдии.</summary>
    public void OnGuildButtonClick()
    {
        this.gameObject.SetActive(!this.gameObject.activeSelf);
        empClass.State = false;
        empClass.SetGuildActive();
    }

    /// <summary>Клик на кнопку нанятых сотрудников.</summary>
    public void OnWorkerButtonClick()
    {
        //print(emps);
        this.gameObject.SetActive(!this.gameObject.activeSelf);
        empClass.State = true;
        empClass.SetWorkersActive();

        //print("fbeog");
    }

    /// <summary>Закрытие панели по кнопке.</summary>
    public void OnExitButtonClick()
    {
        this.gameObject.SetActive(false);
    }
}
