using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;

namespace Samples.Purchasing.Core.BuyingSubscription
{
    // NOTE : most of this code (especially purchasing process) written by reference
    // "Buying Subscription" script -> samples -> in app purchase -> Package Manager

    public class VIPSubscriptions : MonoBehaviour, IStoreListener
    {
        public static VIPSubscriptions instance;
        public GameObject SubscriptionPanel;

        IStoreController m_StoreController;

        // Your subscription ID. It should match the id of your subscription in your store.
        public string subscriptionProductId1m = "";
        public string subscriptionProductId3m = "";
        public string subscriptionProductId6m = "";

        public Text Title;
        public Text Debit;
        public Text Beginning;
        public Text Expiration;
        public Text NextRenewal;

        private static bool IsVIP;

        void Start()
        {
            instance = this;
            InitializePurchasing();
        }

        void InitializePurchasing()
        {
            var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

            // Add our purchasable product and indicate its type.
            builder.AddProduct(subscriptionProductId1m, ProductType.Subscription);
            builder.AddProduct(subscriptionProductId3m, ProductType.Subscription);
            builder.AddProduct(subscriptionProductId6m, ProductType.Subscription);

            UnityPurchasing.Initialize(this, builder);
        }

        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            Debug.Log("In-App Purchasing successfully initialized");
            m_StoreController = controller;

            UpdateVIPSubscriptionsInfo();
        }

        public void OnInitializeFailed(InitializationFailureReason error)
        {
            OnInitializeFailed(error, null);
        }

        public void OnInitializeFailed(InitializationFailureReason error, string message)
        {
            var errorMessage = $"Purchasing failed to initialize. Reason: {error}.";

            if (message != null)
            {
                errorMessage += $" More details: {message}";
            }

            Debug.Log(errorMessage);
        }


        public void BuySubscription1m()
        {
            m_StoreController.InitiatePurchase(subscriptionProductId1m);
        }
        public void BuySubscription3m()
        {
            m_StoreController.InitiatePurchase(subscriptionProductId3m);
        }
        public void BuySubscription6m()
        {
            m_StoreController.InitiatePurchase(subscriptionProductId6m);
        }

        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
        {
            // Retrieve the purchased product
            var product = args.purchasedProduct;

            Debug.Log($"Purchase Complete - Product: {product.definition.id}");

            UpdateVIPSubscriptionsInfo();

            // We return Complete, informing IAP that the processing on our side is done and the transaction can be closed.
            return PurchaseProcessingResult.Complete;
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
        {
            Debug.Log($"Purchase failed - Product: '{product.definition.id}', PurchaseFailureReason: {failureReason}");
        }


        public void UpdateVIPSubscriptionsInfo()
        {
            var subscriptionProduct1 = m_StoreController.products.WithID(subscriptionProductId1m);
            var subscriptionProduct2 = m_StoreController.products.WithID(subscriptionProductId3m);
            var subscriptionProduct3 = m_StoreController.products.WithID(subscriptionProductId6m);

            try
            {
                //var isSubscribed1 = IsSubscribedTo(subscriptionProduct1);
                //var isSubscribed2 = IsSubscribedTo(subscriptionProduct2);
                //var isSubscribed3 = IsSubscribedTo(subscriptionProduct3);

                bool isSubscribed1 = IsSubscribedTo(subscriptionProduct1);
                bool isSubscribed2 = IsSubscribedTo(subscriptionProduct2);
                bool isSubscribed3 = IsSubscribedTo(subscriptionProduct3);

                if (isSubscribed1 || isSubscribed2 || isSubscribed3)
                {
                    IsVIP = true;
                    SubscriptionPanel.SetActive(true);
                }
                else
                {
                    IsVIP = false;
                    SubscriptionPanel.SetActive(false);
                }
                UpdateUI(isSubscribed1, isSubscribed2, isSubscribed3);
            }
            catch (StoreSubscriptionInfoNotSupportedException)
            {
                //var receipt = (Dictionary<string, object>)MiniJson.JsonDecode(subscriptionProduct.receipt);
                //var store = receipt["Store"];
                //isSubscribedText.text =
                //    "Couldn't retrieve subscription information because your current store is not supported.\n" +
                //    $"Your store: \"{store}\"\n\n" +
                //    "You must use the App Store, Google Play Store or Amazon Store to be able to retrieve subscription information.\n\n" +
                //    "For more information, see README.md";
            }
        }

        public static bool GetVIPState()
        {
            return IsVIP;
        }

        bool IsSubscribedTo(Product subscription)
        {
            // If the product doesn't have a receipt, then it wasn't purchased and the user is therefore not subscribed.
            if (subscription.receipt == null)
            {
                return false;
            }

            //The intro_json parameter is optional and is only used for the App Store to get introductory information.
            var subscriptionManager = new SubscriptionManager(subscription, null);

            // The SubscriptionInfo contains all of the information about the subscription.
            // Find out more: https://docs.unity3d.com/Packages/com.unity.purchasing@3.1/manual/UnityIAPSubscriptionProducts.html
            var info = subscriptionManager.getSubscriptionInfo();

            PlayerPrefs.SetString("PurchaseDateKey", info.getPurchaseDate().ToString());
            return info.isSubscribed() == Result.True;
        }

        void UpdateUI(bool _isSubscribed1, bool _isSubscribed2, bool _isSubscribed3)
        {
            int SubscriptionDuration = 0;
            float SubscriptionPrice = 0f;
            DateTime Date = new DateTime();

            if (_isSubscribed1)
            {
                SubscriptionDuration = 1;
                SubscriptionPrice = 12.99f;
            }
            else if (_isSubscribed2)
            {
                SubscriptionDuration = 3;
                SubscriptionPrice = 11.99f;
            }
            else if (_isSubscribed3)
            {
                SubscriptionDuration = 6;
                SubscriptionPrice = 10.99f;
            }

            if (PlayerPrefs.HasKey("PurchaseDateKey"))
            {
                Date = DateTime.Parse(PlayerPrefs.GetString("PurchaseDateKey"));

                // from unity docs :
                // info.getPurchaseDate() Returns the Product’s purchase date.
                // For Apple, the purchase date is the date when the subscription was either purchased or renewed. 
                // For Google, the purchase date is the date when the subscription was originally purchased.

                // so for Google : calculate the real Beginning date
                DateTime Now = DateTime.Now;
                int duration = (Now.Month - Date.Month) + 12 * (Now.Year - Date.Year);
                while (duration > SubscriptionDuration)
                {
                    duration -= SubscriptionDuration;
                    Date.AddMonths(-SubscriptionDuration);
                }
            }

            Title.text = "Pack Legendaire - " + SubscriptionDuration.ToString() + " mois";
            Debit.text = SubscriptionPrice.ToString() + " €";
            Beginning.text = Date.ToString("yyyy/MM/dd");
            Date.AddMonths(SubscriptionDuration);
            Expiration.text = Date.ToString("yyyy/MM/dd");
            NextRenewal.text = Date.ToString("yyyy/MM/dd");
        }
    }
}