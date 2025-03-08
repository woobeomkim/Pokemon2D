using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Condition 
{
    public ConditionID Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    // 컨디션상태일때 메세지
    public string StartMessage { get; set; }

    // Action은 void
    // Func는 값을반환할수있음

    /*
     비교 항목	    Action<T>	                        Func<T>
     반환값	없음    (void)	                            반환값 있음
     기본 사용 목적	작업을 수행하지만 값을 반환하지 않음	작업을 수행하고 값을 반환함
     매개변수	    0개 이상 가능	                    0개 이상 가능
     대표적인 사용 예시	Action<int> → void 함수         (int)	Func<int, string> → string 함수(int)
     
    Func<T1, T2, ..., TResult>에서 마지막 TResult가 반환 타입
    앞의 타입들은 매개변수 타입
    매개변수가 없으면 Func<TResult> 형태 (예: Func<int> → int 반환)
    즉, Func<int, string>이면 int를 받아 string을 반환하는 함수라는 의미입니다! 
     */

    public Action<Pokemon> OnStart { get; set; }
    public Func<Pokemon, bool> OnBeforeMove {  get; set; }
    public Action<Pokemon> onAfterTurn {  get; set; }
}
