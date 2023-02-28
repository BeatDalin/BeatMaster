using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Purchasing;

public class InAppPurchase : MonoBehaviour
{
    private readonly string _idCoin100 = "coin100";
    private readonly string _idSunglasses = "fancySunglasses";
    private readonly string _idPetFox = "petFox";
    private readonly string _idPetCat = "petCat";
    private readonly string _idStarterPack = "starterPack";
    private readonly string _purchasedText = "Purchased";
    [Header("IAP Buttons")]
    [SerializeField] private IAPButton[] _IAPButtons;
    private Dictionary<string, IAPButton> _IAPDict = new Dictionary<string, IAPButton>();
    [Space]
    [Header("Outer Buttons")]
    [SerializeField] private Transform _outerBtnParent;
    private Dictionary<string, Button> _outerBtnDict = new Dictionary<string, Button>();
    private Dictionary<string, Text> _outerPriceDict = new Dictionary<string, Text>();
    
    private ProductCatalog _catalog;
    [SerializeField] private Store _store;
    private WaitForSecondsRealtime _waitForHalfSec;
    
    
    void Start()
    {
        SetDict();
        _catalog = ProductCatalog.LoadDefaultCatalog();
        foreach (var productCatalogItem in _catalog.allProducts)
        {
            var id = productCatalogItem.id.Split('.')[^1];

            if (FirebaseDataManager.Instance.productCatalog.TryGetValue(id, out ProductInfo foundProductInfo))
            {
                SetIAPCatalog(id, foundProductInfo, productCatalogItem);
            }
            else
            {
                // Product Id Key Not Found -> Reload Items
                StartCoroutine(CoWaitCatalog(id, productCatalogItem));
            }
        }
        _waitForHalfSec = new WaitForSecondsRealtime(0.3f);
        StartCoroutine(CoWaitWalletReady());
    }

    /// <summary>
    /// Reload product info from Firebase. Try to set IAP Catalog after reloading product.
    /// </summary>
    /// <param name="productId">Product Id to access Catalog Dictionary</param>
    /// <param name="eachCatalogItem">Each item registered in IAP Catalog</param>
    /// <returns>FirebaseDataManager.Instance._waitForCatalogLoad (wait until catalog is loaded)</returns>
    private IEnumerator CoWaitCatalog(string productId, ProductCatalogItem eachCatalogItem)
    {
        // Catalog Reload when product id was not found in the Catalog Dictionary
        FirebaseDataManager.Instance.ReloadCatalog();
        yield return FirebaseDataManager.Instance.waitForCatalogLoad;

        if (FirebaseDataManager.Instance.productCatalog.TryGetValue(productId, out ProductInfo foundProductInfo))
        {
            SetIAPCatalog(productId, foundProductInfo, eachCatalogItem);
        }
        else
        {
            StartCoroutine(CoWaitCatalog(productId, eachCatalogItem));
        }
    }

    private void SetIAPCatalog(string productId, ProductInfo productInfo, ProductCatalogItem eachCatalogItem)
    {
        // ProductInfo foundProductInfo = FirebaseDataManager.Instance.productCatalog[id];
        eachCatalogItem.defaultDescription.Title = productInfo.title;
        eachCatalogItem.defaultDescription.Description = productInfo.description;
        eachCatalogItem.googlePrice.value = decimal.Parse(productInfo.price);

        if (!_IAPDict.ContainsKey(productId))
        {
            return;
        }
        IAPButton iapBtn = _IAPDict[productId];
        iapBtn.titleText.text = productInfo.title;
        iapBtn.descriptionText.text = productInfo.description;
        iapBtn.priceText.text = $"{productInfo.price}";
    }

