using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public TextMeshProUGUI goldText;
    public Button switchButton;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        GameManager.Instance.OnGoldChanged += UpdateGoldText;
        switchButton.onClick.AddListener(SwitchChildAndHandObject);  // metodu ekleyin
    }

    private void UpdateGoldText(int goldAmount)
    {
        goldText.text = goldAmount.ToString();
    }

    private void SwitchChildAndHandObject()
    {
        GameManager.Instance.SwitchChildObject();
    }
}
