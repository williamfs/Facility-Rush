﻿using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Vector3 = UnityEngine.Vector3;

/*
* Script Goals: Manage Assembly Line minigame- Entails calling for an equation to be generated based on grade level, displaying each part of the equation to be generated in the three 
* pipes, check the equation, give feedback, and go to next equation
*/
public class assemblyManager : MonoBehaviour
{
    public int gradelevel;                  // 

    public GameObject additionPanel;
    public GameObject subtractionPanel;
    public GameObject multiplicationPanel;
    public GameObject divisionPanel;

    public GameObject checkEquationButton;
    public GameObject GameOverPanel;

    public GameObject equationGenerator;

    private string chuteOneChoice;
    private string chuteTwoChoice;
    private string chuteThreeChoice;
    
    public Text[] pipeOne;
    public Text[] pipeThree;

    private int firstPart;
    private int secondPart;
    private string operatorSign;
    private int answer;

    public Text showChoice1;
    public Text showChoice2;
    public Text showChoice3;

    public Text solution;
    private int positionInPipeOne;
    private int positionInPipeTwo;
    private int positionInPipeThree;

    //int[] badPipeNumbers;

    int badPipeNumber;
    public bool gameOver;

    int score;
    public Text scoreText;
    public Text feedbackText;

    public int numberAttempted;
    public int numberCorrect;
    public int numberCorrectSoFar;

    [SerializeField]private bool isToySpawned;
    [SerializeField]private GameObject theToy;

    public GameObject timer;
    // Use this for initialization
    [SerializeField] private Animator [] assemblyLineAnimator;
    [SerializeField] private GameObject[] toyPartsForAnimator;
    [SerializeField] private GameObject[] toyPartChoices;
    [SerializeField] private GameObject[] sampleToys;
    [SerializeField] private Animator[] pipeAnimators;
    [SerializeField] private Transform[] toySpawnPoint;

    [SerializeField] private Animator toyAnimator;

    [SerializeField] private GameObject toyHolder;

    [SerializeField] private GameObject correctToy;
    [SerializeField] private GameObject incorrectToy;

    [SerializeField]private GameObject createdToy;

    [SerializeField] private GameObject[] toyPartsInPipeOne;
    [SerializeField] private GameObject[] toyPartsInPipeTwo;
    [SerializeField] private GameObject[] toyPartsInPipeThree;

   [SerializeField] GameObject bottom;
   [SerializeField] GameObject middle;
   [SerializeField] GameObject top;

   public bool isAnimating;

    float pipe1DownTime;
    float pipe1UpTime;
    float pipe2DownTime;
    float pipe2UpTime;
    float pipe3DownTime;
    float pipe3UpTime;

    float toyConveyerBeltTransition1;
    float toyConveyerBeltTransition2;
    float toyConveyerBeltTransition3;
    float toyConveyerBeltResetPosition;
    [SerializeField] private Animator theGoldenGodAnimator;
    [SerializeField] private assemblyTimer theTimer;

    [SerializeField] private Text gameTimerText;
    [SerializeField] private float gameTimer;
    [SerializeField] private float maxTime;
    public bool isInteractable;

    public int firstIncorrectNumberChoice;
    public int secondIncorrectNumberChoice;

    public int lengthOfOperatorsToCheck;
    private string[] operatorStringSigns = new string[] {"+","-","*","/"};
    public int numberOfPossibleAnswerCombinations;

