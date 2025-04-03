using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Quests/Create a new quest")]
public class QuestBase : ScriptableObject
{
    [SerializeField] string name;
    [SerializeField] string description;

    [SerializeField] Dialog startDialogue;
    [SerializeField] Dialog inProgressDialogue;
    [SerializeField] Dialog completedDialogue;

    [SerializeField] ItemBase requiredItem;
    [SerializeField] ItemBase rewardItem;

    public string Name => name;
    public string Description => description;

    public Dialog StartDialog => startDialogue;
    public Dialog InProgressDialog => inProgressDialogue ?.Lines?.Count > 0 ?inProgressDialogue : startDialogue;
    public Dialog CompletedDialog => completedDialogue;

    public ItemBase RequiredItem => requiredItem;
    public ItemBase RewardItem => rewardItem;
}
