using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

[System.Serializable]
public class ProductInfo
{
    public string id;
    public string title;
    public string price;
    public bool isConsumable;
    public int increaseAmount;

    public ProductInfo(string id, string title, string price, bool isConsumable, int increaseAmount)
    {
        this.id = id;
        this.title = title;
        this.price = price;
        this.isConsumable = isConsumable;
        this.increaseAmount = increaseAmount; // Set amount and never change
    }
}

[System.Serializable]
public class UserInfo
{
    // Set Key to recycle
    private const string _emailKey = "email";
    private const string _nameKey = "name";
    private const string _walletKey = "wallet";
    private const string _walletStringKey = "walletString";
    
    
    // Actual Data
    public string userId;
    public string email;
    public string name;
    public Dictionary<string, int> wallet;  // Key: Product id, Value: the amount of Product
    public string walletString;

    // To create UserInfo
    public UserInfo CreateUserInfo(string userId, string email, string name)
    {
        this.userId = userId;   
        this.email = email;
        this.name = name;
        this.wallet = new Dictionary<string, int>();
        this.walletString = ConvertDictionaryToString(wallet);
        return this;
    }
    
    // objects that can be put in
    // https://firebase.google.com/docs/reference/unity/class/firebase/database/mutable-data#class_firebase_1_1_database_1_1_mutable_data_1a4833f23246b3079078332d57c5649254
    public Dictionary<string, object> ToDictionary()
    {
        Dictionary<string, object> dict = new Dictionary<string, object>();
        dict[_emailKey] = this.email;
        dict[_nameKey] = this.name;
        dict[_walletKey] = this.wallet;
        dict[_walletStringKey] = this.walletString;
        return dict;
    }

    public void ConvertWalletFromString()
    {
        wallet = ConvertStringToDictionary(walletString);
    }

    public bool BuyProduct(string productId, Dictionary<string, ProductInfo> itemCatalogue)
    {
        if (!itemCatalogue.TryGetValue(productId, out ProductInfo productToBuy))
        {
            return false; // The product item does not exist in catalogue
        }
        
        // The product item exists in catalogue
        // Check whether the non-consumable product is already bought.
        bool hasInWallet = wallet.TryGetValue(productId, out int count);

        // Debug.Log($"Product {productId} is inside Dictionary? : {hasInWallet}");
        if (hasInWallet)
        {
            if (!productToBuy.isConsumable)
            {
                return false; // product item is non consumable but already have -> cannot buy again
            }
            // Consumable item -> can buy
            wallet[productId] += productToBuy.increaseAmount;
            walletString = ConvertDictionaryToString(wallet);
            // Debug.Log($"Wallet String After Buy {walletString}");
            return true;
        }
        
        // Buy a new one
        wallet.Add(productId, productToBuy.increaseAmount);
        walletString = ConvertDictionaryToString(wallet);
        // Debug.Log($"Wallet String After Buy New One {walletString}");

        return true;
    }

    public bool ConsumeProduct(string productId, int consumeAmount, Dictionary<string, ProductInfo> itemCatalogue)
    {
        if (!wallet.TryGetValue(productId, out int amountInWallet) || !itemCatalogue.ContainsKey(productId) || !itemCatalogue[productId].isConsumable)
        {
            // User does not have input product / Wrong product id / Input product is non consumable product
            return false;
        }

        // Consume only for consumable products
        if (amountInWallet - consumeAmount < 0)
        {
            // Input product is consumable, but the amount of product in wallet is less than the amount to consume.
            // Reject consumption.
            // Debug.Log($"{productId} || Your wallet({amountInWallet} has less amount than the amount to consume {consumeAmount}.");
            return false;
        }
        
        // Consumption is available
        wallet[productId] -= consumeAmount;
        walletString = ConvertDictionaryToString(wallet);
        // Debug.Log($"Item consumed. {productId} {wallet[productId]}");

        return true;
    }
    
    
    public static string ConvertDictionaryToString<DKey, DValue> (Dictionary<DKey, DValue> dict)
    {
        if (dict == null)
        {
            return string.Empty;
        }
        if (dict.Count == 0) // Nothing in wallet
        {
            return string.Empty; 
        }
        
        string format = "{0}='{1}',";
        StringBuilder itemString = new StringBuilder();
        foreach(KeyValuePair<DKey, DValue> kv in dict)
        {
            // Debug.Log($"{kv.Key} {kv.Value}");
            itemString.AppendFormat(format, kv.Key, kv.Value);
        }
        // Debug.Log("Dictionary String: " + itemString);
        itemString.Remove(itemString.Length - 1, 1); // Remove , at last

        return itemString.ToString();
    }
    
    public static Dictionary<string, int> ConvertStringToDictionary (string dictString)
    {
        // string format must be like
        // "key='value',key='value',key='value', ..."

        if (dictString.Length == 0)
        {
            return new Dictionary<string, int>(); // Nothing in wallet
        }

        return dictString.Split (',')
            .Select(pp => pp.Trim().Split('='))
            .ToDictionary (pp => pp[0], pp => int.Parse(pp[1].Trim('\'')));
    }
}
