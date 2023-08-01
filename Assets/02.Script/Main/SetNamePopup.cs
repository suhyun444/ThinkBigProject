using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SetNamePopup : MonoBehaviour
{
    [SerializeField] private GameObject ui;
    [SerializeField] private AWSConnection connection;
    [SerializeField] private CustomButton acceptButton;
    [SerializeField] private CustomButton updateButton;
    [SerializeField] private CustomButton exitButton;
    [SerializeField] private TextMeshProUGUI inputField;
    [SerializeField] private GameObject enterNameWarningText;
    [SerializeField] private GameObject warningText;

    private float warningTime = 0.0f;
    private void Awake() {
        acceptButton.BindClickEvent(SetName);
        updateButton.BindClickEvent(UpdateName);
        exitButton.BindClickEvent(()=>ui.SetActive(false));
    }
    private void Update() {
        warningTime -= Time.deltaTime;
        if(warningTime <= 0.0f)
        {
            warningText.SetActive(false);
            enterNameWarningText.SetActive(false);
        }
    }
    public void OpenSetNameUI()
    {
        ui.SetActive(true);
        warningText.SetActive(false);
        acceptButton.gameObject.SetActive(true);
        updateButton.gameObject.SetActive(false);
        exitButton.gameObject.SetActive(false);
    }
    public void OpenUpdateNameUI()
    {
        ui.SetActive(true);
        warningText.SetActive(false);
        acceptButton.gameObject.SetActive(false);
        updateButton.gameObject.SetActive(true);
        exitButton.gameObject.SetActive(true);
    }
    public void SetName()
    {
        if(inputField.text == "​")
        {
            warningTime = 0.7f;
            enterNameWarningText.SetActive(true);
            return;
        }
        if(connection.FindPlayer(inputField.text))
        {
            warningTime = 0.7f;
            warningText.SetActive(true);
            return;
        }
        connection.InitPlayer(inputField.text);
        SaveManager.Instance.SetNameDate(inputField.text);
        SaveManager.Instance.SaveData();
        ui.SetActive(false);
    }
    public void UpdateName()
    {
        if (inputField.text == "​") 
        {
            warningTime = 0.7f;
            enterNameWarningText.SetActive(true);
            return;
        }
        if (connection.FindPlayer(inputField.text))
        {
            warningTime = 0.7f;
            warningText.SetActive(true);
            return;
        }
        connection.UpdateName(SaveManager.Instance.GetNameData(),inputField.text);
        SaveManager.Instance.SetNameDate(inputField.text);
        SaveManager.Instance.SaveData();
        ui.SetActive(false);
    }
}
