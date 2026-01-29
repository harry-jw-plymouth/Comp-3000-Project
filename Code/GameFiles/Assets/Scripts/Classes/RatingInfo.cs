using NUnit.Framework;
using UnityEditor.Build;
using UnityEngine;
using System.Collections.Generic;

public class RatingInfo
{
    float Rating;
    List<string> ReportList = new List<string>();
    float HomeLessPercentage = 100;
    public void SetHomeLessPercentage(float New){
        HomeLessPercentage = New;
        if(HomeLessPercentage > 60)
        {
            AddReport("More housing needed desperately");
        }
        else if(HomeLessPercentage<=60 && HomeLessPercentage > 40)
        {
            AddReport("Housing needed very soon");
        }
        else if(HomeLessPercentage<=40 && HomeLessPercentage > 5)
        {
            AddReport("More houses needed but not too bad");
        }
        else
        {
            AddReport("Housing Situation Stable");
        }
    }
    public float GetHomeLessPercentage() { 
        return HomeLessPercentage;
    }
    public void CalulcateRating()
    {

        Rating=(int)(100-HomeLessPercentage);
    }
    public void AddReport(string Report) {  
        ReportList.Add(Report); 
    }
    public List<string> GetReport()
    {
        return ReportList;
    }
    public void ClearReports()
    {
        ReportList.Clear();
    }
    public float GetRating() { 
        return Rating;
    }
}
