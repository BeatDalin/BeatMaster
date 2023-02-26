using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Auth;
using Firebase.Extensions;
using GooglePlayGames;


public class FirebaseDataManager : Singleton<FirebaseDataManager>
{
    [Header("Firebase Auth")]
    private DatabaseReference _dbReference;
    private FirebaseAuth _auth;
    private FirebaseUser _userInFirebase; // will get info from Google Play Account
    private const string _userDataKey = "users";
    
    [Space]
    [Header("Google Play")] 
    // private PlayGamesClientConfiguration _config; 
    private string _authCode;
    public bool isAuthenticated;
    private WaitUntil _waitUntilAuthenticated;
    
    [Space]
    [Header("User Checking")]
    [SerializeField] private bool _isMyUserFound; // True if user already exists in Firebase database
    [SerializeField] private bool _isUserSkimEnd;
    private WaitUntil _waitUntilUserSkim; 
    [SerializeField] private bool _isUserSearchEnd;
    private WaitUntil _waitForSearchEnd;
    [SerializeField] private UserInfo _myUserInfo;
    
    [Space]
    [Header("Catalogue")]
    private Dictionary<string, ProductInfo> _productCatalogue = new Dictionary<string, ProductInfo>();
    private bool _isCatalogueLoaded;
    private WaitUntil _waitForCatalogueLoad;

    // [Space] 
    [Header("Test in Unity")] 
    // [SerializeField] private string _userIDToTest; // enter user id to test in the Inspector
    [SerializeField] private ProductInfo _mySunglasses;
    [SerializeField] private ProductInfo _coins;

    public override void Init()
    {
        DontDestroyOnLoad(this);
        FirebaseApp.DefaultInstance.Options.DatabaseUrl = new System.Uri("https://beatmaster-60220435-default-rtdb.firebaseio.com/");
        _dbReference = FirebaseDatabase.DefaultInstance.RootReference;
        // Firebase Auth Setting
        InitializeFirebaseAuth();
        
        // Set WaitUntils : Wait Until boolean variables become true
        _waitUntilAuthenticated = new WaitUntil(() => isAuthenticated);
        _waitUntilUserSkim = new WaitUntil(() => _isUserSkimEnd);
        _waitForSearchEnd = new WaitUntil(() => _isUserSearchEnd);
        _waitForCatalogueLoad = new WaitUntil(() => _isCatalogueLoaded);
        
        // Get Catalogue from Firebase Database
        GetProductCatalogue(); // Load Product Info from Firebase Database
        StartCoroutine(CoWaitForCatalogue());
        
        // Wait Google Play Log in and Find User
        StartCoroutine(CoFindUser());
    }

    #region Retrieve User Info

    public void GetCredential(string authCode)
    {
        // Authenticate user using Firebase.Auth
        Debug.Log($"=================== Auth code exist? {authCode}");
        if (_authCode == authCode)
        {
            // _authCode is already set -> Already authenticated, No need to execute GetCredential
            return;
        }
        
        if (_auth == null)
        {
            // _auth cannot be null, but just in case..
            _auth = FirebaseAuth.DefaultInstance;
        }

        _authCode = authCode;
        Credential credential = PlayGamesAuthProvider.GetCredential(authCode);
        _auth.SignInWithCredentialAsync(credential).ContinueWithOnMainThread(task => {
            if (task.IsCanceled) {
                Debug.LogError("SignInWithCredentialAsync was canceled.");
                return;
            }
            if (task.IsFaulted) {
                Debug.LogError("SignInWithCredentialAsync encountered an error: " + task.Exception);
                return;
            }

            // task.IsCompleted
            _userInFirebase = task.Result; // same as _auth.CurrentUser
            Debug.LogFormat("User signed in successfully: {0} ({1})", _userInFirebase.DisplayName, _userInFirebase.UserId);
            isAuthenticated = true;
        });
    }

    private IEnumerator CoFindUser()
    {
        // #if UNITY_EDITOR
        //     isAuthenticated = true;
        // #endif
        yield return _waitUntilAuthenticated;
        SkimUserInfo(); // Check whether current user exists in Firebase Database
        
        yield return _waitUntilUserSkim;
        // #if UNITY_EDITOR
        //     GetUserInfo(_userIDToTest);
        // #endif
        GetUserInfo(_userInFirebase.UserId); // Create or Load User information based on boolean variable, _isMyUserFound
    }

