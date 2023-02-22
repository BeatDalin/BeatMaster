using UnityEngine;
using UnityEngine.UI;

public class GoogleLogin : MonoBehaviour
{
    [SerializeField] private Text _status;
    [SerializeField] private Button _achieveBtn;
    [SerializeField] private Button _okBtn;
    [SerializeField] private GameObject _quitPanel;
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
                _quitPanel.SetActive(true);
            }
        });
        _achieveBtn.onClick.AddListener(() => GPGSBinder.Instance.ShowAchievementUI());
        _okBtn.onClick.AddListener(() => FailLogin());
    }

    private void SuccessLogin(UnityEngine.SocialPlatforms.ILocalUser localUser)
    {
        _status.text = localUser.userName;
    }

    private void FailLogin()
    {
        Application.Quit();
    }

    private void Logout()
    {
        GPGSBinder.Instance.Logout();
        _status.text = "Logout";
    }
}
