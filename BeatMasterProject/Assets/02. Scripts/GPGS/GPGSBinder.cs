using UnityEngine;
using System;
using GooglePlayGames;
using GooglePlayGames.BasicApi;

public class GPGSBinder
{
    private static GPGSBinder instance = new GPGSBinder();
    public static GPGSBinder Instance => instance;

    public PlayGamesClientConfiguration config;
    private string _authCode;

    private void Init()
    {
        config = new PlayGamesClientConfiguration.Builder()
            .RequestEmail() // Get Email address to save
            .AddOauthScope("email") // You need to request the Email before asking for it
            .RequestServerAuthCode(false /* Don't force refresh */)
            .Build();
        PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.DebugLogEnabled = true;
        PlayGamesPlatform.Activate();
    }
    
    public void Login(Action<bool, UnityEngine.SocialPlatforms.ILocalUser> onLoginSuccess = null)
    {
        Init();
        PlayGamesPlatform.Instance.Authenticate(SignInInteractivity.CanPromptOnce, (success) =>
        {
            // bool isAuthenticated = PlayGamesPlatform.Instance.localUser.authenticated;
            // // _authCode = PlayGamesPlatform.Instance.GetServerAuthCode();
            // onLoginSuccess?.Invoke(isAuthenticated, Social.localUser);
            Social.localUser.Authenticate((bool isAuthenticated) => {
                if (isAuthenticated)
                {
                    _authCode = PlayGamesPlatform.Instance.GetServerAuthCode();
                    FirebaseDataManager.Instance.GetCredential(_authCode);
                }
                onLoginSuccess?.Invoke(isAuthenticated, Social.localUser);
            });
        });
    }

    public void Logout()
    {
        PlayGamesPlatform.Instance.SignOut();
    }


    public void ShowAchievementUI() =>
        Social.ShowAchievementsUI();

    public void UnlockAchievement(string gpgsId, Action<bool> onUnlocked = null) =>
        Social.ReportProgress(gpgsId, 100, success => onUnlocked?.Invoke(success));
}
