using UnityEngine;
using System.Collections;
using Facebook.Unity;
using System;
using System.Collections.Generic;
using UnityEngine.UI;


public class friends
{
    public string UserName { get; set; }
    public long? Score { get; set; }
    public string UserId { get; set; }
    public Sprite UserAvatar { get; set; }
}

public class FaceBookManager : MonoBehaviour
{

    private static FaceBookManager _instance;
    public static FaceBookManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject fbm = new GameObject("FBManager");
                fbm.AddComponent<FaceBookManager>();
            }
            return _instance;

        }

    }
    
    public bool IsloggedIn { get; set; }
    public string ProfileName { get; set; }
    public Sprite ProfilePic { get; set; }
    public string AppLinkUrl { get; set; }


    //score
    private List<object> ScoresList = null;
    public string SetScoreResult { get; set; }
    public string userId { get; set; }
    public List<friends> FriendsList = new List<friends>();
    GameObject ThisScoreEntryPanal;
    GameObject ThisScoreScrollList;

    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        _instance = this;
        IsloggedIn = true;
    }

    public void InitFB()
    {
         if (!FB.IsInitialized)
        {
            FB.Init(SetInit, OnHideUnity);
        }
        else
        {
            IsloggedIn = FB.IsLoggedIn;
        }
    }
    private void SetInit()
    {
        if (FB.IsLoggedIn)
        {
            Debug.Log("fb is logged in");
            GetProfile();
        }
        else
        {
            Debug.Log("fb is not logged in");
        }
        IsloggedIn = FB.IsLoggedIn;
    }
    private void OnHideUnity(bool isUnityShown)
    {
        if (!isUnityShown)
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
        }
    }

    public void GetProfile()
    {
        FB.API("/me?fields=first_name", HttpMethod.GET, DisplayUsername);
        FB.API("/me/picture?type=square&hight=100&width=100", HttpMethod.GET, Displayprofilpic);
        FB.GetAppLink(DealWithAppLink);
    }

   

    private void Displayprofilpic(IGraphResult result)
    {
        if (result.Texture != null)
        {

            ProfilePic = Sprite.Create(result.Texture, new Rect(0, 0, 100, 100), new Vector2());
        }
        else
        {
            Debug.Log(result.Error);
        }
    }

    private void DisplayUsername(IResult result)
    {
        if (result.Error == null)
        {
           ProfileName  =  ""+result.ResultDictionary["first_name"];
        }
        else
        {
            Debug.Log(result.Error);
        }
    }
    private void DealWithAppLink(IAppLinkResult result)
    {
        if (!string.IsNullOrEmpty(result.Url))
        {
            AppLinkUrl = "" + result.Url + "";
            Debug.Log(AppLinkUrl);
        }
        else
        {
            AppLinkUrl = "https://google.com";
        }
    }

    public void Share()
    {
        FB.FeedShare(
            string.Empty,
            new Uri(AppLinkUrl),
            "Hello this is title",
            "This is caption",
            "Check out this game",
            new Uri("https://i.ytimg.com/vi/NtgtMQwr3Ko/maxresdefault.jpg"),
            string.Empty,
            ShareCallBack
            );
    }

    private void ShareCallBack(IShareResult result)
    {
        Debug.Log(result.RawResult);
        if (result.Cancelled)
        {
            Debug.Log("Share is Cancelled");

        }
        else if (!string.IsNullOrEmpty(result.Error))
        {
            Debug.Log("Error on Share");

        }
        else if (!string.IsNullOrEmpty(result.RawResult))
        {
            Debug.Log("Success On Share");
        }

    }

    public void Invite()
    {
        FB.Mobile.AppInvite(
            new Uri(AppLinkUrl),
            new Uri("https://i.ytimg.com/vi/NtgtMQwr3Ko/maxresdefault.jpg"),
            InviteCallBack
            );
    }

    private void InviteCallBack(IAppInviteResult result)
    {
        Debug.Log(result.RawResult);
        if (result.Cancelled)
        {
            Debug.Log("Invite is Cancelled");

        }
        else if (!string.IsNullOrEmpty(result.Error))
        {
            Debug.Log("Error on Invite");

        }
        else if (!string.IsNullOrEmpty(result.RawResult))
        {
            Debug.Log("Success On Invite");
        }
    }

    public void ShareWithUsers()
    {
        FB.AppRequest(
            "come and join me, i bet you can't beat my score!",
            null,
            new List<object>() { "app_users" },
            null,
            null,
            null,
            null,
            ShareWithUsersCallBack
            );
    }

    private void ShareWithUsersCallBack(IAppRequestResult result)
    {
        Debug.Log(result.RawResult);
        if (result.Cancelled)
        {
            Debug.Log("Invite is Cancelled");

        }
        else if (!string.IsNullOrEmpty(result.Error))
        {
            Debug.Log("Error on Invite");

        }
        else if (!string.IsNullOrEmpty(result.RawResult))
        {
            Debug.Log("Success On Invite");
        }
    }

    //All Score API Related Things
    public void QueryScroes(GameObject ScoreEntryPanal, GameObject ScoreScrollList)
    {
        ThisScoreEntryPanal = ScoreEntryPanal;
        ThisScoreScrollList = ScoreScrollList;
        FB.API("/app/scores?fields=score,user.limit(30)", HttpMethod.GET, QueryScroesCallBack);
        Debug.Log("about to print list");
    }

    private void QueryScroesCallBack(IResult result)
    {

        Debug.Log("json" + result.RawResult);
        Debug.Log("data" + result.ResultDictionary["data"]);
        var datalist = result.ResultDictionary["data"] as List<object>;

        foreach (Transform child in ThisScoreScrollList.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        foreach (var datalistEntry in datalist)
        {
            var dataDict = datalistEntry as Dictionary<string, object>;
            long score = (long)dataDict["score"];
            var user = dataDict["user"] as Dictionary<string, object>;
            string userName = user["name"] as string;
            string userId = user["id"] as string;
            Debug.Log("test USER ID" + userId);


            GameObject ScorePanal;
            ScorePanal = Instantiate(ThisScoreEntryPanal) as GameObject;
            ScorePanal.transform.parent = ThisScoreScrollList.transform;
            ScorePanal.SetActive(true);

            Transform ThisScoreName = ScorePanal.transform.Find("FriendName");
            Transform ThisScoreScore = ScorePanal.transform.Find("FriendScore");
            Text ScoreName = ThisScoreName.GetComponent<Text>();
            Text ScoreScore = ThisScoreScore.GetComponent<Text>();

            ScoreName.text = user["name"] as string;
            ScoreScore.text = score.ToString();

            //picture
            Transform thisUserAvatar = ScorePanal.transform.Find("FriendAvatar");
            Image UserAvatar = thisUserAvatar.GetComponent<Image>();


            string query = "/" + userId + "/picture?type=square&hight=128&width=128";
            FB.API(query, HttpMethod.GET, delegate (IGraphResult pictureResult)
            {
                if (pictureResult.Error != null)
                {
                    Debug.Log(pictureResult.Error);
                }
                else
                {
                    UserAvatar.sprite = Sprite.Create(pictureResult.Texture, new Rect(0, 0, 128, 128), new Vector2());
                }

            });



        }


    }

    public void SetScroes()
    {

        var ScoreData = new Dictionary<string, string>();
        ScoreData["score"] = UnityEngine.Random.Range(10, 200).ToString();
        FB.API("/me/scores", HttpMethod.POST, delegate (IGraphResult result) { SetScoreResult = "score submit result" + result.RawResult; }, ScoreData);
    }
}
