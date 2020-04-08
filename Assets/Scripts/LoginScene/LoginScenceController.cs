using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class LoginScenceController : MonoBehaviour
{
    public static LoginScenceController LoginScenceController_Instance;


    [Header("Login Details")]
    string PhoneNumber;
    string Email;
    string Name;
    string DeviceId;

    [Header("Login Fields")]
    public Text MobileNumberText;
    public Text MobileNumberErrorText;
    public InputField NameInputField, EmailInputField,MobileNumberInputField;
  


    [Header("OTP Dialog")]
    public GameObject OTPDialog;
    public InputField OTPText;
    public Text OTPErrorByUserText;
    public Text OTPSubmitLoadingText;
    public Text ResendOTPButton;
    public Text MobileNumberTextOnOTPDialog;

    [Header("Buttons")]
    public Button SendOTPBtn;

    #region PRIVATE_MEMBER_VARIABLES
    private int TotalOTPDigitsFillCounter;
    private int MaxOTPCharacterLimit = 4;
    #endregion

    #region[MonoBehaviour Methods]
    private void Awake()
    {
        if(LoginScenceController_Instance != null)
        {
            LoginScenceController_Instance = this;
        }
        Initialize();
    }

    void Update()
    {
        if (!InternetConnection.InternetConnection_Instance.CheckInternet())
        {
            InternetConnection.InternetConnection_Instance.NetworkErrorPanel.SetActive(true);
        }
    }
    #endregion

    private void Initialize()
    {
        if (OTPText != null)
        {
            OTPText.text = "";
            MaxOTPCharacterLimit = OTPText.characterLimit;
        }
        TotalOTPDigitsFillCounter = 0;
        UserGivenOTPTextClear();
        if (SendOTPBtn != null)
            SendOTPBtn.interactable = true;
    }

    //Get Mobile Number
    public string GetMobileNumber()
    {
        if (MobileNumberText != null)
            return MobileNumberText.text.ToString();
        return null;
    }

    #region [Send OTP on button click]
    public void SendOTP()
    {
        SoundController.soundsControllerInstance.PlayButtonsound();
        PhoneNumber = MobileNumberInputField.text.ToString();
        Email = EmailInputField.GetComponent<InputField>().text.ToString();
        Name = NameInputField.GetComponent<InputField>().text.ToString();
        DeviceId = SystemInfo.deviceUniqueIdentifier.ToString();

        if(!string.IsNullOrEmpty(PhoneNumber)&&PhoneNumber.Length==10&& !string.IsNullOrEmpty(Email)&&validateEmail(Email)&& !string.IsNullOrEmpty(Name))
        {
            StartCoroutine(WebServiceController.webServiceController_Instance.SendOTPUrl(PhoneNumber, Email, Name, DeviceId));
            OTPInputFieldDialogOpen();
            OTPDialog.SetActive(true);
        }
        else
        {
            if (string.IsNullOrEmpty(PhoneNumber)||PhoneNumber.Length!=10)
            {
                MobileNumberInputField.text = "";
                MobileNumberInputField.placeholder.GetComponent<Text>().text = "Enter valid number";
                MobileNumberInputField.placeholder.GetComponent<Text>().color = Color.red;
                Animator anim = MobileNumberInputField.GetComponent<Animator>();
                anim.Play("WrongInputFieldShake");
            }
            if (string.IsNullOrEmpty(Email)|| !validateEmail(Email))
            {
                EmailInputField.text = "";
                EmailInputField.placeholder.GetComponent<Text>().text = "Enter valid email";
                EmailInputField.placeholder.GetComponent<Text>().color = Color.red;
                Animator anim = EmailInputField.GetComponent<Animator>();
                anim.Play("WrongInputFieldShake");
            }
            if (string.IsNullOrEmpty(Name))
            {
                NameInputField.text = "";
                NameInputField.placeholder.GetComponent<Text>().text = "Enter valid name";
                NameInputField.placeholder.GetComponent<Text>().color = Color.red;
                Animator anim = NameInputField.GetComponent<Animator>();
                anim.Play("WrongInputFieldShake");
            }
        }
    }
    #endregion

    #region[Email Validation]
    public const string MatchEmailPattern = @"^(([\w-]+\.)+[\w-]+|([a-zA-Z]{1}|[\w-]{2,}))@" 
                                            + @"((([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\."
                                            + @"([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])){1}|"
                                            + @"([a-zA-Z]+[\w-]+\.)+[a-zA-Z]{2,4})$";
    public static bool validateEmail(string email)
    {
        if (email != null)
            return Regex.IsMatch(email, MatchEmailPattern);
        else
            return false;
    }

    #endregion

    public bool MobileOTPVerification()
    {
        if (WebServiceController.webServiceController_Instance != null)
        {
            if (WebServiceController.webServiceController_Instance.ServerResponseOTP.Equals(GetOTPFromUser()))
                return true;
        }
        return false;
    }

    private string GetOTPFromUser()
    {
        if (OTPText != null)
        {
            if (WebServiceController.webServiceController_Instance != null)
                WebServiceController.webServiceController_Instance.UserGivenOTP = OTPText.GetComponent<InputField>().text.ToString();

            return OTPText.GetComponent<InputField>().text.ToString();
        }
        return "";
    }

    // Dialogs Animations
    public void OTPInputFieldDialogOpen()
    {
        if (MobileNumberTextOnOTPDialog != null)
            MobileNumberTextOnOTPDialog.text = GetMobileNumber();
    }

    public void OTPInputFieldDialogClose()
    {
        if (OTPDialog != null)
        {
            if (OTPText != null)
                OTPText.text = "";
            UserGivenOTPTextClear();
            Debug.Log("OTP Dialog Close");
            OTPDialog.SetActive(false);
            if (SendOTPBtn != null)
                SendOTPBtn.interactable = true;
        }
    }
    

    //clear OTP fill by user
    private void UserGivenOTPTextClear()
    {
        if (WebServiceController.webServiceController_Instance != null)
            WebServiceController.webServiceController_Instance.UserGivenOTP = null;

        OTPText.GetComponent<InputField>().text = null;
        TotalOTPDigitsFillCounter = 0;
    }

    // Auto Process OTP Submit
    public void FillAllOTPDigits()
    {
        if (OTPText != null)
        {
            TotalOTPDigitsFillCounter++;
            MaxOTPCharacterLimit = 4;
            if (TotalOTPDigitsFillCounter == MaxOTPCharacterLimit)
            {
                ApploaderController.ApploaderController_Instance.Activate_Apploader();
                if (InternetConnection.InternetConnection_Instance != null&&InternetConnection.InternetConnection_Instance.CheckInternet())
                {
                    if (MobileOTPVerification())
                    {
                        PhoneNumber = MobileNumberInputField.text.ToString();
                        Email = EmailInputField.text.ToString();
                        Name = NameInputField.text.ToString();
                        DeviceId = SystemInfo.deviceUniqueIdentifier.ToString();
                        WebServiceController.webServiceController_Instance.SubmitDetailsAfterOTP(Email, PhoneNumber, Name, DeviceId);
                    }
                    else
                    {
                        Debug.Log("Please Enter OTP Correct OTP");
                        OTPErrorByUserText.text = "Please enter correct OTP";
                        UserGivenOTPTextClear();
                        ApploaderController.ApploaderController_Instance.Deactivate_Apploader();
                    }
                }
            }
        }
    }

    // Click to chenge number
    public void ClickToChangeBtn()
    {
        MobileNumberText.text = "";
        MobileNumberErrorText.text = "";
        OTPSubmitLoadingText.text = "";
        OTPInputFieldDialogClose();

    }

    // Resend OTP Btn
    public void ResendOTPBtnClicked()
    {
        if (InternetConnection.InternetConnection_Instance != null)
        {
            if (InternetConnection.InternetConnection_Instance.CheckInternet())
            {
                Debug.Log("Resend OTP Btn Clicked:");
                if (WebServiceController.webServiceController_Instance != null)
                {
                    print("Resend OTP");
                    SendOTP();
                }  
            }
            else
            {
                Debug.Log("Please Enter OTP Correct OTP");
                OTPErrorByUserText.text = "Please enter correct OTP";
                UserGivenOTPTextClear();
            }
        }
    }
  
}
