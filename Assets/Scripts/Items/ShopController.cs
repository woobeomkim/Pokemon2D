using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ShopState {Menu,Buying,Selling,Busy }

public class ShopController : MonoBehaviour
{
    [SerializeField] InventoryUI inventoryUI;
    [SerializeField] WalletUI walletUI;

    public event Action OnStart;
    public event Action OnFinish;

    ShopState state;
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
        int selectedChoice = 0;
        yield return DialogManager.Instance.ShowDialogText($"������ {sellingPrice}�Դϴ�! �Ľðڽ��ϱ�?",
            waitForInput: false,
            choices: new List<string>() { "�Ǵ�", "���Ǵ�"},
            onChoiceSelected: choiceIndex => selectedChoice = choiceIndex);
    
        if(selectedChoice ==0)
        {
            inventory.RemoveItem(item);
            Wallet.i.AddMoney(sellingPrice);
            yield return DialogManager.Instance.ShowDialogText($"{item.Name}�� �ѱ�� {sellingPrice}�� �޾Ҵ�");
        }
        else
        {

        }

        walletUI.Close();

        state = ShopState.Selling;
    }
}
