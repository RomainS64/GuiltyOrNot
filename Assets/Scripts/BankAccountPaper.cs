using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


[Serializable]
public struct AccountTransaction
{
    public TMP_Text text;
    public TMP_Text price;
    public GameObject positiveArrow;
    public GameObject negativeArrow;
}
public class BankAccountPaper : MonoBehaviour
{
    [SerializeField] private int playerId;
    public int GetPlayerId() => playerId;
    
    [SerializeField] private TMP_Text current;
    [SerializeField] private TMP_Text saving;
    
    [SerializeField] private Color positiveColor;
    [SerializeField] private Color negativeColor;
    [SerializeField] private AccountTransaction[] transactions;

    public void SetBankAccountTransactions(float _current, float _saving, List<KeyValuePair<string,float>> _transactions)
    {
        current.text = _current.ToString("F");
        saving.text = _saving.ToString("F");
        int i = 0;
        foreach (var transaction in _transactions)
        {
            bool isPositive = transaction.Value > 0;
            transactions[i].negativeArrow.SetActive(!isPositive);
            transactions[i].positiveArrow.SetActive(isPositive);
            transactions[i].price.color = isPositive ? positiveColor : negativeColor;
            transactions[i].price.text = Mathf.Abs(transaction.Value).ToString("F");
            transactions[i].text.text = transaction.Key;
            ++i;
        }
    }
}