    public bool testing;
    private int questionLimitForTimeBonus;
    IEnumerator newAnimationLoop(GameObject toyToInstantiate,Transform spawningPoint)
    {
        isAnimating = true;
        theTimer.setIsAnimating(true);
        theGoldenGodAnimator.SetInteger("nextTransition",0);
        yield return new WaitForSeconds(pipe1DownTime);
        //Activate Audio
        int index = Random.Range(3, 5);
        AudioManager.instance.soundAudioSource.clip = AudioManager.instance.soundClip[index];  //choose between drill or gear SFX
        AudioManager.instance.soundAudioSource.Play();
        theToy = Instantiate(toyToInstantiate, spawningPoint.position, spawningPoint.rotation);
        theToy.SetActive(true);
        Transform spotOnBelt = toyHolder.transform;
        theToy.transform.parent = toyHolder.transform;
       // theToy.transform.position = spotOnBelt.transform.position;
        //toyAnimator = theToy.GetComponent<Animator>();
        toyPartsForAnimator[0] = theToy.transform.GetChild(0).transform.GetChild(0).gameObject;
        toyPartsForAnimator[0].SetActive(true);
        toyPartsForAnimator[1] = theToy.transform.GetChild(0).transform.GetChild(1).gameObject;
        toyPartsForAnimator[1].SetActive(false);
        toyPartsForAnimator[2] = theToy.transform.GetChild(0).transform.GetChild(2).gameObject;
        toyPartsForAnimator[2].SetActive(false);
        theGoldenGodAnimator.SetInteger("nextTransition", 1);
        yield return new WaitForSeconds(pipe1UpTime);
        theGoldenGodAnimator.SetInteger("nextTransition", 2);
        yield return new WaitForSeconds(toyConveyerBeltTransition1);
        theGoldenGodAnimator.SetInteger("nextTransition",3);
        yield return new WaitForSeconds(pipe2DownTime);
        //Activate Audio
        index = Random.Range(3, 5);
        AudioManager.instance.soundAudioSource.clip = AudioManager.instance.soundClip[index];  //choose between drill or gear SFX
        AudioManager.instance.soundAudioSource.Play();
        toyPartsForAnimator[1].SetActive(true);
        theGoldenGodAnimator.SetInteger("nextTransition", 4);
        yield return new WaitForSeconds(pipe2UpTime);
        theGoldenGodAnimator.SetInteger("nextTransition", 5);
        yield return new WaitForSeconds(toyConveyerBeltTransition2);
        theGoldenGodAnimator.SetInteger("nextTransition",6);
        yield return new WaitForSeconds(pipe3DownTime);
        yield return new WaitForSeconds(.1f);
        //Activate Audio
        index = Random.Range(3, 5);
        AudioManager.instance.soundAudioSource.clip = AudioManager.instance.soundClip[index];  //choose between drill or gear SFX
        AudioManager.instance.soundAudioSource.Play();
        toyPartsForAnimator[2].SetActive(true);
        theGoldenGodAnimator.SetInteger("nextTransition", 7);
        yield return new WaitForSeconds(pipe3UpTime);
        theGoldenGodAnimator.SetInteger("nextTransition",8);
        yield return new WaitForSeconds(toyConveyerBeltTransition3);
        theToy.SetActive(false);
        Destroy(theToy);
        theGoldenGodAnimator.SetInteger("nextTransition", 9);
        yield return new WaitForSeconds(toyConveyerBeltResetPosition);
        theGoldenGodAnimator.SetInteger("nextTransition",-1);
        //
        deleteToyParts();
        theTimer.setIsAnimating(false);
        isAnimating = false;
        nextEquation();
    }
    public void setChuteOneChoice(string choice)
    {
        chuteOneChoice = choice;
    }

    public void setChuteTwoChoice(string choice)
    {
        chuteTwoChoice = choice;
    }

    public void setChuteThreeChoice(string choice)
    {
        chuteThreeChoice = choice;
    }

