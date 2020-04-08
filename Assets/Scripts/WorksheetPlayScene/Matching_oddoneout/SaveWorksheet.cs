using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class SaveWorksheet : MonoBehaviour
{
    public Button SaveWoksheetBtn;
    public GameObject Toast;
    private void Awake()
    {
        Toast.SetActive(false);
    }
    private void Start()
    {
        SaveWoksheetBtn.onClick.AddListener(SaveWorksheet_BtnControl);
    }
    public void SaveWorksheet_BtnControl()
    {
        Activate_Toast();
        Invoke("Deactivate_Active", 1.5f);
    }
    void Activate_Toast()
    {
        Toast.SetActive(true);
    }
    void Deactivate_Active()
    {
        Toast.SetActive(false);
    }

}
