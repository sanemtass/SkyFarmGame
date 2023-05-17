using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public TextMeshProUGUI goldText;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        GameManager.Instance.OnGoldChanged += UpdateGoldText;
    }

    private void UpdateGoldText(int goldAmount)
    {
        goldText.text = goldAmount.ToString();
    }
}
