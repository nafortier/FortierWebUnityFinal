using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using System;


public class ApiUIController : MonoBehaviour
{
    [Header("Api")]
    [SerializeField]
    APIClient api;

    [Header("Public Data Api")]
    [SerializeField]
    TMP_Text txtApiData;

    [Header("Authorization Panel")]
    [SerializeField]
    TMP_InputField inputUsername;
    [SerializeField]
    TMP_InputField inputPassword;
    [SerializeField]
    TMP_Text txtAuthStatus;

    [Header("High Score Panel")]
    [SerializeField]
    GameObject highScorePanel;
    [SerializeField]
    TMP_Text txtScoreList;
    [SerializeField]
    TMP_InputField inputScreenName;
    [SerializeField]
    TMP_InputField inputFirstName;
    [SerializeField]
    TMP_InputField inputLastName;
    [SerializeField]
    TMP_InputField inputDate;
    [SerializeField]
    TMP_InputField inputScore;

    [Header("SearchPanel")]
    [SerializeField]
    TMP_Text txtSearchList;
    [SerializeField]
    TMP_InputField inputScreenNameSearch;

    [SerializeField]
    TMP_Text txtUsersList;


    [SerializeField]
    TMP_InputField searchEditInput;
    [SerializeField]
    GameObject editSearchPanel;
    [SerializeField]
    GameObject editPanel;

    [SerializeField]
    TMP_InputField editScreenName;
    [SerializeField]
    TMP_InputField editFirstName;
    [SerializeField]
    TMP_InputField editLastName;
    [SerializeField]
    TMP_InputField editDate;
    [SerializeField]
    TMP_InputField editScore;


    [SerializeField]
    TMP_InputField deleteSearchInput;

    [SerializeField]
    TMP_Text deleteConfirmText;

    [Header("Error Status")]
    [SerializeField]
    TMP_Text txtErrorStatus;

    [SerializeField]
    GameObject loginPanel;
    [SerializeField]
    GameObject menuPanel;


