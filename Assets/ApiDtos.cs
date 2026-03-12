using System;

[Serializable]
public class RegisterRequest 
{
    public string username;
    public string password;
}

[Serializable]
public class LoginRequest
{
    public string username;
    public string password;
}

[Serializable]
public class LoginResponse
{
    public bool ok;
    public string token;
    public string error;
}

[Serializable]
public class CreateHighScoreRequest
{
    public string screenname;
    public string firstname;
    public string lastname;
    public string date;
    public int score;
    public int wins;
}

[Serializable]
public class HghScoreDto
{
    public string _id;
    public string userId;
    public string screenname;
    public string firstname;
    public string lastname;
    public string date;
    public int score;
    public int wins;
    public string createdAt;
    public string updatedAt;
}
