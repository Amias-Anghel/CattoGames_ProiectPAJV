
using TMPro;
using UnityEngine;

public class LoginButton : MonoBehaviour
{
    public TMP_InputField inputEmail;
    public TMP_InputField inputPassword;
    private string email;
    private string password;
    public GameObject registerGroup;
    public GameObject error;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void getLogInData() {
        this.email = inputEmail.text;
        this.password = inputPassword.text;
        Debug.Log(email + " " + password);
        UserManagement.LoginWithEmailAddress(email, password, error);
    }

    public string getEmail() {
        return this.email;
    }

    public string getPassword() {
        return this.password;
    }

     public void showRegisterMenu() {
        registerGroup.SetActive(true);
        transform.parent.gameObject.SetActive(false);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
