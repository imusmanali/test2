using UnityEngine;
using System.Collections;
using Facebook.Unity;
using System;
using System.Collections.Generic;
using UnityEngine.UI;

public class FBscript : MonoBehaviour
{
    //menu variable
    [SerializeField]
    GameObject DialogLoggedIn;
    [SerializeField]
    GameObject DialogLoggedOut;
    [SerializeField]
    GameObject DialoguserName;
    [SerializeField]
    GameObject DialogProfilePic;

    //Scores
    [SerializeField]
    GameObject DialogDebugScore;
    [SerializeField]
    GameObject ScoreEntryPanal;
    [SerializeField]
    GameObject ScoreScrollList;
    void Awake()
    {
        FaceBookManager.Instance.InitFB();
        DealWithMenus(FB.IsLoggedIn);
    }



    public void FBlogin()
    {
        List<string> Readpermissions = new List<string>();
        List<string> Writepermissions = new List<string>();
        Readpermissions.Add("public_profile");
        Writepermissions.Add("publish_actions");
        FB.LogInWithReadPermissions(Readpermissions, AuthCallBack);
        FB.LogInWithPublishPermissions(Writepermissions, AuthCallBack);
    }

    private void AuthCallBack(IResult result)
    {
        if (result.Error != null)
        {
            Debug.Log(result.Error);
        }
        else
        {
            if (FB.IsLoggedIn)
            {
                FaceBookManager.Instance.IsloggedIn = true;
                FaceBookManager.Instance.GetProfile();
                Debug.Log("fb is logged in");
            }
            else
            {
                Debug.Log("fb is not logged in");
            }
            DealWithMenus(FB.IsLoggedIn);
        }
    }

    void DealWithMenus(bool isloggedIn)
    {
        if (isloggedIn)
        {
            DialogLoggedIn.SetActive(true);
            DialogLoggedOut.SetActive(false);

            if (FaceBookManager.Instance.ProfilePic != null)
            {

                Image ProfilePic = DialogProfilePic.GetComponent<Image>();
                ProfilePic.sprite = FaceBookManager.Instance.ProfilePic;
            }
            else
            {
                StartCoroutine("WaitForProfilePic");
            }

            if (FaceBookManager.Instance.ProfileName != null)
            {
                Text userName = DialoguserName.GetComponent<Text>();
                userName.text = "welcome " + FaceBookManager.Instance.ProfileName;
            }
            else
            {
                StartCoroutine("WaitForProfileName");
            }



        }
        else
        {
            DialogLoggedIn.SetActive(false);
            DialogLoggedOut.SetActive(true);
        }
    }

    IEnumerator WaitForProfileName()
    {
        while (FaceBookManager.Instance.ProfileName == null)
        {
            yield return null;
        }
        DealWithMenus(FB.IsLoggedIn);
    }
    IEnumerator WaitForProfilePic()
    {
        while (FaceBookManager.Instance.ProfilePic == null)
        {
            yield return null;
        }
        DealWithMenus(FB.IsLoggedIn);
    }

    public void OnShare()
    {
        FaceBookManager.Instance.Share();
    }
    public void OnInvite()
    {
        FaceBookManager.Instance.Invite();
    }
    public void OnShareWithUsers()
    {
        FaceBookManager.Instance.ShareWithUsers();
    }

    //Scores
    public void OnQueryScroes()
    {
        FaceBookManager.Instance.QueryScroes(ScoreEntryPanal, ScoreScrollList);

    }


    public void OnSetScroes()
    {
        FaceBookManager.Instance.SetScroes();

        if (FaceBookManager.Instance.SetScoreResult != null)
        {

            Debug.Log(FaceBookManager.Instance.SetScoreResult);
        }
        else
        {
            StartCoroutine("WaitForScoreToSet");
        }

    }
    IEnumerator WaitForScoreToSet()
    {
        while (FaceBookManager.Instance.SetScoreResult == null)
        {
            yield return null;
        }
        OnSetScroes();
    }
}
