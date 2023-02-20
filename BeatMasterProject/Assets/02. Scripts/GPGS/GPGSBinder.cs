using UnityEngine;
using System;
using GooglePlayGames;
using GooglePlayGames.BasicApi;


public class GPGSBinder
{
    private static GPGSBinder instance = new GPGSBinder();
    public static GPGSBinder Instance => instance;


    void Init()
    {
        var config = new PlayGamesClientConfiguration.Builder().Build();
        PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.DebugLogEnabled = true;
        PlayGamesPlatform.Activate();
    }


    public void Login(Action<bool, UnityEngine.SocialPlatforms.ILocalUser> onLoginSuccess = null)
    {
        Init();
        PlayGamesPlatform.Instance.Authenticate(SignInInteractivity.CanPromptAlways, (success) =>
        {
            bool isAuthenticated = PlayGamesPlatform.Instance.localUser.authenticated;
            onLoginSuccess?.Invoke(isAuthenticated, Social.localUser);
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
