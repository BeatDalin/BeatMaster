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
    // Start is called before the first frame update
    void Start()
    {
        _catalog = ProductCatalog.LoadDefaultCatalog();
        foreach (var productCatalogItem in _catalog.allProducts)
        {
            Debug.Log(productCatalogItem.id);
            var id = productCatalogItem.id.Split('.')[^1];
            productCatalogItem.defaultDescription.Title = FirebaseDataManager.Instance.productCatalog[id].title;
            productCatalogItem.defaultDescription.Description = FirebaseDataManager.Instance.productCatalog[id].description;
            productCatalogItem.googlePrice.value = decimal.Parse(FirebaseDataManager.Instance.productCatalog[id].price);
            

        }
        Debug.Log("================ Product Catalog From Firebase ===================");
        foreach (KeyValuePair<string, ProductInfo> pair in FirebaseDataManager.Instance.productCatalog)
        {
            Debug.Log($"Title {pair.Value.title} // Key {pair.Key} // Value {pair.Value.id}");
        }
#if UNITY_EDITOR
        for (int i = 0; i < _IAPButtons.Length; i++)
        {
            Debug.Log(_IAPButtons[i].productId);
            var id = _IAPButtons[i].productId.Split('.')[^1];
            _IAPDict.Add(id, _IAPButtons[i]);
            Button outerBtn = _outerBtnParent.GetChild(i).GetComponent<Button>();
            _outerBtnDict.Add(id, outerBtn);
            _outerPriceDict.Add(id, outerBtn.transform.GetChild(1).GetChild(0).GetComponent<Text>());
            
        
            _IAPButtons[i].titleText.text = FirebaseDataManager.Instance.productCatalog[id].title;
            _IAPButtons[i].descriptionText.text = FirebaseDataManager.Instance.productCatalog[id].description;
            _IAPButtons[i].priceText.text = FirebaseDataManager.Instance.productCatalog[id].price;
            _outerPriceDict[id].text = FirebaseDataManager.Instance.productCatalog[id].price;
            
            
            Debug.Log("================ Dictionary Check ===================");
            Debug.Log(id);
            Debug.Log("Outer Button "+_outerBtnDict[id].name);
            Debug.Log("Outer PriceText "+ _outerPriceDict[id].text);
            Debug.Log($"Title {_IAPButtons[i].titleText.text}");
            
            Debug.Log("================ IAP Button Check ==================");
            Debug.Log($"Title {_IAPButtons[i].titleText.text}");
            Debug.Log($"Description {_IAPButtons[i].descriptionText.text}");
            Debug.Log($"Price {_IAPButtons[i].priceText.text}");
            
        }
        
#endif
        StartCoroutine(CoWaitWalletReady());
        
    }

    private IEnumerator CoWaitWalletReady()
    {
        yield return FirebaseDataManager.Instance.waitForSearchEnd;
        Debug.Log("+++++++++ Wallet Ready! ++++++++");
        Debug.Log(_IAPButtons[0].productId);
        Debug.Log(_IAPButtons[1].productId);
        for (int i = 0; i < _IAPButtons.Length; i++)
        {
            int index = i;
            var id = _IAPButtons[index].productId.Split('.')[^1];
            _IAPDict.Add(id, _IAPButtons[i]);
            LoadProductInfoFromFirebase(_IAPButtons[i]);
            
            // Outer buttons to see detailed info of products
            Button outerBtn = _outerBtnParent.GetChild(i).GetComponent<Button>();
            _outerBtnDict.Add(id, outerBtn);
            _outerPriceDict.Add(id, outerBtn.transform.GetChild(1).GetChild(0).GetComponent<Text>());
            
            Debug.Log("================ Dictionary Check ===================");
            Debug.Log(id);
            Debug.Log("Outer Button "+_outerBtnDict[id].name);
            Debug.Log("Outer PriceText "+ _outerPriceDict[id].name);
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
        button.priceText.text = FirebaseDataManager.Instance.productCatalog[id].price;
        _outerPriceDict[id].text = FirebaseDataManager.Instance.productCatalog[id].price;
        if (!FirebaseDataManager.Instance.productCatalog[id].isConsumable 
            && FirebaseDataManager.Instance.CheckProductInWallet(id))
        {
            // Non consumable which is already purchased
            // button.priceText.text = _purchasedText;
            // button.gameObject.GetComponent<Button>().interactable = false; // Block Button
            _outerBtnDict[id].interactable = false;
            _outerPriceDict[id].text = _purchasedText;
            button.enabled = false; // Block IAP Button
            return;
        }
        
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
            bool resultFromDatabase1 = FirebaseDataManager.Instance.Purchase(_idSunglasses);
            bool resultFromDatabase2 = FirebaseDataManager.Instance.Purchase(_idPetCat);

            resultFromDatabase = resultFromDatabase1 && resultFromDatabase2;
            
        }
        else
        {
            resultFromDatabase = FirebaseDataManager.Instance.Purchase(id);
        }

        if (resultFromDatabase)
        {
            _store.PurchasePaidItem(id);      
            FirebaseDataManager.Instance.UpdateInfo();      
        }

        // Notify result
        Debug.Log($"On Purchase {id} : {resultFromDatabase}");
        if (product.definition.type.Equals(ProductType.NonConsumable))
        {
            _IAPDict[product.definition.id].priceText.text = _purchasedText;
            // _IAPDict[product.definition.id].gameObject.GetComponent<Button>().interactable = false; // Block Button
            _IAPDict[product.definition.id].enabled = false; // Block IAP Button

            _outerBtnDict[product.definition.id].interactable = false;
            _outerPriceDict[product.definition.id].text = _purchasedText;
        }
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        Debug.Log($"Purchase of {product.definition.id} failed because of {failureReason}");   
    }
}
