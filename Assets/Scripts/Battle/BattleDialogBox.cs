using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 배틀 대화상자를 조작하는 클래스
public class BattleDialogBox : MonoBehaviour
{
    [SerializeField] Color highlightedColor;
    [SerializeField] int lettersPerSecond;

    [SerializeField] Text dialogText;
    [SerializeField] GameObject actionSeletor;
    [SerializeField] GameObject moveSeletor;
    [SerializeField] GameObject moveDetails;

    [SerializeField] List<Text> actionTexts;
    [SerializeField] List<Text> moveTexts;
    
    [SerializeField] Text ppTexts;
    [SerializeField] Text typeTexts;

    public void SetDialog(string dialog)
    {
        dialogText.text = dialog;
    }    

    public IEnumerator TypeDialog(string dialog)
    {
        dialogText.text = "";
        foreach(var letter in dialog.ToCharArray())
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(1f / lettersPerSecond);
        }

        yield return new WaitForSeconds(1.0f);
    }

    public void EnableDialogText(bool enabled)
    {
        dialogText.enabled = enabled;
    }

    public void EnableActionSelector(bool enabled)
    {
        actionSeletor.SetActive(enabled);
    }

    public void EnableMoveSelector(bool enabled)
    {
        moveSeletor.SetActive(enabled);
        moveDetails.SetActive(enabled);
    }

    public void UpdateActionSelection(int selectedAction)
    {
        for(int i=0;i<actionTexts.Count; i++)
        {
            if (i == selectedAction)
                actionTexts[i].color = highlightedColor;
            else
                actionTexts[i].color = Color.black;
        }
    }

    public void UpdateMoveSelection(int selectedMove, Move move)
    {
        for (int i = 0; i < moveTexts.Count; i++)
        {
            if (i == selectedMove)
                moveTexts[i].color = highlightedColor;
            else
                moveTexts[i].color = Color.black;
        }

        ppTexts.text = $"PP {move.PP} / {move.Base.PP}";
        typeTexts.text = move.Base.Type.ToString();
    }

    public void SetMoveNames(List<Move> moves)
    {
        for(int i=0;i<moveTexts.Count;i++)
        {
            if (i < moves.Count)
                moveTexts[i].text = moves[i].Base.Name;
            else
                moveTexts[i].text = "-";

        }
    }
}
