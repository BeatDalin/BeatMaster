using UnityEngine;
using UnityEngine.UI;

public class GoogleLogin : MonoBehaviour
{
    [SerializeField] private Text _status;
    [SerializeField] private Button _logoutBtn;

    private void Start()
    {
        GPGSBinder.Instance.Login((success, localUser) =>
        _status.text = (success ? localUser.userName : "Login Failed"));
        _logoutBtn.onClick.AddListener(Logout);
    }

    private void Logout()
    {
        GPGSBinder.Instance.Logout();
        _status.text = "Logout";
    }
}
