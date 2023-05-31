using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public TextMeshProUGUI goldText;
    public TextMeshProUGUI areaValueText;

    public Button switchButton;
    public Button addAreaButton;
    public Button addNewLandButton;
    public PlantedAreaSpawner currentPlantedAreaSpawner;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        GameManager.Instance.OnGoldChanged += UpdateGoldText;
        GameManager.Instance.OnGoldChanged += CheckGoldValueForArea;
        switchButton.onClick.AddListener(SwitchChildAndHandObject);
        addAreaButton.onClick.AddListener(AddAreaValue);
        areaValueText.text = GameManager.Instance.areaValue.ToString();
        addNewLandButton.onClick.AddListener(GameManager.Instance.MoveCameraAndObject);

        currentPlantedAreaSpawner = GameManager.Instance.firstAreaSpawner;
    }

    private void CheckGoldValueForArea(int goldAmount)
    {
        // Gold amount is less than areaValue, disable the button
        if (goldAmount < GameManager.Instance.areaValue)
        {
            addAreaButton.interactable = false;
        }
        else
        {
            addAreaButton.interactable = true;
        }
    }

    private void AddAreaValue()
    {
        if (GameManager.Instance.gold.count >= GameManager.Instance.areaValue)
        {
            GameManager.Instance.ChangeGold(-GameManager.Instance.areaValue);
            GameManager.Instance.areaValue += 5;
            areaValueText.text = GameManager.Instance.areaValue.ToString();
            currentPlantedAreaSpawner.SpawnPlantedArea();
        }
        else
        {
            Debug.Log("Not enough gold!");
        }
    }

    private void UpdateGoldText(int goldAmount)
    {
        goldText.text = goldAmount.ToString();
    }

    private void SwitchChildAndHandObject()
    {
        GameManager.Instance.SwitchChildObject();
    }

    public void ShowAddNewLandButton()
    {
        addNewLandButton.gameObject.SetActive(true);
        addNewLandButton.onClick.AddListener(HideAddNewLandButtonAndMoveCameraAndObject);
    }

    private void HideAddNewLandButtonAndMoveCameraAndObject()
    {
        addNewLandButton.gameObject.SetActive(false);
        GameManager.Instance.MoveCameraAndObject();
    }
}