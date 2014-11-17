using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UserService
{

    public readonly static UserService Instance = new UserService();

    public void increaseBelly(OPUser user,int belly)
    {
        user.Belly -= belly;
        OPUserDAO.Instance.save(user);
    }

    public void decreaseBelly(OPUser user,int belly)
    {
        user.Belly += belly;
        OPUserDAO.Instance.save(user);   
    }

    public void increaseExp(OPUser user,int exp)
    {
        user.Exp += exp;
        OPUserDAO.Instance.save(user);
    }

    public bool isLevelUp()
    {
        return false;
    }

    public void updateScore(OPUser user,int score)
    {
        user.Score = score;
        if(isHighScore(user, score))
            user.HighScore = score;
        OPUserDAO.Instance.save(user);
    }

    public bool isHighScore(OPUser user,int score)
    {
        return (score > user.HighScore);
    }
}