    public void componentChoice(int choiceIdentifier)
    {
      switch(choiceIdentifier)
        {
            case 0:
              
                bottom = toyPartsInPipeOne[0];
                
                break;
            case 1:
               
                bottom = toyPartsInPipeOne[1];
               
                break;
            case 2:
               
                middle = toyPartsInPipeTwo[0];
               
                break;
            case 3:
               
                middle = toyPartsInPipeTwo[1];
                
                break;
            case 4:
               
                middle = toyPartsInPipeTwo[2];
                break;
            case 5:
                middle = toyPartsInPipeTwo[3];
                break;
            case 6:
                top = toyPartsInPipeThree[0];
                break;
            case 7:
               
                top = toyPartsInPipeThree[1];
                break;

        }
    }
    void Start ()
    {
        questionLimitForTimeBonus = 3;
        isInteractable = false;
       // GameObject trial=Instantiate(sampleToys[0], createdToy.transform.position, createdToy.transform.rotation);
       // GameObject changedPart = trial.transform.GetChild(1).gameObject;
        //changedPart = sampleToys[1].transform.GetChild(1).gameObject;
        //trial.transform.parent = createdToy.transform;
        toyPartsInPipeOne = new GameObject[2];
        toyPartsInPipeThree = new GameObject[2];
        toyPartsInPipeTwo = new GameObject[4];
        pipe1DownTime=theGoldenGodAnimator.runtimeAnimatorController.animationClips[4].length;
        pipe1UpTime= theGoldenGodAnimator.runtimeAnimatorController.animationClips[5].length;
        pipe2DownTime= theGoldenGodAnimator.runtimeAnimatorController.animationClips[6].length;
        pipe2UpTime = theGoldenGodAnimator.runtimeAnimatorController.animationClips[7].length;
       // print("This is pipe 2 up time: "+ pipe2UpTime);
        pipe3DownTime = theGoldenGodAnimator.runtimeAnimatorController.animationClips[8].length;
        pipe3UpTime = theGoldenGodAnimator.runtimeAnimatorController.animationClips[9].length;

        toyConveyerBeltTransition1= theGoldenGodAnimator.runtimeAnimatorController.animationClips[0].length;
        toyConveyerBeltTransition2 = theGoldenGodAnimator.runtimeAnimatorController.animationClips[1].length;
        toyConveyerBeltTransition3 = theGoldenGodAnimator.runtimeAnimatorController.animationClips[2].length;
        toyConveyerBeltResetPosition = theGoldenGodAnimator.runtimeAnimatorController.animationClips[3].length;

        isAnimating = false;
        isToySpawned = false;
        numberAttempted=0;
        numberCorrect=0;
        numberCorrectSoFar=0;
        gameTimerText.text = "1:00";
        gameOver = true;
        feedbackText.gameObject.SetActive(false);
        score = 0;
        scoreText.text = "" + score;
        showChoice1.text = "";
        showChoice2.text = "";
        showChoice3.text = "";
        gradelevel = PlayerPrefs.GetInt("grade");
        initiateProperMiddlePanel();
        nextEquation();

    }
    public void initiateProperMiddlePanel()
    {
        switch(gradelevel)
        {
            case 0:
                additionPanel.SetActive(true);
                lengthOfOperatorsToCheck = 1;
                break;
            case 1:
                additionPanel.SetActive(true);
                subtractionPanel.SetActive(true);
                lengthOfOperatorsToCheck = 2;
                break;
            case 2:
                additionPanel.SetActive(true);
                subtractionPanel.SetActive(true);
                lengthOfOperatorsToCheck = 2;
                break;
            case 3:
                additionPanel.SetActive(true);
                subtractionPanel.SetActive(true);
                multiplicationPanel.SetActive(true);
                lengthOfOperatorsToCheck = 3;
                //divisionPanel.SetActive(true);
                break;
            case 4:
                additionPanel.SetActive(true);
                subtractionPanel.SetActive(true);
                multiplicationPanel.SetActive(true);
                divisionPanel.SetActive(true);
                lengthOfOperatorsToCheck = 4;
                break;
            case 5:
                additionPanel.SetActive(true);
                subtractionPanel.SetActive(true);
                multiplicationPanel.SetActive(true);
                divisionPanel.SetActive(true);
                lengthOfOperatorsToCheck = 4;
                break;
        }
    }
    public int calculateChanceOfGradeProblem()
    {
        int gradeNumberToReturn = -5;
        int decidingVariable = Random.Range(1, 11);
        switch (gradelevel)
        {
            case 0:

                gradeNumberToReturn = 0;
                break;

            case 1:
                if(decidingVariable<=3)
                {
                    gradeNumberToReturn = 0;
                }
                else
                {
                    gradeNumberToReturn = 1;
                }
                break;
            case 2:
                if(decidingVariable<3)
                {
                    gradeNumberToReturn = 0;
                }
                else if(decidingVariable>=3 && decidingVariable<5)
                {
                    gradeNumberToReturn = 1;
                }
                else
                {
                    gradeNumberToReturn = 2;
                }
                break;
            case 3:
                if(decidingVariable==1)
                {
                    gradeNumberToReturn = 0;
                }
                else if(decidingVariable==2 || decidingVariable==3)
                {
                    gradeNumberToReturn = 1;
                }
                else if(decidingVariable==4 || decidingVariable==5)
                {
                    gradeNumberToReturn = 2;
                }
                else
                {
                    gradeNumberToReturn = 3;
                }
                break;
            case 4:
                if(decidingVariable==1)
                {
                    gradeNumberToReturn = 0;
                }
                else if(decidingVariable==2)
                {
                    gradeNumberToReturn = 1;
                }
                else if(decidingVariable ==3)
                {
                    gradeNumberToReturn = 2;
                }
                else if(decidingVariable==4||decidingVariable==5)
                {
                    gradeNumberToReturn = 3;
                }
                else
                {
                    gradeNumberToReturn = 4;
                }
                break;
            case 5:
                if(decidingVariable==1)
                {
                    gradeNumberToReturn = 0;
                }
                else if(decidingVariable==2)
                {
                    gradeNumberToReturn = 1;
                }
                else if(decidingVariable==3)
                {
                    gradeNumberToReturn = 2;
                }
                else if(decidingVariable==4)
                {
                    gradeNumberToReturn = 3;
                }
                else if(decidingVariable==5)
                {
                    gradeNumberToReturn = 4;
                }
                else
                {
                    gradeNumberToReturn = 5;
                }
                break;
        }
        return gradeNumberToReturn;
    }
    public void nextEquation()
    {
        int indexOfCorrectToy = Random.Range(0, sampleToys.Length);
        int indexOfIncorrectToy = Random.Range(0, sampleToys.Length);
            while(indexOfCorrectToy==indexOfIncorrectToy)
            {
            indexOfIncorrectToy = Random.Range(0, sampleToys.Length);
            }
        correctToy = sampleToys[indexOfCorrectToy];
        incorrectToy = sampleToys[indexOfIncorrectToy];
        string equation=equationGenerator.GetComponent<generateEquations>().generateEquation(gradelevel);
       // print(equation);
       while(equation.Contains("*1")|| equation.Contains("/1"))
        {
            equation = equationGenerator.GetComponent<generateEquations>().generateEquation(gradelevel);
        }
        //print(ExpressionEvaluator.Evaluate<int>("4"));
        breakdownEquation(equation);
        solution.text = answer + "";
    }
    public void determinePositionInPipe2()
    {
        switch(operatorSign)
        {
            case "+":
                positionInPipeTwo = 0;
               // print("the position for 2 is: " + positionInPipeTwo);
                break;
            case "-":
                positionInPipeTwo = 1;
               // print("the position for 2 is: " + positionInPipeTwo);
                break;
            case "*":
                positionInPipeTwo = 2;
               // print("the position for 2 is: "+ positionInPipeTwo);
                break;
            case "/":
                positionInPipeTwo = 3;
               // print("the position for 2 is: " + positionInPipeTwo);
                break;

            default:
               // print("Pipe 2 Poisition has not been established");
                break;
        }

        for(int i=0;i<toyPartsInPipeTwo.Length;i++)
        {
            if(i!=positionInPipeTwo)
            {
                toyPartsInPipeTwo[i] = incorrectToy.transform.GetChild(1).gameObject;
            }
        }
        toyPartsInPipeTwo[positionInPipeTwo] = correctToy.transform.GetChild(1).gameObject;
    }
    public void breakdownEquation(string wholeEquation)
    {
        string numericalValue="";
        for(int i=0; i<wholeEquation.Length;i++)
        {
           // print("wholeEquation[i] is " + wholeEquation[i]);
            if(wholeEquation[i].Equals('+')|| wholeEquation[i].Equals('-')|| wholeEquation[i].Equals('*')|| wholeEquation[i].Equals('/'))
            {
                operatorSign = wholeEquation[i]+"";
                //ExpressionEvaluator.Evaluate<int>(numericalValue,out firstPart);
                firstPart = int.Parse(numericalValue);
                //operatorSign = ""+wholeEquation[i];
                numericalValue = "";
            }
            else if(wholeEquation[i].Equals('='))
            {
                //ExpressionEvaluator.Evaluate<int>(numericalValue,out secondPart);
                secondPart = int.Parse(numericalValue);
                numericalValue = "";
            }
            else
            {
                numericalValue += wholeEquation[i];
                //print("Right now numerical value is: " + numericalValue);
            }
        }
        //ExpressionEvaluator.Evaluate<int>(numericalValue,out answer);
        answer = int.Parse(numericalValue);
        positionInPipeOne = Random.Range(0, 2);
        positionInPipeThree= Random.Range(0, 2);
        determinePositionInPipe2();
        pipeOne[positionInPipeOne].text = firstPart + "";
        toyPartsInPipeOne[positionInPipeOne] = correctToy.transform.GetChild(0).gameObject;
        pipeThree[positionInPipeThree].text = secondPart + "";
        toyPartsInPipeThree[positionInPipeThree] = correctToy.transform.GetChild(2).gameObject;
        solution.text = answer + "";
        setNumberinPipe(firstPart, pipeOne,positionInPipeOne,toyPartsInPipeOne);
        firstIncorrectNumberChoice = badPipeNumber;
        setNumberinPipe(secondPart, pipeThree,positionInPipeThree,toyPartsInPipeThree);
        secondIncorrectNumberChoice = badPipeNumber;
        checkForMultipleRightAnswers();
    }
    
