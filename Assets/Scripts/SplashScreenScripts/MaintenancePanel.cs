
using UnityEngine;


public class MaintenancePanel : MonoBehaviour
{
    #region[Member Variables]
    public GameObject UIPanel;
    public GameObject MaintenacePanel;
    public static MaintenancePanel MaintenancePanel_intance;
    #endregion

    #region[Monobehaviour Methods]
    private void Awake()
    {
        if (MaintenancePanel_intance == null)
        {
            MaintenancePanel_intance = this;
        }
        if (MaintenacePanel.activeSelf)
        {
            MaintenacePanel.SetActive(false);
        }
    }
    #endregion

    #region[Custom Methods]
    public void Contact_usbtn()
    {
        Application.OpenURL("tel://9818336734");
    }
    public void ShowEmergencyPanel()
    {
        MaintenacePanel.SetActive(true);
    }
    public void closeBtn()
    {
        Application.Quit();
    }
    #endregion
}
