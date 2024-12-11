using UnityEngine;
using UnityEngine.Purchasing;
using TMPro;
using Unity.Services.Core;

public class IAPManager : MonoBehaviour, IStoreListener
{
    public static IStoreController storeController;
    private static IExtensionProvider storeExtensionProvider;

    // Identifiants des produits (� configurer dans Unity IAP)
    public static string PRODUCT_RESOURCE_SMALL = "resource_small";
    public static string PRODUCT_RESOURCE_MEDIUM = "resource_medium";
    public static string PRODUCT_RESOURCE_LARGE = "resource_large";
    public static string PRODUCT_REMOVE_ADS = "remove_ads";

    [Header("Gold Rewards")]
    [SerializeField] private int smallResourceGold = 100;
    [SerializeField] private int mediumResourceGold = 300;
    [SerializeField] private int largeResourceGold = 1000;


    private void Start()
    {
        if (storeController == null)
        {
            InitializePurchasing();
        }
    }

    public async void InitializePurchasing()
    {
        if (IsInitialized())
            return;

        await UnityServices.InitializeAsync();

        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

        // Ajouter les produits
        builder.AddProduct(PRODUCT_RESOURCE_SMALL, ProductType.Consumable);
        builder.AddProduct(PRODUCT_RESOURCE_MEDIUM, ProductType.Consumable);
        builder.AddProduct(PRODUCT_RESOURCE_LARGE, ProductType.Consumable);
        builder.AddProduct(PRODUCT_REMOVE_ADS, ProductType.NonConsumable);

        UnityPurchasing.Initialize(this, builder);
    }

    private bool IsInitialized()
    {
        return storeController != null && storeExtensionProvider != null;
    }

    // M�thodes pour les achats de ressources
    public void BuySmallResource()
    {
        BuyProductID(PRODUCT_RESOURCE_SMALL);
    }

    public void BuyMediumResource()
    {
        BuyProductID(PRODUCT_RESOURCE_MEDIUM);
    }

    public void BuyLargeResource()
    {
        BuyProductID(PRODUCT_RESOURCE_LARGE);
    }

    // M�thode pour supprimer les publicit�s
    public void BuyRemoveAds()
    {
        BuyProductID(PRODUCT_REMOVE_ADS);
    }

    private void BuyProductID(string productId)
    {
        if (IsInitialized())
        {
            Product product = storeController.products.WithID(productId);

            if (product == null)
            {
                Debug.LogError($"Product not found: {productId}");
                return;
            }

            if (product.availableToPurchase)
            {
                Debug.Log($"Attempting to purchase: {product.definition.id}");
                storeController.InitiatePurchase(product);
            }
            else
            {
                Debug.LogWarning($"Product not available for purchase: {productId}");
            }
        }
        else
        {
            Debug.LogError("IAP system not initialized. Cannot make a purchase.");
        }
    }

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        Debug.Log("IAP initialized.");
        storeController = controller;
        storeExtensionProvider = extensions;
    }

    public void OnInitializeFailed(InitializationFailureReason error)
    {
        Debug.Log($"IAP initialization failed: {error}");
    }

    public void OnInitializeFailed(InitializationFailureReason error, string message)
    {
        Debug.Log($"IAP initialization failed: {error}, Message: {message}");
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        Debug.Log($"Purchase failed: {product.definition.id}, Reason: {failureReason}");
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
        if (args.purchasedProduct.definition.id == PRODUCT_RESOURCE_SMALL)
        {
            Debug.Log("Small resource pack purchased!");
            SettingSystem.instance.AddGold(smallResourceGold); // R�compense pour le petit pack de ressources
        }
        else if (args.purchasedProduct.definition.id == PRODUCT_RESOURCE_MEDIUM)
        {
            Debug.Log("Medium resource pack purchased!");
            SettingSystem.instance.AddGold(mediumResourceGold); // R�compense pour le pack moyen de ressources
        }
        else if (args.purchasedProduct.definition.id == PRODUCT_RESOURCE_LARGE)
        {
            Debug.Log("Large resource pack purchased!");
            SettingSystem.instance.AddGold(largeResourceGold); // R�compense pour le grand pack de ressources
        }
        else if (args.purchasedProduct.definition.id == PRODUCT_REMOVE_ADS)
        {
            Debug.Log("Ads removed!");
            PlayerPrefs.SetInt("AdsRemoved", 1); // Enregistre la suppression des publicit�s
        }
        else
        {
            Debug.Log($"Purchase not recognized: {args.purchasedProduct.definition.id}");
        }

        return PurchaseProcessingResult.Complete;
    }
}
