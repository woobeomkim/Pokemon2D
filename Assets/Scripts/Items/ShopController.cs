using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ShopState {Menu,Buying,Selling,Busy }

public class ShopController : MonoBehaviour
{
    [SerializeField] Vector3 shopCameraOffset;
    [SerializeField] InventoryUI inventoryUI;
    [SerializeField] ShopUI shopUI;
    [SerializeField] WalletUI walletUI;
    [SerializeField] CountSelectorUI countSelectorUI;

    public event Action OnStart;
    public event Action OnFinish;

    ShopState state;

    Merchant merchant;
    public static ShopController i { get; private set; }
    private void Awake()
    {
        i = this;
    }

    Inventory inventory;
    private void Start()
    {
        inventory = Inventory.GetInventory();
    }

    public IEnumerator StartTrading(Merchant merchant)
    {
        this.merchant = merchant;
        OnStart?.Invoke();
        yield return StartMenuState();      
    }

    IEnumerator StartMenuState()
    {
        state = ShopState.Menu;

        int selectedChoice = 0;
        yield return DialogManager.Instance.ShowDialogText("어떻게 도와드릴까요",
            waitForInput: false,
            choices: new List<string>() { "산다", "판다", "나간다" },
            onChoiceSelected: choiceIndex => selectedChoice = choiceIndex);

        if (selectedChoice == 0)
        {
            //BUY
            state = ShopState.Buying;
            yield return GameController.Instance.MoveCamera(shopCameraOffset);
            walletUI.Show();
            shopUI.Show(merchant.AvailableItems, (item) => StartCoroutine(BuyItem(item)),
                () => StartCoroutine(OnBackFromBuying()));
        }
        else if (selectedChoice == 1)
        {
            //SELL
            state = ShopState.Selling;

            inventoryUI.gameObject.SetActive(true);
        }
        else if (selectedChoice == 2)
        {
            //EXIT
            OnFinish?.Invoke();
            yield break;
        }
    }

    public void HandleUpdate()
    {
        if(state == ShopState.Selling)
        {
            inventoryUI.HandleUpdate(OnBackFromSelling,(selectedItem) => { StartCoroutine(SellItem(selectedItem)); });
        }
        else if (state == ShopState.Buying)
        {
            shopUI.HandleUpdate();
        }
    }

    void OnBackFromSelling()
    {
        inventoryUI.gameObject.SetActive(false);
        StartCoroutine(StartMenuState());
    }

    IEnumerator SellItem(ItemBase item)
    {
        state = ShopState.Busy;

        if(!item.IsSellable)
        {
            yield return DialogManager.Instance.ShowDialogText("이 아이템은 팔 수 없습니다.");
            state = ShopState.Selling;
            yield break;
        }

        walletUI.Show();

        float sellingPrice = Mathf.Round(item.Price / 2);
        int countToSell = 1;

        int itemCount = inventory.GetItemCount(item);
        if(itemCount >1)
        {
            yield return DialogManager.Instance.ShowDialogText($"몇개를 파시겠습니까?",
                waitForInput: false, autoClose: false);

            yield return countSelectorUI.ShowSelector(itemCount, sellingPrice,
                (selectedCount) => { countToSell = selectedCount; });

            DialogManager.Instance.CloseDialog();
        }

        sellingPrice = sellingPrice * countToSell;

        int selectedChoice = 0;
        yield return DialogManager.Instance.ShowDialogText($"가격은 {sellingPrice}입니다! 파시겠습니까?",
            waitForInput: false,
            choices: new List<string>() { "판다", "안판다"},
            onChoiceSelected: choiceIndex => selectedChoice = choiceIndex);
    
        if(selectedChoice ==0)
        {
            inventory.RemoveItem(item, countToSell);
            Wallet.i.AddMoney(sellingPrice);
            yield return DialogManager.Instance.ShowDialogText($"{item.Name}을 넘기고 {sellingPrice}를 받았다");
        }
        else
        {

        }

        walletUI.Close();

        state = ShopState.Selling;
    }

    IEnumerator BuyItem(ItemBase item)
    {
        state = ShopState.Busy;
        yield return DialogManager.Instance.ShowDialogText($"몇개를 사시겠습니까?",
                waitForInput: false, autoClose: false);
    
        int countToBuy = 1;
        yield return countSelectorUI.ShowSelector(100, item.Price,
            (selectedCount) => { countToBuy = selectedCount; });
    
        DialogManager.Instance.CloseDialog();

        float totalPrice = item.Price* countToBuy;
    
        if(Wallet.i.HasMoney(totalPrice))
        {
            int selectedChoice = 0;
            yield return DialogManager.Instance.ShowDialogText($"가격은 {totalPrice}입니다! 사시겠습니까?",
                waitForInput: false,
                choices: new List<string>() { "산다", "안산다" },
                onChoiceSelected: choiceIndex => selectedChoice = choiceIndex);
     
            if(selectedChoice == 0)
            {
                inventory.AddItem(item, countToBuy);
                Wallet.i.TakeMoney(totalPrice);
                yield return DialogManager.Instance.ShowDialogText($"{item.Name}을 사고 {totalPrice}를 지불했다");
            }
            else
            {

            }
            state = ShopState.Buying;
        }
        else
        {
            yield return DialogManager.Instance.ShowDialogText("돈이 부족합니다.");  
        }
    }

    IEnumerator OnBackFromBuying()
    {
        yield return GameController.Instance.MoveCamera(-shopCameraOffset);
        shopUI.Close();
        walletUI.Close();
        StartCoroutine(StartMenuState());
    }
}
