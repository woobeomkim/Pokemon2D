using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UI;

public enum InventoryUIState { ItemSelection, PartySelection, MoveToForget ,Busy }

public class InventoryUI : MonoBehaviour
{
    [SerializeField] GameObject itemList;
    [SerializeField] ItemSlotUI itemSlotUI;

    [SerializeField] Text categoryText;
    [SerializeField] Image itemIcon;
    [SerializeField] Text itemDescription;

    [SerializeField] GameObject upArrow;
    [SerializeField] GameObject downArrow;

    [SerializeField] PartyScreen partyScreen;
    [SerializeField] MoveSelectionUI moveSelectionUI;

    Action<ItemBase> onItemUsed;

    int selectedItem = 0;
    int selectedCategory = 0;

    MoveBase moveToLearn;

    const int itemsInViewport = 8;

    List<ItemSlotUI> slotUIList;

    Inventory inventory;
    InventoryUIState state;

    RectTransform itemListRect;
    private void Awake()
    {
        inventory = Inventory.GetInventory();
        itemListRect = itemList.GetComponent<RectTransform>();
    }

    private void Start()
    {
        UpdateItemList();

        inventory.OnUpdated += UpdateItemList;
    }

    void UpdateItemList()
    {
        // Clear all the existing item slots
        foreach (Transform child in itemList.transform)
        {
            Destroy(child.gameObject);
        }

        slotUIList = new List<ItemSlotUI>();
        foreach (var itemSlot in inventory.GetSlotsByCategory(selectedCategory))
        {
            var slotUIObj = Instantiate(itemSlotUI, itemList.transform);
            slotUIObj.SetData(itemSlot);

            slotUIList.Add(slotUIObj);
        }


        UpdateItemSelection();

    }

    public void HandleUpdate(Action onBack, Action<ItemBase> onItemUsed = null)
    {
        this.onItemUsed = onItemUsed;

        if (state == InventoryUIState.ItemSelection)
        {

            int prevSelected = selectedItem;
            int prevCategory = selectedCategory;

            if (Input.GetKeyDown(KeyCode.DownArrow))
                selectedItem++;
            else if (Input.GetKeyDown(KeyCode.UpArrow))
                selectedItem--;
            else if (Input.GetKeyDown(KeyCode.RightArrow))
                selectedCategory++;
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
                selectedCategory--;


            if(selectedCategory > Inventory.ItemCategorys.Count - 1)
            {
                selectedCategory = 0;
            }
            else if (selectedCategory < 0)
            {
                selectedCategory = Inventory.ItemCategorys.Count - 1;
            }
            selectedItem = Mathf.Clamp(selectedItem, 0, inventory.GetSlotsByCategory(selectedCategory).Count - 1);

            if(prevCategory != selectedCategory)
            {
                ResetSelection();
                categoryText.text = Inventory.ItemCategorys[selectedCategory];
                UpdateItemList();
            }
            else if (prevSelected != selectedItem)
                UpdateItemSelection();

            if (Input.GetKeyDown(KeyCode.Z))
                StartCoroutine(ItemSelected());
            else if (Input.GetKeyDown(KeyCode.X))
                onBack?.Invoke();
        }
        else if(state == InventoryUIState.PartySelection)
        {
            Action onSelected = () =>
            {
                StartCoroutine(UseItem()); 
            };

            Action onBackPartyScreen = () =>
            {
                ClosePartyScreen();
            };

            partyScreen.HandleUpdate(onSelected, onBackPartyScreen);
        }
        else if(state == InventoryUIState.MoveToForget)
        {
            Action<int> onMoveSelected = (int moveIndex) =>
            {
                StartCoroutine(OnMoveToForgetSelected(moveIndex));
            };

            moveSelectionUI.HandleMoveSelection(onMoveSelected);
        }

    }

    IEnumerator ItemSelected()
    {
        state = InventoryUIState.Busy;

        var item = inventory.GetItem(selectedItem, selectedCategory);

        if(GameController.Instance.State == GameState.Battle)
        {
            // In Battle
            if(!item.CanBeUsedInBattle)
            {
                yield return DialogManager.Instance.ShowDialogText($"이 아이템은 배틀에서 사용할수 없습니다!");
                state = InventoryUIState.ItemSelection;
                yield break;
            }
        }
        else
        {
            // Outsize Battle
            if (!item.CanBeUsedOutsideBattle)
            {
                yield return DialogManager.Instance.ShowDialogText($"이 아이템은 배틀밖에서 사용할수 없습니다!");
                state = InventoryUIState.ItemSelection;
                yield break;
            }
        }

        if(selectedCategory == (int)ItemCategory.Pokeballs)
        {
            StartCoroutine(UseItem());
        }
        else
        {
            OpenPartyScreen();

            if (item is TmItem)
                partyScreen.ShowIfTmIsUsable(item as TmItem);
        }
    }

    IEnumerator UseItem()
    {
        state = InventoryUIState.Busy;

        yield return HandleTmItems();

        var usedItem = inventory.UseItem(selectedItem, partyScreen.SelectedMember, selectedCategory);
        if(usedItem != null)
        {
            if(usedItem is RecoveryItem)
                yield return DialogManager.Instance.ShowDialogText($"{usedItem.Name} 을 사용했다.");
            onItemUsed?.Invoke(usedItem);
        }
        else
        {
            if (selectedCategory == (int)ItemCategory.Items)
                yield return DialogManager.Instance.ShowDialogText("효과가 없는 아이템 입니다.");
        }

        ClosePartyScreen();
    }

