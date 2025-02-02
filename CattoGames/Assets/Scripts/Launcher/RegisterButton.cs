
using TMPro;
using UnityEngine;

public class RegisterButton : MonoBehaviour
{
    public TMP_InputField inputEmail;
    public TMP_InputField inputPassword;
    public TMP_InputField inputUsername;
    public GameObject error;

    private string email;
    private string password;
    private string username;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void getRegisterData() {
        this.email = inputEmail.text;
        this.password = inputPassword.text;
        this.username = inputUsername.text;
        Debug.Log("New User: " + username + " " + email + " " + password);
        UserManagement.Register(email, password, username, error);
    }

    public string getEmail() {
        return this.email;
    }

    public string getPassword() {
        return this.password;
    }

    public string getUsername() {
        return this.username;
    }
   
    // Update is called once per frame
    void Update()
    {
        
    }
}
