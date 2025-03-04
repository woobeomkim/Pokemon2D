using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

// ��Ʋ�ϴ� ���ϸ��� �����ϴ�Ŭ����
public class BattleUnit : MonoBehaviour
{
    [SerializeField] PokemonBase _base;
    [SerializeField] int level;
    [SerializeField] bool isPlayerUnit;

    public Pokemon Pokemon { get; set; }

    Image image;
    Vector3 originalPos;
    Color orginalColor;

    private void Awake()
    {
        image = GetComponent<Image>();
        
        // ĵ���������� �����ġ�� ���ϱ⶧���� ���ý����Ϸ� �����´�.
        originalPos = image.transform.localPosition;
        orginalColor = image.color;
    }

    public void Setup()
    {
        Pokemon = new Pokemon(_base, level);
        if (isPlayerUnit)
            image.sprite = Pokemon.Base.BackSprite;
        else
            image.sprite = Pokemon.Base.FrontSprite;

        PlayerEnterAnimation();
    }

    /*
     Ʈ���� (Tweening)
    Ʈ������ ��ü�� �Ӽ�(��ġ,ũ��,�����) �� �ε巴�� ��ȭ��Ű�� ����Դϴ�.
    ������� � ĳ���Ͱ� �����̵��ƴ°� �ƴ϶� �ڿ������� �̵��ϵ��� �����ʹٸ� Ʈ������ ����ϸ鵵��ϴ�.
    Ʈ���� ����

    ���� �ִϸ��̼� �� �÷��̾ ���� �ö󰬴ٰ� �ڿ������� ������
    ��ư �ִϸ��̼� �� Ŭ���ϸ� ��ư�� ��¦ Ŀ���ٰ� ���� ũ��� ���ƿ�
    ���� ȭ�� �ۿ��� ���� �� ȭ�� �ۿ��� õõ�� �̵��ϸ鼭 ��Ÿ��

    DOTween�̶�?
    DOTween�� Unity���� Ʈ������ ���� ������ �� �ֵ��� �����ִ� �����Դϴ�.

    **���� ���(Lerp, Coroutine)**�� ����ϸ� Ʈ������ ����Ⱑ �����ϰ� �ڵ尡 ���������,
    DOTween�� ����ϸ� ������ �ڵ� �� �ٷ� ���� �ִϸ��̼��� ���� �� �ֽ��ϴ�.
     */

    public void PlayerEnterAnimation()
    {
        if (isPlayerUnit)
            image.transform.localPosition = new Vector3(-500f, originalPos.y);
        else
            image.transform.localPosition = new Vector3(500f, originalPos.y);

        // orginalPos.x �� duration���� ���������ϸ鼭 �ε巴�Կ�����.
        // DOLcoalMoveX -> ������ǥ DOMoveX-> ������ǥ
        image.transform.DOLocalMoveX(originalPos.x, 1f);
    }

    public void PlayAttackAnimation()
    {
        // sequence�� Append���ϸ� ���������� �ڵ�����ȴ�.
        var sequence = DOTween.Sequence();
        if (isPlayerUnit)
            sequence.Append(image.transform.DOLocalMoveX(originalPos.x + 50f, 0.25f));
        else
            sequence.Append(image.transform.DOLocalMoveX(originalPos.x + -50f, 0.25f));


        sequence.Append(image.transform.DOLocalMoveX(originalPos.x, 0.25f));
    }

    //������ ������ ȸ�����ιٲ� ������ó�� �ִϸ��̼��� �����.
    public void PlayHitAnimation()
    {
        // DOColor �̹��������� �ε巴�� �����ϴ��Լ�
        var sequence = DOTween.Sequence();
        sequence.Append(image.DOColor(Color.gray, 0.1f));
        sequence.Append(image.DOColor(orginalColor, 0.1f));

    }

    public void PlayFaintAnimation()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(image.transform.DOLocalMoveY(originalPos.y - 150f, 0.5f));
        sequence.Join(image.DOFade(0f, 0.5f));
    }
}