    IEnumerator HandleTmItems()
    {
        var tmItem = inventory.GetItem(selectedItem, selectedCategory) as TmItem;
        if (tmItem == null)
            yield break;

        var pokemon = partyScreen.SelectedMember;

        if(pokemon.HasMove(tmItem.Move))
        {
            yield return DialogManager.Instance.ShowDialogText($"{pokemon.Base.Name}(이)가 이미 {tmItem.Move.Name}을 배웠다!");
            yield break;
        }

        if(!tmItem.CanBeTaught(pokemon))
        {
            yield return DialogManager.Instance.ShowDialogText($"{pokemon.Base.Name}(이)가 {tmItem.Move.Name}을 배울수없다!");
            yield break;
        }

        if(pokemon.Moves.Count < PokemonBase.MaxNumOfMoves)
        {
            pokemon.LearnMove(tmItem.Move);
            yield return DialogManager.Instance.ShowDialogText($"{pokemon.Base.Name}(이)가 {tmItem.Move.Name}을 배웠다!");
        }
        else
        {
            yield return DialogManager.Instance.ShowDialogText($"{pokemon.Base.Name} (이)가 {tmItem.Move.Name}을 배우려고한다!");
            yield return DialogManager.Instance.ShowDialogText($"그러나 4개이상의 기술을 배울수가 없다!");
            yield return ChooseMoveToForget(pokemon, tmItem.Move);
            yield return new WaitUntil(() => state != InventoryUIState.MoveToForget);
        }
    }

    IEnumerator ChooseMoveToForget(Pokemon pokemon, MoveBase newMove)
    {
        state = InventoryUIState.Busy;
        yield return DialogManager.Instance.ShowDialogText($"잊어버릴 기술을 선택해주세요 !",true,false);
        moveSelectionUI.gameObject.SetActive(true);
        moveSelectionUI.SetMoveData(pokemon.Moves.Select(x => x.Base).ToList(), newMove);
        moveToLearn = newMove;

        state = InventoryUIState.MoveToForget;
    }

    void UpdateItemSelection()
    {
        var slots = inventory.GetSlotsByCategory(selectedCategory);

        selectedItem = Mathf.Clamp(selectedItem, 0, slots.Count - 1);
        
        for (int i = 0; i < slotUIList.Count; i++)
        {
            if (i == selectedItem)
                slotUIList[i].NameText.color = GlobalSettings.i.HighlightedColor;
            else
                slotUIList[i].NameText.color = Color.black;
        }


        if (slots.Count > 0)
        {
            var item = slots[selectedItem].Item;
            itemIcon.sprite = item.Icon;
            itemDescription.text = item.Description;
        }
        HandleScrolling();
    }

    void HandleScrolling()
    {
        if (slotUIList.Count <= itemsInViewport)
            return;
        
        float scrollPos = Mathf.Clamp(selectedItem - itemsInViewport / 2, 0, selectedItem) * slotUIList[0].Height;
        itemListRect.localPosition = new Vector2(itemListRect.localPosition.x, scrollPos);

        bool showUpArrow = selectedItem > itemsInViewport / 2;
        upArrow.gameObject.SetActive(showUpArrow);
        bool showDownArrow = selectedItem + itemsInViewport / 2 < inventory.GetSlotsByCategory(selectedCategory).Count;
        downArrow.gameObject.SetActive(showDownArrow);

    }

    void ResetSelection()
    {
        selectedItem = 0;
        upArrow.gameObject.SetActive(false);
        downArrow.gameObject.SetActive(false);

        itemIcon.sprite = null;
        itemDescription.text = "";
    }

    void OpenPartyScreen()
    {
        state = InventoryUIState.PartySelection;
        partyScreen.gameObject.SetActive(true);
    }

    void ClosePartyScreen()
    {
        state = InventoryUIState.ItemSelection;

        partyScreen.ClearMemeberSlotMessages();
        partyScreen.gameObject.SetActive(false);
    }

    IEnumerator OnMoveToForgetSelected(int moveIndex)
    {
        var pokemon = partyScreen.SelectedMember;

        DialogManager.Instance.CloseDialog();
        moveSelectionUI.gameObject.SetActive(false);
        if (moveIndex == PokemonBase.MaxNumOfMoves)
        {
            // 기술을 배우지않는다
            yield return(DialogManager.Instance.ShowDialogText($"{pokemon.Base.Name} (이)가 {moveToLearn.Name}을 배우지 않았다!"));
        }
        else
        {
            // 기술을 잊고 새로운기술을배운다
            var selectedMove = pokemon.Moves[moveIndex].Base;

            yield return (DialogManager.Instance.ShowDialogText($"{pokemon.Base.Name} (이)가 {selectedMove.Name}을 잊고 {moveToLearn.Name}을 배웠다!"));
            pokemon.Moves[moveIndex] = new Move(moveToLearn);
        }

        moveToLearn = null;
        state = InventoryUIState.ItemSelection;
    }
}