    public void setNumberinPipe(int numberInPipe,Text [] pipe,int positionInPipe, GameObject[] arrayForToyPartSelection)
    {
       // int k = 0;
        determineRandomNumbers(gradelevel, numberInPipe);
        for(int i =0;i<2;i++)
        {
            if(i!=positionInPipe)
            {
                pipe[i].text=badPipeNumber+"";
                arrayForToyPartSelection[i] = incorrectToy.transform.GetChild(i).gameObject;
                //k++;
            }
            
        }
    }

    public void determineRandomNumbers(int gradeLevel, int doNotEqual)
    {
        int firstGenerated=0;
        //int secondGenerated=0;
        switch(gradeLevel)
        {
            case 0:
                do
                {
                    firstGenerated= Random.Range(1, 11);
                    
                } while (firstGenerated == doNotEqual);
                badPipeNumber = firstGenerated;
                break;
            case 1:
                do
                {
                    firstGenerated = Random.Range(1, 21);
                    
                } while (firstGenerated == doNotEqual);
                badPipeNumber= firstGenerated;
                break;
            case 2:
                do
                {
                    firstGenerated = Random.Range(1, 101);
                } while (firstGenerated == doNotEqual);
                badPipeNumber= firstGenerated;
                break;
            case 3:
                do
                {
                    firstGenerated = Random.Range(1, 101);
                } while (firstGenerated == doNotEqual);
                badPipeNumber= firstGenerated;
                break;
            case 4:
                do
                {
                    firstGenerated = Random.Range(1, 101);
                } while (firstGenerated == doNotEqual);
                badPipeNumber= firstGenerated;
                break;
            case 5:
                do
                {
                    firstGenerated = Random.Range(1, 101);
                } while (firstGenerated == doNotEqual);
                badPipeNumber= firstGenerated;
                break;
            default:
               // print("nope");
                break;
        }
        
    }
    public void checkForMultipleRightAnswers()
    {
        numberOfPossibleAnswerCombinations = 0;
        for(int i=0;i<lengthOfOperatorsToCheck;i++)
        {
            for(int j=0;j<2;j++)
            {
                if(solution.text.Contains(evaluateEquation(pipeOne[j].text+operatorStringSigns[i]+pipeThree[j].text).ToString()))
                {
                    numberOfPossibleAnswerCombinations++;
                }
            }
        }

        if(numberOfPossibleAnswerCombinations>=2)
        {
            int randomProblemChoice=0;
            switch(gradelevel)
            {
                case 0:
                    randomProblemChoice = Random.Range(0, 2);
                    break;
                case 1:
                    randomProblemChoice = Random.Range(0, 4);
                    break;
                case 2:
                    randomProblemChoice = Random.Range(0, 6);
                    break;
                case 3:
                    randomProblemChoice = Random.Range(5, 8);
                    break;
                case 4:
                    randomProblemChoice = Random.Range(5, 10);
                    break;
                case 5:
                    randomProblemChoice = Random.Range(5, 12);
                    break;
            }
            switch(randomProblemChoice)
            {
                case 0:
                    pipeOne[positionInPipeOne].text = "1";
                    pipeThree[positionInPipeThree].text = "2";
                    for (int i = 0; i < pipeOne.Length; i++)
                    {
                        if (i != positionInPipeOne)
                        {
                            pipeOne[i].text = "9";
                        }
                    }
                    for (int i = 0; i < pipeThree.Length; i++)
                    {
                        if (i != positionInPipeThree)
                        {
                            pipeThree[i].text = "8";
                        }
                    }
                    answer = 3;
                    operatorSign = "+";
                    determinePositionInPipe2();
                    break;
                case 1:
                    pipeOne[positionInPipeOne].text = "3";
                    pipeThree[positionInPipeThree].text = "4";
                    for (int i = 0; i < pipeOne.Length; i++)
                    {
                        if (i != positionInPipeOne)
                        {
                            pipeOne[i].text = "1";
                        }
                    }
                    for (int i = 0; i < pipeThree.Length; i++)
                    {
                        if (i != positionInPipeThree)
                        {
                            pipeThree[i].text = "2";
                        }
                    }
                    answer = 7;
                    operatorSign = "+";
                    determinePositionInPipe2();
                    break;
                case 2:
                    pipeOne[positionInPipeOne].text = "11";
                    pipeThree[positionInPipeThree].text = "2";
                    for (int i = 0; i < pipeOne.Length; i++)
                    {
                        if (i != positionInPipeOne)
                        {
                            pipeOne[i].text = "5";
                        }
                    }
                    for (int i = 0; i < pipeThree.Length; i++)
                    {
                        if (i != positionInPipeThree)
                        {
                            pipeThree[i].text = "10";
                        }
                    }
                    answer = 9;
                    operatorSign = "-";
                    determinePositionInPipe2();
                    break;
                case 3:
                    pipeOne[positionInPipeOne].text = "9";
                    pipeThree[positionInPipeThree].text = "4";
                    for (int i = 0; i < pipeOne.Length; i++)
                    {
                        if (i != positionInPipeOne)
                        {
                            pipeOne[i].text = "20";
                        }
                    }
                    for (int i = 0; i < pipeThree.Length; i++)
                    {
                        if (i != positionInPipeThree)
                        {
                            pipeThree[i].text = "19";
                        }
                    }
                    answer = 13;
                    operatorSign = "+";
                    determinePositionInPipe2();
                    break;
                case 4:
                    pipeOne[positionInPipeOne].text = "11";
                    pipeThree[positionInPipeThree].text = "12";
                    for (int i = 0; i < pipeOne.Length; i++)
                    {
                        if (i != positionInPipeOne)
                        {
                            pipeOne[i].text = "9";
                        }
                    }
                    for (int i = 0; i < pipeThree.Length; i++)
                    {
                        if (i != positionInPipeThree)
                        {
                            pipeThree[i].text = "6";
                        }
                    }
                    answer = 23;
                    operatorSign = "+";
                    determinePositionInPipe2();
                    break;
                case 5:
                    pipeOne[positionInPipeOne].text = "45";
                    pipeThree[positionInPipeThree].text = "44";
                    for (int i = 0; i < pipeOne.Length; i++)
                    {
                        if (i != positionInPipeOne)
                        {
                            pipeOne[i].text = "27";
                        }
                    }
                    for (int i = 0; i < pipeThree.Length; i++)
                    {
                        if (i != positionInPipeThree)
                        {
                            pipeThree[i].text = "42";
                        }
                    }
                    answer = 89;
                    operatorSign = "+";
                    determinePositionInPipe2();
                    break;
                case 6:
                    pipeOne[positionInPipeOne].text = "9";
                    pipeThree[positionInPipeThree].text = "5";
                    for (int i = 0; i < pipeOne.Length; i++)
                    {
                        if (i != positionInPipeOne)
                        {
                            pipeOne[i].text = "2";
                        }
                    }
                    for (int i = 0; i < pipeThree.Length; i++)
                    {
                        if (i != positionInPipeThree)
                        {
                            pipeThree[i].text = "1";
                        }
                    }
                    answer = 45;
                    operatorSign = "*";
                    determinePositionInPipe2();
                    break;
                case 7:
                    pipeOne[positionInPipeOne].text = "7";
                    pipeThree[positionInPipeThree].text = "6";
                    for (int i = 0; i < pipeOne.Length; i++)
                    {
                        if (i != positionInPipeOne)
                        {
                            pipeOne[i].text = "5";
                        }
                    }
                    for (int i = 0; i < pipeThree.Length; i++)
                    {
                        if (i != positionInPipeThree)
                        {
                            pipeThree[i].text = "3";
                        }
                    }
                    answer = 42;
                    operatorSign = "*";
                    determinePositionInPipe2();
                    break;
                case 8:
                    pipeOne[positionInPipeOne].text = "50";
                    pipeThree[positionInPipeThree].text = "12";
                    for (int i = 0; i < pipeOne.Length; i++)
                    {
                        if (i != positionInPipeOne)
                        {
                            pipeOne[i].text = "20";
                        }
                    }
                    for (int i = 0; i < pipeThree.Length; i++)
                    {
                        if (i != positionInPipeThree)
                        {
                            pipeThree[i].text = "5";
                        }
                    }
                    answer = 600;
                    operatorSign = "*";
                    determinePositionInPipe2();
                    break;
                case 9:
                    pipeOne[positionInPipeOne].text = "20";
                    pipeThree[positionInPipeThree].text = "9";
                    for (int i = 0; i < pipeOne.Length; i++)
                    {
                        if (i != positionInPipeOne)
                        {
                            pipeOne[i].text = "10";
                        }
                    }
                    for (int i = 0; i < pipeThree.Length; i++)
                    {
                        if (i != positionInPipeThree)
                        {
                            pipeThree[i].text = "25";
                        }
                    }
                    answer = 180;
                    operatorSign = "*";
                    determinePositionInPipe2();
                    break;
                case 10:
                    pipeOne[positionInPipeOne].text = "180";
                    pipeThree[positionInPipeThree].text = "5";
                    for (int i = 0; i < pipeOne.Length; i++)
                    {
                        if (i != positionInPipeOne)
                        {
                            pipeOne[i].text = "6";
                        }
                    }
                    for (int i = 0; i < pipeThree.Length; i++)
                    {
                        if (i != positionInPipeThree)
                        {
                            pipeThree[i].text = "20";
                        }
                    }
                    answer =900;
                    operatorSign = "*";
                    determinePositionInPipe2();
                    break;
                case 11:
                    pipeOne[positionInPipeOne].text = "300";
                    pipeThree[positionInPipeThree].text = "6";
                    for (int i = 0; i < pipeOne.Length; i++)
                    {
                        if (i != positionInPipeOne)
                        {
                            pipeOne[i].text = "1";
                        }
                    }
                    for (int i = 0; i < pipeThree.Length; i++)
                    {
                        if (i != positionInPipeThree)
                        {
                            pipeThree[i].text = "900";
                        }
                    }
                    answer = 50;
                    operatorSign = "/";
                    determinePositionInPipe2();
                    break;
            }
        }
    }
   
