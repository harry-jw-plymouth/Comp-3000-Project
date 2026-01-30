using NUnit.Framework;
using UnityEditor.Build;
using UnityEngine;
using System.Collections.Generic;

public class RatingInfo
{
    float Rating;
    List<string> ReportList = new List<string>();
    float HomeLessPercentage = 100;
    int ShopRating = 0;
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
    public void SetShopRating(int NumberOfNPCs,int NumberOfShops)
    {
        if (NumberOfShops == 0)
        {
            //No shops
            AddReport("No shops, build one as soon as possible");
            ShopRating = 0;
        }
        else if ((float)(NumberOfNPCs / 40) >= NumberOfShops && (float)(NumberOfNPCs / 30) < NumberOfShops)
        {
            //one shop per 40-30 people
            ShopRating = 30;
            AddReport("More shops needed soon");
        }
        else if ((float)(NumberOfNPCs / 30) >= NumberOfShops && (float)(NumberOfNPCs / 20) < NumberOfShops)
        {
            //one shop per 30-20 people
            ShopRating = 60;
            AddReport("More shops needed soon");
        }
        else if ((float)(NumberOfNPCs / 20) >= NumberOfShops && (float)(NumberOfNPCs / 10) < NumberOfShops)
        {
            //one shop per 30-20 people
            ShopRating = 60;
            AddReport("A good amount of shops");
        }
        else {
            //one shop per 20 people or more
            ShopRating = 100;
            AddReport("A good amount of shops");
        }
    }
    public void CalulcateRating()
    {
        Rating=((int)(100-HomeLessPercentage)+ShopRating)/2;
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