    private void SkimUserInfo()
    {
        // Get information under "users"
        DatabaseReference uiReference = FirebaseDatabase.DefaultInstance.GetReference(_userDataKey);

        Debug.Log("####### SKIM ALL USER INFO ########");
        uiReference.GetValueAsync().ContinueWith(
            task =>
            {
                if (task.IsCompleted)
                {
                    DataSnapshot snapshot = task.Result;
                    // Debug.Log(snapshot.Child(_userDataKey).Child(_userId).Child("walletString"));
                    // These do not work...                    
                    // Debug.Log(snapshot.Child(_userDataKey).Child(_userFromGP.UserId).Child("name"));
                    // Debug.Log(snapshot.Child(_userDataKey).Child("u6zUWPD9IZSqk514E6mTIHxpfnf2").Child("name"));
                    
                    foreach (DataSnapshot data in snapshot.Children)
                    {
                        // Children under users => Each individual users
                        Debug.Log($"data in users : {data.Key}");
                        // #if UNITY_EDITOR
                        //     if (data.Key.Equals(_userIDToTest))
                        //     {
                        //         // My user Exists
                        //         _isMyUserFound = true;
                        //     }
                        // #endif
                        if (data.Key.Equals(_userInFirebase.UserId))
                        {
                            // My user Exists
                            _isMyUserFound = true;
                            _isUserSkimEnd = true;
                        }
                        IDictionary userInfo = (IDictionary)data.Value;
                        if (!string.IsNullOrEmpty(data.GetRawJsonValue()))
                        {
                            var info = JsonUtility.FromJson<UserInfo>(data.GetRawJsonValue());
                            Debug.Log($"USER Name: {info.name} Email: {info.email} Wallet: {info.wallet} WalletString: {info.walletString}");
                        }
                        // Read all users (Both way works!)
                        Debug.Log($"USER Name: {userInfo["name"]} Email: {userInfo["email"]} Wallet: {userInfo["wallet"]} WalletString: {userInfo["walletString"]}");
                        Debug.Log("---------------------------------------");
                    }
                    Debug.Log("####### SKIM ALL USER INFO Ends ########");
                    _isUserSkimEnd = true;
                }
            }
        );
    }