	// Update is called once per frame
	void Update ()
    {
        if (gameOver == false && isAnimating==false)
        {
            gameTimer -= Time.deltaTime;


            int seconds = (int)(gameTimer % 60);
            int minutes = (int)(gameTimer / 60) % 60;
            int hours = (int)(gameTimer / 3600) % 24;

            string timerString = string.Format("{0:00}:{1:00}", minutes, seconds);
            if (timerString.Equals("00:00"))
            {
                initiateGameOver();
            }
            gameTimerText.text = timerString;
        }
    }

    public void initiateGameOver()
    {
        checkEquationButton.SetActive(false);
        gameOver = true;
        GameOverPanel.SetActive(true);
        feedbackText.text = "GameOver";
        GameOverPanel.GetComponent<gameOverPanel>().crossCheckScores(0, score);
        calculateRestoration();
    }
    public void checkForNewHighScore()
    {
        int currentHighScore = PlayerPrefs.GetInt("assemblyHighScore");
        if (score > currentHighScore)
        {
            PlayerPrefs.SetInt("assemblyHighScore", score);
            PlayerPrefs.Save();
        }
    }
    public void calculateRestoration()
    {
        DegradationManager degredationManager = GameObject.FindGameObjectWithTag("degredationManager").GetComponent<DegradationManager>();
        degredationManager.aAttempted = numberAttempted;
        degredationManager.aCorrect = numberCorrect;
        degredationManager.assemblyCalulate();
        degredationManager.gameHasBeenPlayed(1);
        checkForNewHighScore();
    }
    public void createToy()
    {
        GameObject createdBottom=Instantiate(bottom, createdToy.transform.GetChild(0).position, createdToy.transform.GetChild(0).rotation);
        createdBottom.transform.parent = createdToy.transform.GetChild(0).transform;
        createdBottom.transform.position = correctToy.transform.GetChild(0).transform.position;

        GameObject createdMiddle=Instantiate(middle, createdToy.transform.GetChild(1).position, createdToy.transform.GetChild(1).rotation);
        createdMiddle.transform.parent = createdToy.transform.GetChild(0).transform;
        createdMiddle.transform.position = correctToy.transform.GetChild(1).transform.position;

        GameObject createdTop=Instantiate(top, createdToy.transform.GetChild(2).position, createdToy.transform.GetChild(2).rotation);
        createdTop.transform.parent = createdToy.transform.GetChild(0).transform;
        createdTop.transform.position= correctToy.transform.GetChild(2).transform.position;


    }
    public void deleteToyParts()
    {
        Destroy(createdToy.transform.GetChild(0).transform.GetChild(0).gameObject);
        Destroy(createdToy.transform.GetChild(0).transform.GetChild(1).gameObject);
        Destroy(createdToy.transform.GetChild(0).transform.GetChild(2).gameObject);
    }
    public int evaluateEquation(string equation)
    {
        string numericalValue = "";
        string operatorSign = "";
        int firstOperand = 0;
        int secondOperand = 0;
        int answer = 0; ;
        for (int i = 0; i < equation.Length; i++)
        {
            if (equation[i].Equals('+') || equation[i].Equals('-') || equation[i].Equals('*') || equation[i].Equals('/'))
            {
                operatorSign = equation[i] + "";
                firstOperand = int.Parse(numericalValue);
                numericalValue = "";
            }
            else if (i == equation.Length - 1)
            {
                numericalValue += equation[i];
                secondOperand = int.Parse(numericalValue);
                numericalValue = "";
            }
            else
            {
                numericalValue += equation[i];
            }
        }

        switch (operatorSign)
        {
            case "+":
                answer = firstOperand + secondOperand;
                break;
            case "-":
                answer = firstOperand - secondOperand;
                break;
            case "*":
                answer = firstOperand * secondOperand;
                break;
            case "/":
                answer = firstOperand / secondOperand;
                break;
        }
        return answer;
    }
    public void checkequation()
    {
        if (isAnimating==false && isInteractable==true && showChoice1.text !="" && showChoice2.text != "" && showChoice3.text !="")
        {
            createToy();
            string playerEquation = chuteOneChoice + chuteTwoChoice + chuteThreeChoice;
            feedbackText.gameObject.SetActive(true);
            int temp;
            temp = evaluateEquation(playerEquation);

            if (temp == answer && isAnimating == false)
            {
                StartCoroutine(newAnimationLoop(createdToy, toySpawnPoint[0]));
                score += 100;
                PlayerPrefs.SetInt("recentAssemblyHighScore", score);
                scoreText.text = "" + score;
                feedbackText.text = "Correct";
                numberCorrect++;
                numberCorrectSoFar++;
                numberAttempted++;
                if (numberCorrectSoFar == questionLimitForTimeBonus)
                {
                    addTime();
                    numberCorrectSoFar = 0;
                    questionLimitForTimeBonus++;
                }

                isToySpawned = false;
            }
            else
            {
                feedbackText.text = "Incorrect";
                StartCoroutine(newAnimationLoop(createdToy, toySpawnPoint[0]));
                numberAttempted++;
                nextEquation();

                isToySpawned = false;
                nextEquation();
            }
            
            showChoice1.text = "";
            showChoice2.text = "";
            showChoice3.text = "";
        }
    }

    public void addTime()
    {
        if (gameTimer + 5f > maxTime)
        {
            gameTimer = maxTime;
        }
        else
        {
            gameTimer += 5f;
        }
    }
}
