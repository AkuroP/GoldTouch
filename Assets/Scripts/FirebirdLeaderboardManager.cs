using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Database;
using Firebase.Analytics;
using TMPro;

public class FirebirdLeaderboardManager : MonoBehaviour
{

    public GameObject usernamePanel,userProfilePanel,leaderboardPanel,leaderboardContent,userDataPrefab;

    public TMP_Text profileUsernameTxt, profileUserScoreTxt, errorUsernameTxt;

    public TMP_InputField usernameInput;

    public int highscore,totalUsers = 0;

    
    public string username = "";

    private DatabaseReference db;



    // Start is called before the first frame update
    void Start()
    {
        FirebaseInitialize();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ShowLeaderboard()
    {
        StartCoroutine(FetchLeaderBoardData()); 
        leaderboardPanel.SetActive(true);
        userProfilePanel.SetActive(false);
        
    }

    public void SignInWithUsername() 
    { 

        
        StartCoroutine(CheckUserExistInDatabase());
    

    }


    public void CloseLeaderboard()
    {
        if(leaderboardContent.transform.childCount>0)
        {
            for (int i = 0; i < leaderboardContent.transform.childCount; i++)
            {
                Destroy(leaderboardContent.transform.GetChild(i).gameObject);
            }
        }

        leaderboardPanel.SetActive(false);
        userProfilePanel.SetActive(true);
    }
    public void SignOut()
    {
        PlayerPrefs.DeleteKey("PlayerID");
        PlayerPrefs.DeleteKey("Username");
        PlayerPrefs.DeleteKey("Highscore");
        usernameInput.text = " ";
        profileUsernameTxt.text = " ";
        profileUserScoreTxt.text = " ";
        highscore = 0;
        username = " ";
        usernamePanel.SetActive(true);
        userProfilePanel.SetActive(false);
        

    }

    void FirebaseInitialize()
    {
        db = FirebaseDatabase.DefaultInstance.GetReference("/leaderboard/");

        // Need to create Firebase Child Added Function wich check if new user highscore added or not

        db.ChildAdded += HandleChildAdded;

        // Now Fetch Total Users Count

        GetTotalUser();

        //Check if Player Already login Then  Show user profile Otherwise show username signin

        StartCoroutine(FetchUserProfileData(PlayerPrefs.GetInt("PlayerID")));
        StartCoroutine(CheckAndPushHighScore());  


    }

    void HandleChildAdded(object sender, ChildChangedEventArgs args)
    {
        if(args.DatabaseError != null) {return;}

        //if new child added then we need to fetch Total Users Numbers in Database

        GetTotalUser();
    }

    void GetTotalUser()
    {
        //Get Total Users  From firebase Datatabase

        db.ValueChanged+=(object sender2,ValueChangedEventArgs e2)=>
        {
            if (e2.DatabaseError!=null)
            {
                Debug.LogError(e2.DatabaseError.Message);
                return;
            }

            totalUsers=int.Parse(e2.Snapshot.ChildrenCount.ToString());

            Debug.LogError("Total Users:"+ totalUsers);
        };
    }


    IEnumerator CheckUserExistInDatabase()
    {

        var task = db.OrderByChild("username").EqualTo(usernameInput.text).GetValueAsync();
        yield return new WaitUntil(() => task.IsCompleted);

        if(task.IsFaulted)
        {
            Debug.LogError("Invalid Error");
            errorUsernameTxt.text = "Invalid Error";

        }
        else if (task.IsCompleted)
        {
            DataSnapshot snapshot = task.Result;

            if (snapshot != null && snapshot.HasChildren)
            {
                Debug.LogError("Username Exist");
                errorUsernameTxt.text = "Username Already Exist";
            }

            if (string.IsNullOrEmpty(usernameInput.text))
            {
                Debug.LogError("Username can't be Blank");
                errorUsernameTxt.text = "Username can't be Blank";
                yield break;
            }

            else 
            {
                Debug.LogError("Username Not Exist");
                
                //Push New User Data 
                //Set PlayerPrefs User ID and Username for Login Purpose
                //Show UserProfile

                PushUserData(); 
                PlayerPrefs.SetInt("PlayerID",totalUsers+1);
                PlayerPrefs.SetString("Username",usernameInput.text);

                Firebase.Analytics.FirebaseAnalytics.LogEvent(Firebase.Analytics.FirebaseAnalytics.EventSignUp);

                StartCoroutine(DelayFetchProfile());

                
            }
        }
    }

IEnumerator DelayFetchProfile(){
    yield return new WaitForSeconds(1f);
    StartCoroutine(FetchUserProfileData(totalUsers));
}

void PushUserData()
{
    db.Child("User_"+(totalUsers+1).ToString()).Child("username").SetValueAsync(usernameInput.text);
    db.Child("User_"+(totalUsers+1).ToString()).Child("highscore").SetValueAsync(PlayerPrefs.GetInt("Highscore"));
    Debug.LogError(PlayerPrefs.GetInt("Highscore"));

}

IEnumerator FetchUserProfileData(int playerID)
    {
        Debug.LogError(playerID);
        if (playerID!=0){

            var task = db.Child("User_"+playerID.ToString()).GetValueAsync();
            yield return new WaitUntil(() => task.IsCompleted);

            if (task.IsFaulted)
            {
                Debug.LogError("Invalid Error");
            }

            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;

                if (snapshot != null && snapshot.HasChildren)
                {
                    username = snapshot.Child("username").Value.ToString();
                    highscore = int.Parse(snapshot.Child("highscore").Value.ToString());
                    profileUsernameTxt.text = username;
                    profileUserScoreTxt.text = ""+ PlayerPrefs.GetInt("Highscore");
                    userProfilePanel.SetActive(true);
                    usernamePanel.SetActive(false);
                }

                else
                {
                    Debug.LogError("User ID Not Exist");
                }
            }
        }
    }    