    public void GetUserInfo(string userId)
    {
        // Get "users" from Firebase
        DatabaseReference uiReference = FirebaseDatabase.DefaultInstance.GetReference(_userDataKey);
        _isUserSearchEnd = false;
        uiReference.GetValueAsync().ContinueWith(
            task =>
            {
                if (task.IsCompleted)
                {
                    Debug.Log("########### Get My User ###########");
                    DataSnapshot snapshot = task.Result;
                    // Check DB First -> Get or Make user info
                    
                    if (!_isMyUserFound)
                    {
                        // User Not Found => Create User
                        Debug.Log("!!!!!!!! User Not Found !!!!!!!");
                        _myUserInfo = new UserInfo();

                        // Create User with Google Play Account Info
                        _myUserInfo = _myUserInfo.CreateUserInfo(
                            _userInFirebase.UserId, // User id from Firebase Auth
                            ((PlayGamesLocalUser)Social.localUser).Email, // email
                            _userInFirebase.DisplayName); // Google Play Account Name
                        CreateUserWithJson(_userInFirebase.UserId, _myUserInfo);
                        Debug.Log("New User: " + JsonUtility.ToJson(_myUserInfo));
                        
                        _isUserSearchEnd = true;
                        return;
                    }
                    
                    // Debug.Log($"############ Google Play Email ? {_userFromGP.Email}");
                    
                    // foreach (DataSnapshot data in snapshot.Children)
                    // {
                    //     // JSON 자체가 딕셔너리 기반
                    //     IDictionary userDict = (IDictionary)data.Value;
                    //     // Dictionary<string, object> userInfo = (Dictionary<string, object>)data.Value;
                    //     Debug.Log(userDict["userId"]);
                    //     if (userDict["userId"].Equals(userId))
                    //     {
                    //         _myUserInfo = JsonUtility.FromJson<UserInfo>(userDict.ToString());
                    //         Debug.Log($"Name: {userDict["name"]} Email: {userDict["email"]}  ");/*prodList: {userDict["prodList"]}*/
                    //         break;
                    //     }
                    //     
                    // }
                    _myUserInfo = JsonUtility.FromJson<UserInfo>(snapshot.Child(userId).GetRawJsonValue());
                    _myUserInfo.ConvertWalletFromString(); // Make wallet from wallet string
                    Debug.Log("########### Get My User End!!! ###########");
                    _isUserSearchEnd = true;
                }
                else if (task.IsFaulted)
                {
                    Debug.Log("Faulted!");
                    _isUserSearchEnd = true;
                }

            });
    }
    #endregion
    
    
    private IEnumerator CoBuyConsumeTest()
    {
        yield return _waitForSearchEnd;
        _isUserSearchEnd = false;
        
        Debug.Log("User From Database: " + JsonUtility.ToJson(_myUserInfo));

        
        Debug.Log("########### Buy Product Test ###########");
        // #################### Buy product Test #######################
        bool resultCoin = _myUserInfo.BuyProduct("coin500", _productCatalogue);
        //  coin 생겼는지 확인
        Debug.Log("Coin : " + _myUserInfo.wallet["coin500"]);
        Debug.Log($"User after buy coin: {resultCoin} "+JsonUtility.ToJson(_myUserInfo));
        
        bool result = _myUserInfo.BuyProduct("fancySunglasses", _productCatalogue);
        Debug.Log("Fancy Sunglasses : " + _myUserInfo.wallet["fancySunglasses"]);
        Debug.Log($"User after buy sunglasses: {result} "+JsonUtility.ToJson(_myUserInfo));
        // UpdateUserInfo(_userIDToTest, _myUserInfo);
        UpdateUserInfo(_userInFirebase.UserId, _myUserInfo);


        // ################# Consume Test #####################
        _myUserInfo.ConsumeProduct("sunglasses", 1, _productCatalogue);
        _myUserInfo.ConsumeProduct("fancySunglasses", 1, _productCatalogue);
        
        _myUserInfo.ConsumeProduct("coin500", 500, _productCatalogue);
        // UpdateUserInfo(_userIDToTest, _myUserInfo);
        UpdateUserInfo(_userInFirebase.UserId, _myUserInfo);
    }


    public void CreateUserWithJson(string userId, UserInfo userInfo)
    {
        string json = JsonUtility.ToJson(userInfo);
        Debug.Log("CreateUser: "+json);
        _dbReference.Child(_userDataKey).Child(userId).SetRawJsonValueAsync(json);
    }
 
    public void CreateUserWithPath(string userId, UserInfo userInfo)
    {
        _dbReference.Child("users").Child(userId).Child("email").SetValueAsync(userInfo.email);
        _dbReference.Child("users").Child(userId).Child("name").SetValueAsync(userInfo.name);
        // _dbReference.Child("users").Child(userId).Child("wallet")
        //     .SetRawJsonValueAsync(JsonUtility.ToJson(userInfo.wallet));
    }
 
    public void UpdateUserInfo(string uID, UserInfo userInfo)
    {
        // Data가 새로 추가 되어서 이건 못 쓰겠다.
        // _dbReference.Child("users").Child(userId).SetRawJsonValueAsync(JsonUtility.ToJson(userInfo)).ContinueWith(
        //     task =>
        //     {
        //         if (task.IsCompleted)
        //         {
        //             Debug.Log("Data Saved");
        //         }
        //         else
        //         {
        //             Debug.Log("Failed");
        //         }
        //     });
        
        //다른 자식 노드를 덮어쓰지 않고 노드의 특정 자식에 동시에 쓰려면 UpdateChildrenAsync() 메서드를 사용합니다.
        // UpdateChildrenAsync() 를 호출할 때 키의 경로를 지정하여 하위 수준 자식 값을 업데이트할 수 있습니다. 
        Debug.Log("################ UPDATE INFO IN FIREBASE #################");
        _dbReference.Child(_userDataKey).Child(uID).UpdateChildrenAsync(userInfo.ToDictionary());
    }

