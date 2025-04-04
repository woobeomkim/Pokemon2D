using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGiver : MonoBehaviour, ISavable
{
    [SerializeField] ItemBase item;
    [SerializeField] int count = 1;
    [SerializeField] Dialog dialog;

    bool used = false;

    public IEnumerator GiveItem(PlayerController player)
    {
        yield return DialogManager.Instance.ShowDialog(dialog);

        player.GetComponent<Inventory>().AddItem(item,count);

        used = true;

        string dialogText = $"{player.Name}이 {item.Name}을 받았다";
        if(count > 1)
            dialogText = $"{player.Name}이 {item.Name} {count}개를 받았다";

        yield return DialogManager.Instance.ShowDialogText(dialogText);
    }
    
    public bool CanBeGiven()
    {
        return item != null&& count > 0 && !used;
    }

    public object CaptureState()
    {
        return used;
    }

    public void RestoreState(object state)
    {
        used = (bool)state;


    }
}