    private string currentEditingMongoId;
    private string idToDelete;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        api.ClearToken();
        //Check to see if Token is available
        highScorePanel.SetActive(!string.IsNullOrWhiteSpace(api.Token));
        txtErrorStatus.text = string.IsNullOrWhiteSpace(api.Token) ? "Not logged in" : "Token loaded Logged in";

    }

    //Public Data Api
    public void OnClickLoadApiData()
    {
        SetStatus("Loading Api: api/data...");

        StartCoroutine(api.Get("/api/data", 
            onSuccess: (json) =>
            {
                txtApiData.text = json;
                SetStatus("Loaded /api/data");
            },
            onError: (err) =>
            {
                SetStatus(err, isError: true);
            }
            ));
    }

    //Add login function
    public void OnClickLogin()
    {
        var username = inputUsername.text.Trim();
        var password = inputPassword.text;

        //Some velidation
        if (username.Length < 3 || password.Length <3)
        {
            txtAuthStatus.text = "Username or Password inncorrect";
            return;
        }

        txtAuthStatus.text = "Logging in...";
        SetStatus("POST /api/auth/login...");

        StartCoroutine(api.PostJson("/api/auth/login", new LoginRequest {username = username, password = password },
                onSuccess: (json) =>
                {
                    //Getting token
                    var res = JsonUtility.FromJson<LoginResponse>(json);

                    if (res != null && res.ok && !string.IsNullOrWhiteSpace(res.token))
                    {
                        api.SetToken(res.token);
                        txtAuthStatus.text = "Logged in";
                        //highScorePanel.SetActive(true);
                        menuPanel.SetActive(true);
                        loginPanel.SetActive(false);
                        SetStatus("Login Success");
                        //Refresh Scores Later

                        OnClickFetchScores();
                    }
                    else
                    {
                        txtAuthStatus.text = "Login Failed";
                        
                        SetStatus("Login failed", isError:true);
                    }
                },
                onError: (err) =>
                {
                    txtAuthStatus.text = "Login Failed";

                    SetStatus(err, isError: true);

                },
                auth:false
            ));
    }

    //Register
    public void OnClickRegister()
    {
        var username = inputUsername.text.Trim();
        var password = inputPassword.text;

        //Some velidation
        if (username.Length < 3 || password.Length < 3)
        {
            txtAuthStatus.text = "Username or Password inncorrect";
            return;
        }

        txtAuthStatus.text = "Registering...";
        SetStatus("POST /api/auth/register...");

        StartCoroutine(api.PostJson("/api/auth/register", new RegisterRequest { username = username, password = password },
                onSuccess: (json) =>
                {

                    txtAuthStatus.text = "Registered User";
                    SetStatus("Registered Success");





               
                },
                onError: (err) =>
                {
                    txtAuthStatus.text = "Register Failed";

                    SetStatus(err, isError: true);

                },
                auth: false
            ));
    }

    //Logout

    public void OnClickLogout()
    {
        api.ClearToken();
        highScorePanel.SetActive(false);
        txtAuthStatus.text = "Logged out";
        txtScoreList.text = "";
        inputScreenName.text = "";
        inputFirstName.text = "";
        inputLastName.text = "";
        inputDate.text = "";
        inputScore.text = "";
        
        SetStatus("Cleared token");
    }

    //fetch scores

    public void OnClickFetchScores()
    {
        if (string.IsNullOrWhiteSpace(api.Token))
        {
            SetStatus("Not Logged In", true);
            return;
        }

        SetStatus("Get /api/highscores...");

        StartCoroutine(api.Get("/api/highscores",
                onSuccess: (json) =>
                {
                    var scores = JsonArrayWrapper.FromJsonArray<HighScoreDto>(json);
                    txtScoreList.text = FormatScores(scores);
                    SetStatus($"Loaded Scores {scores.Length} scores");


                },
                onError: (err) =>
                {

                    SetStatus(err, isError: true);

                },
                auth: true
            ));
    }

    public void OnClickSubmitScore()
    {
        if (string.IsNullOrWhiteSpace(api.Token))
        {
            SetStatus("Not Logged In", true);
            return;
        }

        var player = inputScreenName.text;

        if (string.IsNullOrEmpty(player))
        {
            player = inputUsername.text;

        }
        var firstname = inputFirstName.text;
        var lastname = inputLastName.text;
        var date = inputDate.text;
      

        if(!int.TryParse(inputScore.text, out var score))
        {
            SetStatus("Score must be an integer", true);
        }
        
        SetStatus("POST /api/highscores...");


    

        StartCoroutine(api.PostJson("/api/highscores", new CreateHighScoreRequest { screenname=player, firstname=firstname, lastname=lastname, date=date,score = score},
                onSuccess: (json) =>
                {

                    SetStatus("Score Submitted");
                    OnClickFetchScores();

                    inputScreenName.text = "";
                    inputScreenName.text = "";
                    inputFirstName.text = "";
                    inputLastName.text = "";
                    inputDate.text = "";
                    inputScore.text = "";
                 


                },
                onError: (err) =>
                {

                    SetStatus(err, isError: true);

                },
                auth: true
            ));
    }


    public void SearchForScreenName()
    {
        string targetName = inputScreenNameSearch.text.Trim();

        if (string.IsNullOrEmpty(targetName))
        {
            SetStatus("Please enter a name.", isError: true);
            return;
        }

        StartCoroutine(api.Get("/api/highscores",
            onSuccess: (json) =>
            {
                var scores = JsonArrayWrapper.FromJsonArray<HighScoreDto>(json);

           
            HighScoreDto foundUser = null;

            foreach (var record in scores)
            {
                if (record.screenname.ToLower() == targetName.ToLower())
                {
                    foundUser = record;
                    break; 
                }
            }

            
            if (foundUser != null)
                {
                    HighScoreDto[] scoresarr = new HighScoreDto[1];
                    scoresarr[0] = foundUser;
                    txtSearchList.text = FormatScores(scoresarr);
                }
                else
                {
                    txtSearchList.text = ""; // Clear old text
                SetStatus("The user doesn't exist.", isError: true);
                }
            },
            onError: (err) =>
            {
                SetStatus($"Error: {err}", isError: true);
            },
            auth: true
        ));
    }
    
    public void DisplayAllUsersSorted()
    {
        SetStatus("Fetching all users...");

        StartCoroutine(api.Get("/api/highscores",
            onSuccess: (json) =>
            {
    
            var scores = JsonArrayWrapper.FromJsonArray<HighScoreDto>(json);

                if (scores == null || scores.Length == 0)
                {
                    txtScoreList.text = "Database is empty.";
                    return;
                }

     
            Array.Sort(scores, (a, b) => string.Compare(a.screenname, b.screenname, StringComparison.OrdinalIgnoreCase));

            string fullList = "";
                foreach (var user in scores)
                {
                    fullList += $"{user.screenname} - Score: {user.score}\n";
                }

        
            txtUsersList.text = fullList;
                SetStatus($"Displayed {scores.Length} users.");
            },
            onError: (err) =>
            {
                SetStatus($"Error: {err}", isError: true);
            },
            auth: true
        ));
    }


    
    public void OnClickSearchToEdit()
    {
        string targetName = searchEditInput.text;

        if (string.IsNullOrEmpty(targetName))
        {
            SetStatus("Enter a screen name to search.", true);
            return;
        }

        SetStatus($"Searching for {targetName}...");

        // Fetch the latest data to ensure we have the correct ID and current values
        StartCoroutine(api.Get("/api/highscores",
            onSuccess: (json) =>
            {
                var scores = JsonArrayWrapper.FromJsonArray<HighScoreDto>(json);
                HighScoreDto foundUser = null;

                editSearchPanel.SetActive(false);
                editPanel.SetActive(true);

            // Search the array for the name
            foreach (var user in scores)
                {
                    if (user.screenname.Equals(targetName, StringComparison.OrdinalIgnoreCase))
                    {
                        foundUser = user;
                        break;
                    }
                }

                if (foundUser != null)
                {
                     LoadUserIntoEditFields(foundUser);
                }
                else
                {
                    SetStatus("User not found in database.", true);
                }
            },
            onError: (err) => SetStatus($"Search Failed: {err}", true),
            auth: true
        ));
    }
    
    // 2. Helper to populate the edit boxes
    private void LoadUserIntoEditFields(HighScoreDto user)
    {
        currentEditingMongoId = user._id; 

        // Fill your existing input fields
        editScreenName.text = user.screenname;
        editFirstName.text = user.firstname;
        editLastName.text = user.lastname;
        editDate.text = user.date;
        editScore.text = user.score.ToString();

        // Toggle Panels
        

        SetStatus($"Editing User ID: {currentEditingMongoId}");
    }
    
    // 3. The actual Save/Update function (Matches your router.put("/:id"))
    public void OnClickSaveEditedUser()
    {
        if (string.IsNullOrEmpty(currentEditingMongoId)) return;

        HighScoreDto updatedData = new HighScoreDto
        {
            screenname = editScreenName.text,
            firstname = editFirstName.text,
            lastname = editLastName.text,
            date = editDate.text,
            score = int.Parse(editScore.text)
        };

        StartCoroutine(api.PutJson($"/api/highscores/{currentEditingMongoId}", updatedData,
            onSuccess: (json) => {
                SetStatus("Update Successful!");
               // editPanel.SetActive(false);
               // listPanel.SetActive(true);
                OnClickFetchScores(); // Refresh the list
        },
            onError: (err) => SetStatus("Update Failed", true),
            auth: true
        ));
    }

  


 
    public void OnClickSearchForDelete()
    {
        
        string targetName = deleteSearchInput.text;

        if (string.IsNullOrEmpty(targetName))
        {
            SetStatus("Enter a name to delete.", true);
            return;
        }

        SetStatus($"Searching for {targetName} to delete...");

        StartCoroutine(api.Get("/api/highscores",
            onSuccess: (json) =>
            {
                var scores = JsonArrayWrapper.FromJsonArray<HighScoreDto>(json);
                HighScoreDto foundUser = null;

                foreach (var user in scores)
                {
                    if (user.screenname.Equals(targetName, StringComparison.OrdinalIgnoreCase))
                    {
                        foundUser = user;
                        break;
                    }
                }

                if (foundUser != null)
                {
                    idToDelete = foundUser._id;
                    deleteConfirmText.text = $"Deleting {foundUser.screenname}";

                    ExecuteDelete();
                }
                else
                {
                    SetStatus("User not found.", true);
                }
            },
            onError: (err) => SetStatus($"Search Error: {err}", true),
            auth: true
        ));


        
    }


    public void ExecuteDelete()
    {
        if (string.IsNullOrEmpty(idToDelete))
        {
            SetStatus("No user selected to delete.", true);
            return;
        }

        SetStatus("Executing Delete...");

        StartCoroutine(api.DeleteRequest($"/api/highscores/{idToDelete}",
            onSuccess: (json) => {
                SetStatus("Entry deleted successfully.");
                deleteSearchInput.text = "";
                idToDelete = null;
                OnClickFetchScores();
            },
            onError: (err) => SetStatus($"Delete Failed: {err}", true),
            auth: true // This MUST stay true because your API checks ownership
        ));
    }


    void SetStatus(string message, bool isError = false)
    {
        if (txtErrorStatus == null) return;

        txtErrorStatus.text = (isError ? "Error: ": "") + message;

        Debug.Log(txtErrorStatus.text);
    }

    static string FormatScores(HighScoreDto[] scores) 
    {
        if(scores == null || scores.Length == 0) return "(no scores yet)";

        var str = "";


        for (int i = 0; i < scores.Length; i++)
        {
            var row = scores[i];
            str += $"{i + 1}. {row.screenname} First Name:{row.firstname} Last Name:{row.firstname} Date:{row.date} Score:{row.score}\n";
        }


        return str;



    }
}
