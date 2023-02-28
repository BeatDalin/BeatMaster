using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
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
    [SerializeField] private IAPButton[] _IAPButtons;
    private Dictionary<string, IAPButton> _IAPDict = new Dictionary<string, IAPButton>();
    private ProductCatalog _catalog;
    [SerializeField] private Store _store;
    // Start is called before the first frame update
    void Start()
    {
        _catalog = ProductCatalog.LoadDefaultCatalog();
        foreach (var productCatalogItem in _catalog.allProducts)
        {
            Debug.Log(productCatalogItem.id);
        }
        Debug.Log("================ Product Catalog From Firebase ===================");
        foreach (KeyValuePair<string, ProductInfo> pair in FirebaseDataManager.Instance.productCatalog)
        {
            Debug.Log($"Key {pair.Key} // Value {pair.Value.id}");
            Debug.Log($"Title {pair.Value.title}");
        }
        
        StartCoroutine(CoWaitWalletReady());
    }

    private IEnumerator CoWaitWalletReady()
    {
        yield return FirebaseDataManager.Instance.waitForSearchEnd;
        Debug.Log("+++++++++ Wallet Ready! ++++++++");
        for (int i = 0; i < _IAPButtons.Length; i++)
        {
            _IAPDict.Add(_IAPButtons[i].productId, _IAPButtons[i]);
            LoadProductInfoFromFirebase(_IAPButtons[i]);
        }
    }
    private void LoadProductInfoFromFirebase(IAPButton button)
    {
        // Get Product Id From IAP Button's ProductId
        string[] s = button.productId.Split('.');
        string id = s[^1];
        if (!FirebaseDataManager.Instance.productCatalog.ContainsKey(id))
        {
            return;
        }
        button.titleText.text = FirebaseDataManager.Instance.productCatalog[id].title;
        button.descriptionText.text = FirebaseDataManager.Instance.productCatalog[id].description;

        if (!FirebaseDataManager.Instance.productCatalog[id].isConsumable 
            && FirebaseDataManager.Instance.CheckProductInWallet(id))
        {
            // Non consumable which is already purchased
            button.priceText.text = "Purchased";
            button.gameObject.GetComponent<Button>().interactable = false; // Block Button
            button.enabled = false; // Block IAP Button
            return;
        }
        button.priceText.text = FirebaseDataManager.Instance.productCatalog[id].price;
    }

    public void OnPurchaseCompleted(Product product)
    {
        string id = product.definition.id.Split('.')[^1];
        bool resultFromDatabase = true;
        // if (product.definition.type == ProductType.NonConsumable)
        // {
        //     // Non consumable product which is already purchased
        //     return;
        // }
        if (id.Equals(_idStarterPack))
        {
            // Starter Pack buy => Give Sunglasses and Pet Cat
            // bool resultFromDatabase1 = FirebaseDataManager.Instance.Purchase(_idSunglasses);
            // bool resultFromDatabase2 = FirebaseDataManager.Instance.Purchase(_idPetCat);

            // resultFromDatabase = resultFromDatabase1 && resultFromDatabase2;
            
        }
        else
        {
            resultFromDatabase = FirebaseDataManager.Instance.Purchase(id);
        }

        if (resultFromDatabase)
        {
            _store.PurchasePaidItem(id);            
        }

        // FirebaseDataManager.Instance.UpdateInfo();
        // Notify result
        Debug.Log($"On Purchase {id} : {resultFromDatabase}");
        if (product.definition.type.Equals(ProductType.NonConsumable))
        {
            _IAPDict[product.definition.id].priceText.text = "Purchased";
            _IAPDict[product.definition.id].gameObject.GetComponent<Button>().interactable = false; // Block Button
            _IAPDict[product.definition.id].enabled = false; // Block IAP Button
        }
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        Debug.Log($"Purchase of {product.definition.id} failed because of {failureReason}");   
    }
}
