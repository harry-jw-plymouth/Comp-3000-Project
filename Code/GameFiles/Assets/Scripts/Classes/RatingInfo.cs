using UnityEditor.Build;
using UnityEngine;

public class RatingInfo
{
    float Rating;
    float HomeLessPercentage = 100;
    public void SetHomeLessPercentage(float New){
        HomeLessPercentage = New;
    }
    public float GetHomeLessPercentage() { 
        return HomeLessPercentage;
    }
    public void CalulcateRating()
    {

        Rating=(int)(100-HomeLessPercentage);
    }
    public float GetRating() { 
        return Rating;
    }
}
