using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Bank : MonoBehaviour
{
    [SerializeField] private Scrollbar monthScrollbar;
    [SerializeField] private Scrollbar sumScrollbar;

    private int monthAmountPanel = 3;
    private int sumAmountPanel = 0;
    private Money payment = new();

    private float percent = 0.03f;

    [SerializeField] private TMP_Text monthAmountText;
    [SerializeField] private TMP_Text sumText;
    [SerializeField] private TMP_InputField sumInputField;
    [SerializeField] private TMP_Text paymentAmountText;
    [SerializeField] private TMP_Text debtAmountText;
    [SerializeField] private GameObject paymentDates;
    [SerializeField] private GameObject loanDate;
    [SerializeField] private GameObject overdraftDate;

    [SerializeField] private TMP_InputField debtInputField;
    [SerializeField] private Scrollbar debtScrollbar;

    private int debtLimit;
    private bool inputDebtState;
    private bool inputLoanState;
    private int sumLimit = 50000;
    private int debt;

    private Player player;

    public List<Loan> LoanList = new();
    public List<Overdraft> OverdraftList = new();
    public class Loan
    {
        public Money Amount;
        public int monthAmount;
        public Money CurrentAmount;
        public Loan(int amount, int month)
        {
            Amount = new()
            {
                Value = amount
            };
            monthAmount = month;
            CurrentAmount = new()
            {
                Value = amount
            };
        }

    }
    public class Overdraft // � �� � ��� ������ ������� � ���
    {
        public Money Amount;
        public int monthAmount = 2;
        public Overdraft(int amount)
        {
            Amount.Value = amount;
        }
    }

    void Start()
    {
        player = Player.Instance;
        monthScrollbar.onValueChanged.AddListener((monthValue) => OnPercentScrollbar(monthValue));
        sumScrollbar.onValueChanged.AddListener(value => OnSumScrollbar(value));
        debtScrollbar.onValueChanged.AddListener(value => OnDebtScrollbar(value));

        debtAmountText.text = CalculateDebt().ToString();
        CheckDates();
        debt = CalculateDebt();

        debtLimit = player.Money.Value;
        OnDebtInputFieldChanged();
    }

    private void OnDebtScrollbar(float value)
    {
        if (!inputDebtState)
        {
            
            if (debt > debtLimit)
                debt = debtLimit;
            debtInputField.text = ((int)(value * debt)).ToString();
        }
    }

    private void CheckDates()
    {
        if (!CheckLoans() && !CheckOverdraft())
        {
            paymentDates.SetActive(false);
        }
        else
        {
            paymentDates.SetActive(true);
            loanDate.SetActive(CheckLoans());
            overdraftDate.SetActive(CheckOverdraft());
        }
    }
    private bool CheckLoans()
    {
        if (LoanList.Count == 0)
        {
            return false;
        }
        else return true;
    }
    private bool CheckOverdraft()
    {
        if (OverdraftList.Count == 0)
        {
            return false;
        }
        else return true;
    }
    private int CalculateDebt()
    {
        return CalculateSumLoans() + CalculateSumOD();
    }
    private int CalculateSumLoans()
    {
        int sumLoan = 0;
        foreach (var loan in LoanList)
        {
            sumLoan += loan.Amount.Value;
        }
        return sumLoan;
    }
    private int CalculateSumOD()
    {
        int sumOD = 0;
        foreach (var overdraft in OverdraftList)
        {
            sumOD += overdraft.Amount.Value;
        }
        return sumOD;
    }


    private void OnPercentScrollbar(float monthValue)
    {
        monthAmountPanel = (int)(monthValue*monthScrollbar.numberOfSteps)+3;
        monthAmountText.text = monthAmountPanel.ToString();
        CalculateMonthlyPaymentPanel();
    }

    public void OnSumScrollbar(float sumValue)
    {
        if (!inputLoanState)
        {
            sumAmountPanel = (int)(sumValue * sumLimit);
            sumInputField.text = sumAmountPanel.ToString();
            //print(sumValue);
            CalculateMonthlyPaymentPanel();
        }
    }
    public void OnLoanInputSelected()
    {
        inputLoanState = true;
    }

    public void OnLoanInputDeselected()
    {
        inputLoanState = false;
    }

    public void OnDebtInputSelected()
    {
        inputLoanState = true;
    }

    public void OnDebtInputDeselected()
    {
        inputLoanState = false;
    }
    public void OnSumInputFieldValueChanged()
    {
        print(inputLoanState);
        if (inputLoanState)
        {
            if (int.TryParse(sumInputField.text, out var amount))
            {
                if (amount > sumLimit)
                    sumInputField.text = sumLimit.ToString();
            }
            else sumInputField.text = "0";
            //print(SumInputField.text);
            float sum = float.Parse(sumInputField.text) / sumLimit;
            sumScrollbar.value = sum;
            sumAmountPanel = amount;
            CalculateMonthlyPaymentPanel();
            print("vuv");
        }
    }
    public void OnDebtInputFieldChanged()
    {

        debt = CalculateDebt();
        print(debt);

        if (debt > debtLimit)
            debt = debtLimit;
        if (inputDebtState)
            if (int.TryParse(debtInputField.text, out var amount))
            {
                print(amount);
                if (amount > debt)
                    debtInputField.text = debt.ToString();
            }
    }
    private void CalculateMonthlyPaymentPanel()
    {
        payment.Value = (int)Math.Ceiling((float)(sumAmountPanel / monthAmountPanel) + sumAmountPanel * percent);
        print(payment.Value);
        paymentAmountText.text = payment.Value.ToString();
    }

    public void OnTakeLoanButtonClick() //TODO ������ ���� ������ ����� �����??
    {
        LoanList.Add(new Loan(sumAmountPanel, monthAmountPanel));

        debtAmountText.text = CalculateDebt().ToString();
        print(CalculateDebt());
        CheckDates();
    }

    //private void CalculateMonthlyPayment()
    //{
    //    int sumMounthlyLoan = 0;
    //    foreach (var loan in LoanList)
    //    {
    //        sumMounthlyLoan += (int)Math.Ceiling((float)(loan.Amount.Value / loan.monthAmount) + loan.Amount.Value * percent);
    //    }
    //    int sumMounthlyOD = 0;
    //    foreach (var overdraft in OverdraftList)
    //    {
    //        sumMounthlyOD += overdraft.Amount.Value / overdraft.monthAmount);
    //    }
    //}
    public void OnRepayDebtsButtonClick()
    {
        // ��� �� �������� ����� � ������
        //if( ���-�� ����� > CalculateDebt()){
        OverdraftList.Clear();
        LoanList.Clear();
        debtAmountText.text = CalculateDebt().ToString();
        CheckDates();
    }



}