 IEnumerator FetchLeaderBoardData()
    {
        var task = db.OrderByChild("highscore").LimitToLast(10).GetValueAsync();
        yield return new WaitUntil(() => task.IsCompleted);

        if (task.IsFaulted)
        {
            Debug.LogError("Invalid Error");
        }

        else if (task.IsCompleted)
        {
            Debug.LogError("Show Leaderboard");
            DataSnapshot snapshot = task.Result;

            List<LeaderboardData> listLeaderboardEntry = new List<LeaderboardData>();

            foreach(DataSnapshot childSnapshot in snapshot.Children)
            {
                string username2 = childSnapshot.Child("username").Value.ToString();
                int highscore = int.Parse(childSnapshot.Child("highscore").Value.ToString());

                listLeaderboardEntry.Add(new LeaderboardData(username2,highscore));
            }

            DisplayLeaderboardData(listLeaderboardEntry);
        }

    }   

    void DisplayLeaderboardData(List<LeaderboardData> leaderboardData) 
    { 
        int rankCount = 0;
        Debug.LogError(leaderboardData.Count);

        for(int i = leaderboardData.Count-1; i>=0; i--)
        {
            rankCount = rankCount+1;
            //Spawn User Leaderboard Data UI

            GameObject obj= Instantiate(userDataPrefab);
            obj.transform.parent=leaderboardContent.transform;
            obj.transform.localScale=Vector3.one;

            obj.GetComponent<UserDataUI>().userRankTxt.text = "Rank"+rankCount;
            obj.GetComponent<UserDataUI>().usrenameTxt.text = ""+leaderboardData[i].username;
            obj.GetComponent<UserDataUI>().userScoreTxt.text = ""+leaderboardData[i].highscore;
        }

        leaderboardPanel.SetActive(true);
        usernamePanel.SetActive(false);
    
    }




    IEnumerator CheckAndPushHighScore()
    {
    var task = db.Child("User_" + (totalUsers + 1).ToString()).GetValueAsync();
    yield return new WaitUntil(() => task.IsCompleted);

    if (task.IsFaulted)
    {
        Debug.LogError("Failed to retrieve high score from database");
        yield break;
    }

    DataSnapshot snapshot = task.Result;

    if (snapshot != null && snapshot.HasChildren)
    {
        int lastHighScore = int.Parse(snapshot.Child("highscore").Value.ToString());

        if (PlayerPrefs.GetInt("Highscore") > lastHighScore)
        {
            db.Child("User_" + (totalUsers + 1).ToString()).Child("highscore").SetValueAsync(PlayerPrefs.GetInt("Highscore"));
        }
    }
}






}




public class LeaderboardData
{

    public string username;
    public int highscore;

    public LeaderboardData(string username,int highscore)
    {

        this.username = username;
        this.highscore = highscore;

    }








}