using UnityEngine;
using UnityEngine.UI;

public class GoogleLogin : MonoBehaviour
{
    [SerializeField] private Text _status;
    private void Start()
    {
        GPGSBinder.Instance.Login((success, localUser) =>
        {
            if (success)
            {
                SuccessLogin(localUser);
            }
            else
            {
                FailLogin();
            }
        });
    }

    private void SuccessLogin(UnityEngine.SocialPlatforms.ILocalUser localUser)
    {
        _status.text = localUser.userName;
    }

    private void FailLogin()
    {
        //로그인 실패했을 때 로그인해야 게임을 만들 수 있습니다 panel 뜨고
        //Button은 확인 AddListener로 Application.Quit();추가
    }

    private void Logout()
    {
        GPGSBinder.Instance.Logout();
        _status.text = "Logout";
    }
}