    // public void PushUserInfo(UserInfo userInfo)
    // {
    //     // Push를 이용하면, _userID로 적용했던 부분이 임의의 키로 설정되고, 이를 반환해줌
    //     // string key = _dbReference.Child("users").Push().Key;
    //     // _dbReference.Child("users").Child(key).Child("name").SetValueAsync(userInfo.name);
    //     // _dbReference.Child("users").Child(key).Child("score").SetValueAsync(userInfo.email);
    //
    //     // string key = _dbReference.Child("users").Child(_userId).Push().Key;
    //     // _dbReference.Child("users").Child(userInfo.userId).Child("name").SetValueAsync(userInfo.name);
    //     // _dbReference.Child("users").Child(userInfo.userId).Child("email").SetValueAsync(userInfo.email);
    // }

 
    public void RemoveUserInfo(string _userID)
    {
        _dbReference.Child("users").Child(_userID).RemoveValueAsync();
    }


    #region FirebaseAuth

    // Handle initialization of the necessary firebase modules:
    void InitializeFirebaseAuth() {
        Debug.Log("Setting up Firebase Auth");
        _auth = FirebaseAuth.DefaultInstance;
        _auth.StateChanged += AuthStateChanged;
        AuthStateChanged(this, null);
    }
    

    // Track state changes of the auth object.
    void AuthStateChanged(object sender, System.EventArgs eventArgs)
    {
        if (_auth.CurrentUser != _userInFirebase) {
            bool signedIn = _userInFirebase != _auth.CurrentUser && _auth.CurrentUser != null;
            if (!signedIn && _userInFirebase != null) {
                Debug.Log("Signed out " + _userInFirebase.UserId);
            }
            _userInFirebase = _auth.CurrentUser;
            if (signedIn) {
                Debug.Log("Signed in " + _userInFirebase.UserId);
            }
        }
    }

    // Handle removing subscription and reference to the Auth instance.
    // Automatically called by a Monobehaviour after Destroy is called on it.
    void OnDestroy() {
        _auth.StateChanged -= AuthStateChanged;
        _auth = null;
    }

    #endregion
    

    #region Product Info

    /// <summary>
    /// Execute only when you want to add new items.
    /// </summary>
    private void InitProductInfo()
    {
        var product = new ProductInfo("coin500", "Coins 100", "100", true, 100);
        _dbReference.Child("productCatalogue").Child(product.id).SetRawJsonValueAsync(JsonUtility.ToJson(product));

        product = new ProductInfo("fancySunglasses", "Fancy Sunglasses", "300", false, 1);
        _dbReference.Child("productCatalogue").Child(product.id).SetRawJsonValueAsync(JsonUtility.ToJson(product));
    }

    private void GetProductCatalogue()
    {
        // Get "productCatalogue" from Firebase
        DatabaseReference uiReference = FirebaseDatabase.DefaultInstance.GetReference("productCatalogue");
        uiReference.GetValueAsync().ContinueWith(
            task =>
            {
                if (task.IsCompleted)
                {
                    Debug.Log("######## GET PRODUCT CATALOGUE ########");
                    DataSnapshot snapshot = task.Result;
                    foreach (var data in snapshot.Children)
                    {
                        Debug.Log(data.GetRawJsonValue());
                        ProductInfo info = JsonUtility.FromJson<ProductInfo>(data.GetRawJsonValue());
                        _productCatalogue.Add(info.id, info);
                        
                        Debug.Log($"Increase Amount of {_productCatalogue[info.id].id}: {_productCatalogue[info.id].increaseAmount}");
                    }
                    Debug.Log("######## GET PRODUCT CATALOGUE Ended ########");
                    _isCatalogueLoaded = true;
                }
            });
    }
    
    private IEnumerator CoWaitForCatalogue()
    {
        yield return _waitForCatalogueLoad;
        Debug.Log("####### Catalogue Load Ended! #######");
        
        // ############## Product ################
        // If products are loaded in the inspector, the product catalogue has been created well
        _mySunglasses = _productCatalogue["fancySunglasses"];
        _coins = _productCatalogue["coin500"];
    } 

    #endregion

}
