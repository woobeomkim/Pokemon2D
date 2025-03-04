using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

// 배틀하는 포켓몬을 관리하는클래스
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
        
        // 캔버스에대한 상대위치를 원하기때문에 로컬스케일로 가져온다.
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
     트위닝 (Tweening)
    트위닝은 객체의 속성(위치,크기,색상등) 을 부드럽게 변화시키는 기술입니다.
    예를들어 어떤 캐릭터가 순간이도아는게 아니라 자연스럽게 이동하도록 만들고싶다면 트위닝을 사용하면도비니다.
    트위닝 예시

    점프 애니메이션 → 플레이어가 위로 올라갔다가 자연스럽게 내려옴
    버튼 애니메이션 → 클릭하면 버튼이 살짝 커졌다가 원래 크기로 돌아옴
    적이 화면 밖에서 등장 → 화면 밖에서 천천히 이동하면서 나타남

    DOTween이란?
    DOTween은 Unity에서 트위닝을 쉽게 구현할 수 있도록 도와주는 도구입니다.

    **기존 방법(Lerp, Coroutine)**을 사용하면 트위닝을 만들기가 복잡하고 코드가 길어지지만,
    DOTween을 사용하면 간단한 코드 몇 줄로 쉽게 애니메이션을 만들 수 있습니다.
     */

    public void PlayerEnterAnimation()
    {
        if (isPlayerUnit)
            image.transform.localPosition = new Vector3(-500f, originalPos.y);
        else
            image.transform.localPosition = new Vector3(500f, originalPos.y);

        // orginalPos.x 로 duration동안 선형보간하면서 부드럽게움직임.
        // DOLcoalMoveX -> 로컬좌표 DOMoveX-> 월드좌표
        image.transform.DOLocalMoveX(originalPos.x, 1f);
    }

    public void PlayAttackAnimation()
    {
        // sequence에 Append를하면 순차적으로 자동실행된다.
        var sequence = DOTween.Sequence();
        if (isPlayerUnit)
            sequence.Append(image.transform.DOLocalMoveX(originalPos.x + 50f, 0.25f));
        else
            sequence.Append(image.transform.DOLocalMoveX(originalPos.x + -50f, 0.25f));


        sequence.Append(image.transform.DOLocalMoveX(originalPos.x, 0.25f));
    }

    //빠르게 색상을 회색으로바꿔 맞은것처럼 애니메이션을 만든다.
    public void PlayHitAnimation()
    {
        // DOColor 이미지색상을 부드럽게 변경하는함수
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
