using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ComputerManager : MonoBehaviour
{
    public FirstPersonController FirstPersonController;

    [Header("Screens")]
    public GameObject BootingScreen;
    public GameObject Desktop;
    public GameObject DatingApp;
    public GameObject ExcelApp;
    public GameObject BrowserApp;
    public GameObject notepadApp;
    public GameObject WordApp;
    public GameObject LockedScreen;
    public GameObject BlackScreen;

    

    [Header("Animations")]
    public Animator Intro;
    public string animationName;

    [Header("QuestionsWindows")]
    public GameObject Windows1;
    public GameObject Windows2;
    public GameObject Windows3;
    public GameObject Windows4;
    public GameObject Windows5;
    public GameObject Windows6;
    public GameObject Windows7;
    public GameObject Windows8;
    public GameObject Windows9;
    public GameObject Windows10;
    public GameObject Windows11;
    public GameObject Windows12;


    private void Start()
    {
        
       
        Windows1.SetActive(false);
        Windows2.SetActive(false);
        Windows3.SetActive(false);
        Windows4.SetActive(false);
        Windows5.SetActive(false);
        Windows6.SetActive(false);
        Windows7.SetActive(false);
        Windows8.SetActive(false);
        Windows9.SetActive(false);
        Windows10.SetActive(false);
        Windows11.SetActive(false);
        Windows12.SetActive(false);

        Desktop.SetActive(false);
        DatingApp.SetActive(false);
        ExcelApp.SetActive(false);
        BrowserApp.SetActive(false);
        notepadApp.SetActive(false);
        WordApp.SetActive(false);
        BlackScreen.SetActive(false);
        BootingScreen.SetActive(false);

    }

    
    public void StartEngine()
    {
        LockedScreen.SetActive(false);
        BootingScreen.SetActive(true);
        StartCoroutine(PlayAnimation());

    }

    private IEnumerator PlayAnimation()
    {
        Intro.Play(animationName);
        yield return new WaitForSeconds(5);
        Intro.StopPlayback();
        BootingScreen.SetActive(false);
        Desktop.SetActive(true);


    }

    public void OnClickDating()
    {
        DatingApp.SetActive(true);
        ExcelApp.SetActive(false);
        BrowserApp.SetActive(false);
        WordApp.SetActive(false);
        notepadApp.SetActive(false);

    }

    public void OnClickLeaveDating()
    {
        DatingApp.SetActive(false);
    }

   public void OnClickInternet()
    {
        BrowserApp.SetActive(true);
        Windows1.SetActive(true);
        notepadApp.SetActive(false);
        WordApp.SetActive(false);
        DatingApp.SetActive(false);
    }

    public void OnClickLeaveBrowser()
    {
        BrowserApp.SetActive(false);
    }

    public void OnClickWord()
    {
        WordApp.SetActive(true);
        DatingApp.SetActive(false);
        ExcelApp.SetActive(false);
        BrowserApp.SetActive(false);
        notepadApp.SetActive(false);
    }

    public void OnClickLeaveWord()
    {
        WordApp.SetActive(false);
    }

    public void OnClickNotepad()
    {
        notepadApp.SetActive(true);
        DatingApp.SetActive(false);
        ExcelApp.SetActive(false);
        BrowserApp.SetActive(false);
        WordApp.SetActive(false);
        
    }

    public void OnClickLeaveNotepad()
    {
        notepadApp.SetActive(false);
    }


    public void OnClickExcel()
    {
        ExcelApp.SetActive(true);
        DatingApp.SetActive(false);
        BrowserApp.SetActive(false);
        WordApp.SetActive(false);
        notepadApp.SetActive(false);
    }

    public void OnCLickLeaveExcel()
    {
        ExcelApp.SetActive(false);
    }

    public void OnClickMenuButton()
    {
        DatingApp.SetActive(false);
        ExcelApp.SetActive(false);
        BrowserApp.SetActive(false);
        notepadApp.SetActive(false);
        WordApp.SetActive(false);
    }

    public void OnClickButtonWindow1()
    {
        Windows1.SetActive(true);
        Windows2.SetActive(false);
        Windows3.SetActive(false);
        Windows4.SetActive(false);
        Windows5.SetActive(false);
        Windows6.SetActive(false);
        Windows7.SetActive(false);
        Windows8.SetActive(false);
        Windows9.SetActive(false);
        Windows10.SetActive(false);
        Windows11.SetActive(false);
        Windows12.SetActive(false);

    }
    public void OnClickButtonWindow2()
    {
        Windows1.SetActive(false);
        Windows2.SetActive(true);
        Windows3.SetActive(false);
        Windows4.SetActive(false);
        Windows5.SetActive(false);
        Windows6.SetActive(false);
        Windows7.SetActive(false);
        Windows8.SetActive(false);
        Windows9.SetActive(false);
        Windows10.SetActive(false);
        Windows11.SetActive(false);
        Windows12.SetActive(false);
    }
    public void OnClickButtonWindow3()
    {
        Windows1.SetActive(false);
        Windows2.SetActive(false);
        Windows3.SetActive(true);
        Windows4.SetActive(false);
        Windows5.SetActive(false);
        Windows6.SetActive(false);
        Windows7.SetActive(false);
        Windows8.SetActive(false);
        Windows9.SetActive(false);
        Windows10.SetActive(false);
        Windows11.SetActive(false);
        Windows12.SetActive(false);
    }
    public void OnClickButtonWindow4()
    {
        Windows1.SetActive(false);
        Windows2.SetActive(false);
        Windows3.SetActive(false);
        Windows4.SetActive(true);
        Windows5.SetActive(false);
        Windows6.SetActive(false);
        Windows7.SetActive(false);
        Windows8.SetActive(false);
        Windows9.SetActive(false);
        Windows10.SetActive(false);
        Windows11.SetActive(false);
        Windows12.SetActive(false);
    }
    public void OnClickButtonWindow5()
    {
        Windows1.SetActive(false);
        Windows2.SetActive(false);
        Windows3.SetActive(false);
        Windows4.SetActive(false);
        Windows5.SetActive(true);
        Windows6.SetActive(false);
        Windows7.SetActive(false);
        Windows8.SetActive(false);
        Windows9.SetActive(false);
        Windows10.SetActive(false);
        Windows11.SetActive(false);
        Windows12.SetActive(false);
    }
    public void OnClickButtonWindow6()
    {
        Windows1.SetActive(false);
        Windows2.SetActive(false);
        Windows3.SetActive(false);
        Windows4.SetActive(false);
        Windows5.SetActive(false);
        Windows6.SetActive(true);
        Windows7.SetActive(false);
        Windows8.SetActive(false);
        Windows9.SetActive(false);
        Windows10.SetActive(false);
        Windows11.SetActive(false);
        Windows12.SetActive(false);
    }
    public void OnClickButtonWindow7()
    {
        Windows1.SetActive(false);
        Windows2.SetActive(false);
        Windows3.SetActive(false);
        Windows4.SetActive(false);
        Windows5.SetActive(false);
        Windows6.SetActive(false);
        Windows7.SetActive(true);
        Windows8.SetActive(false);
        Windows9.SetActive(false);
        Windows10.SetActive(false);
        Windows11.SetActive(false);
        Windows12.SetActive(false);
    }
    public void OnClickButtonWindow8()
    {
        Windows1.SetActive(false);
        Windows2.SetActive(false);
        Windows3.SetActive(false);
        Windows4.SetActive(false);
        Windows5.SetActive(false);
        Windows6.SetActive(false);
        Windows7.SetActive(false);
        Windows8.SetActive(true);
        Windows9.SetActive(false);
        Windows10.SetActive(false);
        Windows11.SetActive(false);
        Windows12.SetActive(false);
    }
    public void OnClickButtonWindow9()
    {
        Windows1.SetActive(false);
        Windows2.SetActive(false);
        Windows3.SetActive(false);
        Windows4.SetActive(false);
        Windows5.SetActive(false);
        Windows6.SetActive(false);
        Windows7.SetActive(false);
        Windows8.SetActive(false);
        Windows9.SetActive(true);
        Windows10.SetActive(false);
        Windows11.SetActive(false);
        Windows12.SetActive(false);
    }
    public void OnClickButtonWindow10()
    {
        Windows1.SetActive(false);
        Windows2.SetActive(false);
        Windows3.SetActive(false);
        Windows4.SetActive(false);
        Windows5.SetActive(false);
        Windows6.SetActive(false);
        Windows7.SetActive(false);
        Windows8.SetActive(false);
        Windows9.SetActive(false);
        Windows10.SetActive(true);
        Windows11.SetActive(false);
        Windows12.SetActive(false);
    }
    public void OnClickButtonWindow11()
    {
        Windows1.SetActive(false);
        Windows2.SetActive(false);
        Windows3.SetActive(false);
        Windows4.SetActive(false);
        Windows5.SetActive(false);
        Windows6.SetActive(false);
        Windows7.SetActive(false);
        Windows8.SetActive(false);
        Windows9.SetActive(false);
        Windows10.SetActive(false);
        Windows11.SetActive(true);
        Windows12.SetActive(false);
    }
    public void OnClickButtonWindow12()
    {
        Windows1.SetActive(false);
        Windows2.SetActive(false);
        Windows3.SetActive(false);
        Windows4.SetActive(false);
        Windows5.SetActive(false);
        Windows6.SetActive(false);
        Windows7.SetActive(false);
        Windows8.SetActive(false);
        Windows9.SetActive(false);
        Windows10.SetActive(false);
        Windows11.SetActive(false);
        Windows12.SetActive(true);
    }

    public void OnClickStopEngine()
    {
        BlackScreen.SetActive(true);
        FirstPersonController.SetPlayerState(1);
    }

    public void OnClickBlackScreen()
    {
        BlackScreen.SetActive(false);


    }




}
