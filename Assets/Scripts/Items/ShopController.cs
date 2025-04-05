using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ShopState {Menu,Buying,Selling,Busy }

public class ShopController : MonoBehaviour
{
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
        yield return DialogManager.Instance.ShowDialogText("��� ���͵帱���",
            waitForInput: false,
            choices: new List<string>() { "���", "�Ǵ�", "������" },
            onChoiceSelected: choiceIndex => selectedChoice = choiceIndex);

        if (selectedChoice == 0)
        {
            //BUY
            state = ShopState.Buying;
            walletUI.Show();
            shopUI.Show(merchant.AvailableItems, (item) => StartCoroutine(BuyItem(item)) , OnBackFromBuying);
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
            yield return DialogManager.Instance.ShowDialogText("�� �������� �� �� �����ϴ�.");
            state = ShopState.Selling;
            yield break;
        }

        walletUI.Show();

        float sellingPrice = Mathf.Round(item.Price / 2);
        int countToSell = 1;

        int itemCount = inventory.GetItemCount(item);
        if(itemCount >1)
        {
            yield return DialogManager.Instance.ShowDialogText($"��� �Ľðڽ��ϱ�?",
                waitForInput: false, autoClose: false);

            yield return countSelectorUI.ShowSelector(itemCount, sellingPrice,
                (selectedCount) => { countToSell = selectedCount; });

            DialogManager.Instance.CloseDialog();
        }

        sellingPrice = sellingPrice * countToSell;

        int selectedChoice = 0;
        yield return DialogManager.Instance.ShowDialogText($"������ {sellingPrice}�Դϴ�! �Ľðڽ��ϱ�?",
            waitForInput: false,
            choices: new List<string>() { "�Ǵ�", "���Ǵ�"},
            onChoiceSelected: choiceIndex => selectedChoice = choiceIndex);
    
        if(selectedChoice ==0)
        {
            inventory.RemoveItem(item, countToSell);
            Wallet.i.AddMoney(sellingPrice);
            yield return DialogManager.Instance.ShowDialogText($"{item.Name}�� �ѱ�� {sellingPrice}�� �޾Ҵ�");
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
        yield return DialogManager.Instance.ShowDialogText($"��� ��ðڽ��ϱ�?",
                waitForInput: false, autoClose: false);
    
        int countToBuy = 1;
        yield return countSelectorUI.ShowSelector(100, item.Price,
            (selectedCount) => { countToBuy = selectedCount; });
    
        DialogManager.Instance.CloseDialog();

        float totalPrice = item.Price* countToBuy;
    
        if(Wallet.i.HasMoney(totalPrice))
        {
            int selectedChoice = 0;
            yield return DialogManager.Instance.ShowDialogText($"������ {totalPrice}�Դϴ�! ��ðڽ��ϱ�?",
                waitForInput: false,
                choices: new List<string>() { "���", "�Ȼ��" },
                onChoiceSelected: choiceIndex => selectedChoice = choiceIndex);
     
            if(selectedChoice == 0)
            {
                inventory.AddItem(item, countToBuy);
                Wallet.i.TakeMoney(totalPrice);
                yield return DialogManager.Instance.ShowDialogText($"{item.Name}�� ��� {totalPrice}�� �����ߴ�");
            }
            else
            {

            }
            state = ShopState.Buying;
        }
        else
        {
            yield return DialogManager.Instance.ShowDialogText("���� �����մϴ�.");  
        }
    }

    void OnBackFromBuying()
    {
        shopUI.Close();
        walletUI.Close();
        StartCoroutine(StartMenuState());
    }
}
