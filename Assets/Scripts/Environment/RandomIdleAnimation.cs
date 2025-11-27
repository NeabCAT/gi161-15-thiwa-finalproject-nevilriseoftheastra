using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomIdleAnimation : MonoBehaviour
{
    private Animator myAnimator;

    private void Awake()
    {
        myAnimator = GetComponent<Animator>();
    }

    private void Start()
    {
        // รอ 1 frame ให้ Animator เริ่มทำงานก่อน
        StartCoroutine(RandomizeAnimation());
    }

    private IEnumerator RandomizeAnimation()
    {
        yield return null; // รอ 1 frame

        AnimatorStateInfo state = myAnimator.GetCurrentAnimatorStateInfo(0);

        // เล่นอนิเมชั่นเดิม แต่เริ่มจากจุดสุ่ม
        myAnimator.Play(state.fullPathHash, 0, Random.Range(0f, 1f));
    }
}