    /// <summary>
    /// Set up IAP Button, Outer Button, Outer Price Text dictionaries to access each components fast and easy.
    /// </summary>
    private void SetDict()
    {
        for (int i = 0; i < _IAPButtons.Length; i++)
        {
            // Check IAPButton's Product Id -> Debug.Log(_IAPButtons[i].productId);
            var id = _IAPButtons[i].productId.Split('.')[^1];
            _IAPDict.Add(id, _IAPButtons[i]);
            
            // Outer buttons to see detailed info of products
            Button outerBtn = _outerBtnParent.GetChild(i).GetComponent<Button>();
            _outerBtnDict.Add(id, outerBtn);
            _outerPriceDict.Add(id, outerBtn.transform.GetChild(1).GetChild(0).GetComponent<Text>());
        }
    }

    /// <summary>
    /// Wait until user's wallet info is ready. Check each In-App Purchase items were purchased and block buttons.
    /// </summary>
    /// <returns>FirebaseDataManager.Instance.waitForSearchEnd (wait until user info is found in firebase)</returns>
    private IEnumerator CoWaitWalletReady()
    {
        yield return FirebaseDataManager.Instance.waitForSearchEnd;
        // ####### Wallet Ready! #######
        for (int i = 0; i < _IAPButtons.Length; i++)
        {
            // Check whether each non-consumable product is already puchased
            LoadPurchasedFromWallet(_IAPButtons[i]);
        }
    }
    
    /// <summary>
    /// If IAP Button's id is found in product catalog and the product is non-consumable, prevent double purchase.
    /// Otherwise, show price in the outer button's price text.
    /// </summary>
    /// <param name="button">IAP Button to be investigated</param>
    private void LoadPurchasedFromWallet(IAPButton button)
    {
        // Get Product Id From IAP Button's ProductId
        string[] s = button.productId.Split('.');
        string id = s[^1];
        if (!FirebaseDataManager.Instance.productCatalog.ContainsKey(id))
        {
            return;
        }
        
        // Non consumable which is already purchased
        if (!FirebaseDataManager.Instance.productCatalog[id].isConsumable)
        {
            if (FirebaseDataManager.Instance.CheckProductInWallet(id))
            {
                _outerBtnDict[id].interactable = false;
                _outerPriceDict[id].text = _purchasedText;
                button.enabled = false; // Block IAP Button
                return;
            }
            _outerPriceDict[id].text = button.priceText.text;
        }
    }

    /// <summary>
    /// When IAP Button is pressed and the In-App Purchase was successful, push product data in user's wallet and update the wallet data to firebase.
    /// </summary>
    /// <param name="product"></param>
    public void OnPurchaseCompleted(Product product)
    {
        string id = product.definition.id.Split('.')[^1];
        bool resultFromDatabase = false;
        
        if (id.Equals(_idStarterPack))
        {
            // Starter pack -> give two products
            bool resultFromDatabase1 = FirebaseDataManager.Instance.Purchase(_idSunglasses);
            bool resultFromDatabase2 = FirebaseDataManager.Instance.Purchase(_idPetCat);

            resultFromDatabase = resultFromDatabase1 && resultFromDatabase2;
        }
        else
        {
            resultFromDatabase = FirebaseDataManager.Instance.Purchase(id);
        }

        // When updating wallet data in Firebase Database was successful... 
        if (resultFromDatabase)
        {
            StartCoroutine(CoSendProducts(id));
            FirebaseDataManager.Instance.UpdateInfo();      
            // Notify result
            if (product.definition.type.Equals(ProductType.NonConsumable))
            {
                // Buttons should be disabled after OnCompletePurchase function has ended to prevent InvalidOperationException: Collection was modified; 
                StartCoroutine(CoDisableButtons(id));
            } 
        }

    }

    private IEnumerator CoSendProducts(string id)
    {
        yield return _waitForHalfSec;
        _store.PurchasePaidItem(id);
    }
    private IEnumerator CoDisableButtons(string idKey)
    {
        yield return _waitForHalfSec;
        _IAPDict[idKey].enabled = false;
        _outerBtnDict[idKey].enabled = false;
        _outerPriceDict[idKey].text = _purchasedText;
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        Debug.Log($"Purchase of {product.definition.id} failed because of {failureReason}");   
    }
}
