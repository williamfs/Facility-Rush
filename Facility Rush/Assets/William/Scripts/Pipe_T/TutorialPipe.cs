﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialPipe : MonoBehaviour
{
    [SerializeField] private string[] dialog;
    [SerializeField] private int index;

    [SerializeField] private Animator anim;
    [SerializeField] private Animator pipeAnim;

    [SerializeField] private Image[] image;

    [SerializeField] private RectTransform bubblePos;
    [SerializeField] private Transform bubbleImage;
    [SerializeField] private GameObject pipeContainer;

    [SerializeField] private Text scoreText, timerText, progressText;

    private Text bubbleText;

    private void OnEnable()
    {
        bubbleText = GetComponent<Text>();
        TutorialSystem.PopDialog += ChangeText;
    }
    private void OnDisable()
    {
        TutorialSystem.PopDialog -= ChangeText;
    }

    void Start()
    {
        TutorialSystem.PopDialog(index);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))  //TODO: need to change to touchpad?
        {
            if (index < 5)
            {
                NextDialogue();  // 完成bubble text上的東西後才呼叫
            }
            else if (index >= 7)
            {
                image[0].GetComponentInChildren<Text>().text = "1+1";
                image[1].GetComponentInChildren<Text>().text = "4+1";
                image[2].GetComponentInChildren<Text>().text = "6\n+\n3";
                image[3].GetComponentInChildren<Text>().text = "3\n+\n4";
                image[4].GetComponentInChildren<Text>().text = "5";
                image[5].GetComponentInChildren<Text>().text = "10";
                image[6].GetComponentInChildren<Text>().text = "1";
                image[7].GetComponentInChildren<Text>().text = "8";

                NextDialogue();
            }
        }
    }

    void ChangeText(int index)
    {
        bubbleText.text = dialog[index];
    }

    public void NextDialogue()
    {
        if (index < dialog.Length - 1)
        {
            index++;

            if (index == 4)
            {
                pipeAnim.SetBool("IsGlowing", true);
            }
            else if (index == 5)
            {
                pipeAnim.SetBool("IsGlowing", false);

                anim.SetBool("IsFlashing_Pipe", true);
                StartCoroutine("WaitFingerAnimation");
            }
            else
            {
                anim.SetBool("IsFlashing_Pipe", false);
            }
            TutorialSystem.PopDialog(index);
        }

        if (index == 9)
        {
            bubbleImage.localPosition = bubblePos.localPosition;
            pipeContainer.SetActive(false);

            //indicator finger pops out
            anim.SetBool("PipeScore", true);
            StartCoroutine("WaitScoreAdded");
        }

        if (index == 10)
        {
            anim.SetBool("PipeScore", false);
            anim.SetBool("PipeTimer", true);

            StartCoroutine("WaitTimerDecrease");
        }
    }

    public void FalseResultDialogue()
    {
        if (index < dialog.Length - 1)
        {
            index = 6;
            TutorialSystem.PopDialog(index);

            progressText.text = "Try again!";
        }
        else if (index > dialog.Length - 1)
        {
            index = 0;
            TutorialSystem.PopDialog(index);
        }
    }

    public void CorrectResultDialogue()
    {
        if (index <= dialog.Length - 1)
        {
            index = 7;
            TutorialSystem.PopDialog(index);

            progressText.text = "Great job!";

            foreach (Image j in image)
            {
                j.raycastTarget = false;
            }

            anim.SetBool("IsFlashing_Pipe", false);
        }
        else if (index > dialog.Length - 1)
        {
            index = 0;
            TutorialSystem.PopDialog(index);
        }
    }

    IEnumerator WaitFingerAnimation()
    {
        yield return new WaitForSeconds(3f);

        foreach (Image i in image)
        {
            i.raycastTarget = true;
        }
    }

    IEnumerator WaitScoreAdded()
    {
        yield return new WaitForSeconds(1.5f);

        scoreText.text = "200";
    }

    IEnumerator WaitTimerDecrease()
    {
        yield return new WaitForSeconds(1.5f);

        timerText.text = "0:00";
    }
}