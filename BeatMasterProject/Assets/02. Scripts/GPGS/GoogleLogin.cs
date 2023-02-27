using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GoogleLogin : MonoBehaviour
{
    [SerializeField] private Text _status;
    [SerializeField] private Button _achieveBtn;
    [SerializeField] private Button _okBtn;
    [SerializeField] private Button _leaderboardBtn;
    [SerializeField] private GameObject _quitPanel;
#if !UNITY_EDITOR
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
        _leaderboardBtn.onClick.AddListener(() => GPGSBinder.Instance.ShowAllLeaderboardUI());
    }
#endif

    private void SuccessLogin(UnityEngine.SocialPlatforms.ILocalUser localUser)
    {
        _status.text = localUser.userName;
    }

    private void FailLogin()
    {
        _okBtn.interactable = false;
        GPGSBinder.Instance.Login((success, localUser) =>
        {
            if (success)
            {
                SuccessLogin(localUser);
                _quitPanel.SetActive(false);
            }
            else
            {
                StartCoroutine(CoRetryLogin());
            }
        });
    }

    private IEnumerator CoRetryLogin()
    {
        yield return new WaitForSeconds(1f);

        _okBtn.interactable = true;
    }

    /*private IEnumerator CoGoogleLoginCoroutine()
    {
        while (!GPGSBinder.Instance.loginCheck)
        {
            GPGSBinder.Instance.Login((success, localUser) =>
            {
                if (success)
                {
                    SuccessLogin(localUser);
                    GPGSBinder.Instance.loginCheck = true;
                }
            });
            yield return new WaitForSeconds(1f);
        }
        _quitPanel.SetActive(false);
    }*/

    private void Logout()
    {
        GPGSBinder.Instance.Logout();
        _status.text = "Logout";
    }
}
