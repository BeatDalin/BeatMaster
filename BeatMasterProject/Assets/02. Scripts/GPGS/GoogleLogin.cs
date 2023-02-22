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
        //�α��� �������� �� �α����ؾ� ������ ���� �� �ֽ��ϴ� panel �߰�
        //Button�� Ȯ�� AddListener�� Application.Quit();�߰�
    }

    private void Logout()
    {
        GPGSBinder.Instance.Logout();
        _status.text = "Logout";
    }
}
