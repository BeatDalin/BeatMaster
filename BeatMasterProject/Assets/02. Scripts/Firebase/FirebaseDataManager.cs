using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
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
    [SerializeField] private bool _isMyUserFound; // true if user already exists in Firebase Database
    [SerializeField] private bool _isUserSkimEnd; // true once skimming all users in Firebase Database is done
    private WaitUntil _waitUntilUserSkim; 
    [SerializeField] private bool _isUserSearchEnd; // true if loading user info from Firebase Database to _myUserInfo 
    public WaitUntil waitForSearchEnd; 
    [SerializeField] private UserInfo _myUserInfo;
    
    [Space]
    [Header("Catalog")]
    public Dictionary<string, ProductInfo> productCatalog = new Dictionary<string, ProductInfo>();
    private bool _isCatalogueLoaded;
    public WaitUntil waitForCatalogLoad;

    public override void Init()
    {
        DontDestroyOnLoad(this);
        // Firebase Database Setting
        FirebaseApp.DefaultInstance.Options.DatabaseUrl = new Uri("https://beatmaster-60220435-default-rtdb.firebaseio.com/");
        _dbReference = FirebaseDatabase.DefaultInstance.RootReference;
        // Firebase Auth Setting
        InitializeFirebaseAuth();
        
        // Set WaitUntils : Wait Until boolean variables become true
        _waitUntilAuthenticated = new WaitUntil(() => isAuthenticated);
        _waitUntilUserSkim = new WaitUntil(() => _isUserSkimEnd);
        waitForSearchEnd = new WaitUntil(() => _isUserSearchEnd);
        waitForCatalogLoad = new WaitUntil(() => _isCatalogueLoaded);
        
        // Get Catalogue from Firebase Database
        GetProductCatalogue(); // Load Product Info from Firebase Database
        StartCoroutine(CoWaitForCatalog());
        
        // Wait Google Play Log in and Find User
        StartCoroutine(CoFindUser());
        
#if UNITY_EDITOR
        _myUserInfo = _myUserInfo.CreateUserInfo("testUserID", "test.test.com", "Test User");
        _isUserSearchEnd = true;
#endif
    }

    #region Retrieve User Info

    /// <summary>
    /// After Google Play Login is done, find user id of Firebase Authentication. Using authentication code from Google Play as key, access to Firebase Authentication.
    /// </summary>
    /// <param name="authCode">Authentication code provided by Google Play Games Authentication</param>
    public void GetCredential(string authCode)
    {
        // Authenticate user using Firebase.Auth
        // Check authCode when you think it is null.... Debug.Log($"=================== Auth code exist? {authCode}");
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
    
    /// <summary>
    /// Start skimming all users after the user's Google Play id is authenticated.
    /// Get user info if user's info is found in Firebase Database. Otherwise, create one and write new user's info in Firebase Database. 
    /// </summary>
    /// <returns>_waitUntilAuthenticated (wait until user is authenticated), _waitUntilUserSkim (wait until skimming user database is done)</returns>
    private IEnumerator CoFindUser()
    {
        yield return _waitUntilAuthenticated;
        SkimUserInfo(); // Check whether current user exists in Firebase Database
        
        yield return _waitUntilUserSkim;
        GetUserInfo(_userInFirebase.UserId); // Create or Load User information based on boolean variable, _isMyUserFound
    }

    /// <summary>
    /// Skim every user in Firebase Database and check whether current Google Play User does exist.
    /// </summary>
    private void SkimUserInfo()
    {
        // Get information under "users"
        DatabaseReference uiReference = FirebaseDatabase.DefaultInstance.GetReference(_userDataKey);

        // ####### SKIM ALL USER INFO ########
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
                        if (data.Key.Equals(_userInFirebase.UserId))
                        {
                            // My user Exists
                            _isMyUserFound = true;
                            _isUserSkimEnd = true;
                        }
                        // IDictionary userInfo = (IDictionary)data.Value;
                        // if (!string.IsNullOrEmpty(data.GetRawJsonValue()))
                        // {
                        //     var info = JsonUtility.FromJson<UserInfo>(data.GetRawJsonValue());
                        //     Debug.Log($"USER Name: {info.name} Email: {info.email} Wallet: {info.wallet} WalletString: {info.walletString}");
                        // }
                        // Read all users (Both way works!)
                        // Debug.Log($"USER Name: {userInfo["name"]} Email: {userInfo["email"]} Wallet: {userInfo["wallet"]} WalletString: {userInfo["walletString"]}");
                        // Debug.Log("---------------------------------------");
                    }
                    // ####### SKIM ALL USER INFO ENDS #######
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
                    // ########### Get My User ###########
                    // Check DB First -> Get or Make user info
                    DataSnapshot snapshot = task.Result;
                    if (!_isMyUserFound)
                    {
                        // User Not Found => Create User
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
                    _myUserInfo = JsonUtility.FromJson<UserInfo>(snapshot.Child(userId).GetRawJsonValue());
                    _myUserInfo.ConvertWalletFromString(); // Make wallet from wallet string
                    // ########### Get My User End ###########
                    _isUserSearchEnd = true;
                }
                else if (task.IsFaulted)
                {
                    Debug.Log("Faulted!");
                    _isUserSearchEnd = true;
                }

            }
        );
    }
    #endregion

    #region Firebase Database
    
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
    }
 
    private void UpdateUserInfo(string userID, UserInfo userInfo)
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
        
        //다 른 자식 노드를 덮어쓰지 않고 노드의 특정 자식에 동시에 쓰려면 UpdateChildrenAsync() 메서드를 사용합니다.
        // UpdateChildrenAsync() 를 호출할 때 키의 경로를 지정하여 하위 수준 자식 값을 업데이트할 수 있습니다. 
        // ################ UPDATE INFO IN FIREBASE #################
        _dbReference.Child(_userDataKey).Child(userID).UpdateChildrenAsync(userInfo.ToDictionary());
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <returns>true if updating was successful</returns>
    public bool UpdateInfo()
    {
        try
        {
            _dbReference.Child(_userDataKey).Child(_myUserInfo.userId).UpdateChildrenAsync(_myUserInfo.ToDictionary());
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

        return true;
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
    

    #endregion
    
    #region FirebaseAuth

    // Handle initialization of the necessary firebase modules:
    private void InitializeFirebaseAuth()
    {
        Debug.Log("Setting up Firebase Auth");
        _auth = FirebaseAuth.DefaultInstance;
        _auth.StateChanged += AuthStateChanged;
        AuthStateChanged(this, null);
    }
    

    // Track state changes of the auth object.
    private void AuthStateChanged(object sender, System.EventArgs eventArgs)
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
    private void OnDestroy() {
        _auth.StateChanged -= AuthStateChanged;
        _auth = null;
    }

    #endregion
    

    #region Product Info

    /// <summary>
    /// Execute only when you want to add new items.
    /// </summary>
    public void InitProductInfo()
    {
        // var product = new ProductInfo("coin100", "Coins 100", "You can buy more stuff using these special coins....", "0.99", true, 100);
        // _dbReference.Child("productCatalogue").Child(product.id).SetRawJsonValueAsync(JsonUtility.ToJson(product));

        
        // var product = new ProductInfo("starterPack", "Starter Pack", "You will get fancy sunglasses and a cute pet.", "2.99", true, 100);
        // _dbReference.Child("productCatalogue").Child(product.id).SetRawJsonValueAsync(JsonUtility.ToJson(product));

        // var product = new ProductInfo("petCat", "Pet Cat", "Your tiny pet, Kitty.", "0.99", false, 1);
        // _dbReference.Child("productCatalogue").Child(product.id).SetRawJsonValueAsync(JsonUtility.ToJson(product));
        //
        // product = new ProductInfo("petFox", "Pet Fox", "Your tiny pet, Fox.", "0.99", false, 1);
        // _dbReference.Child("productCatalogue").Child(product.id).SetRawJsonValueAsync(JsonUtility.ToJson(product));
        
        // product = new ProductInfo("effectCapybara", "Capybara Effect", "Capybara will be with you.", "0.99", false, 1);
        // _dbReference.Child("productCatalogue").Child(product.id).SetRawJsonValueAsync(JsonUtility.ToJson(product));

        
        // product = new ProductInfo("fancySunglasses", "Fancy Sunglasses", "What a super fancy sunglasses!","0.99", false, 1);
        // _dbReference.Child("productCatalogue").Child(product.id).SetRawJsonValueAsync(JsonUtility.ToJson(product));
    }

    /// <summary>
    /// Get all product info from Firebase Database
    /// </summary>
    private void GetProductCatalogue()
    {
        // Get "productCatalogue" from Firebase
        DatabaseReference uiReference = FirebaseDatabase.DefaultInstance.GetReference("productCatalogue");
        uiReference.GetValueAsync().ContinueWith(
            task =>
            {
                if (task.IsCompleted)
                {
                    // ######## GET PRODUCT CATALOG ########
                    DataSnapshot snapshot = task.Result;
                    foreach (var data in snapshot.Children)
                    {
                        // if you wanna check each Product Info, try Debug.Log(data.GetRawJsonValue());
                        ProductInfo info = JsonUtility.FromJson<ProductInfo>(data.GetRawJsonValue());
                        if (productCatalog.ContainsKey(info.id))
                        {
                            productCatalog.Remove(info.id); // clean to add
                        }
                        productCatalog.Add(info.id, info);
                    }
                    // ######## GET PRODUCT CATALOG Ended ########
                    _isCatalogueLoaded = true;
                }
            }
        );
    }
    
    private IEnumerator CoWaitForCatalog()
    {
        yield return waitForCatalogLoad;
        // ####### Catalogue Load Ended! #######
    }

    /// <summary>
    /// Reload catalog from Firebase Database. This function is called when product id was not found in productCatalog
    /// </summary>
    public void ReloadCatalog()
    {
        _isCatalogueLoaded = false;
        GetProductCatalogue();
    }
    

    /// <summary>
    /// After In-App Purchase payment was successful, update user's wallet to give product.
    /// </summary>
    /// <param name="productId">string Id of purchased product</param>
    /// <returns>true when the wallet update was successful</returns>
    public bool Purchase(string productId)
    {
        bool result = _myUserInfo.BuyProduct(productId, productCatalog);
        return result;
    }

    /// <summary>
    /// Consume In-App Purchase product which is type of consumable. 
    /// </summary>
    /// <param name="productId">string Id of product to consume</param>
    /// <param name="amountToConsume">the amount of consumption, which will be subtracted from current savings in wallet</param>
    /// <returns>true when the consumption was successful</returns>
    public bool Consume(string productId, int amountToConsume)
    {
        bool result = _myUserInfo.ConsumeProduct(productId, amountToConsume, productCatalog);
        return result;
    }
    
    /// <summary>
    /// Confirms whether user's wallet contains product by checking product id and its amount. Returns true when the user has the product.
    /// </summary>
    /// <param name="productId">string Id of product to check</param>
    /// <returns>true if the product id exists in user's wallet dictionary</returns>
    public bool CheckProductInWallet(string productId)
    {
        if (_myUserInfo.wallet.TryGetValue(productId, out int count))
        {
            return count > 0;
        }
        else
        {
            return false;
        }
    }

    #endregion
    
    #region Test Code

    private IEnumerator CoBuyConsumeTest()
    {
        yield return waitForSearchEnd;
        _isUserSearchEnd = false;
        
        Debug.Log("User From Database: " + JsonUtility.ToJson(_myUserInfo));

        
        Debug.Log("########### Buy Product Test ###########");
        // #################### Buy product Test #######################
        bool resultCoin = _myUserInfo.BuyProduct("coin500", productCatalog);
        //  coin 생겼는지 확인
        Debug.Log("Coin : " + _myUserInfo.wallet["coin500"]);
        Debug.Log($"User after buy coin: {resultCoin} "+JsonUtility.ToJson(_myUserInfo));
        
        bool result = _myUserInfo.BuyProduct("fancySunglasses", productCatalog);
        Debug.Log("Fancy Sunglasses : " + _myUserInfo.wallet["fancySunglasses"]);
        Debug.Log($"User after buy sunglasses: {result} "+JsonUtility.ToJson(_myUserInfo));
        // UpdateUserInfo(_userIDToTest, _myUserInfo);
        UpdateUserInfo(_userInFirebase.UserId, _myUserInfo);


        // ################# Consume Test #####################
        _myUserInfo.ConsumeProduct("sunglasses", 1, productCatalog);
        _myUserInfo.ConsumeProduct("fancySunglasses", 1, productCatalog);
        
        _myUserInfo.ConsumeProduct("coin500", 500, productCatalog);
        // UpdateUserInfo(_userIDToTest, _myUserInfo);
        UpdateUserInfo(_userInFirebase.UserId, _myUserInfo);
    }

    #endregion

}
