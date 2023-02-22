using UnityEngine;
using UnityEngine.UI;

public class GoogleLogin : MonoBehaviour
{
    [SerializeField] private Text _status;
    private bool _isLogin;
    private void Start()
    {
        //GPGSBinder.Instance.Login((success, localUser) => );
        //_logoutBtn.onClick.AddListener(Logout);
    }

    private void Logout()
    {
        GPGSBinder.Instance.Logout();
        _status.text = "Logout";
    }
}
