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
    int HospitalRating = 0;
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
    public void SetHospitalRating(int NumberOfNPCs, int NumberOfHospitals)
    {
        if (NumberOfHospitals == 0)
        {
            //No hospitals
            HospitalRating = 0;
            AddReport("No hospitals, build one as soon as possible");
        }
        else if ((float)(NumberOfNPCs / 200) >= NumberOfHospitals && (float)(NumberOfNPCs / 150) < NumberOfHospitals)
        {
            //one shop per 40-30 people
            HospitalRating = 30;
            AddReport("Another hospital needed very soon");
        }
        else if ((float)(NumberOfNPCs / 150) >= NumberOfHospitals && (float)(NumberOfNPCs / 120) < NumberOfHospitals)
        {
            //one hospital per 150-120 people
            HospitalRating = 60;
            AddReport("More hospitals needed somewhat soon");
        }
        else if ((float)(NumberOfNPCs /120) >= NumberOfHospitals && (float)(NumberOfNPCs / 100) < NumberOfHospitals)
        {
            //one hospital per 120-100 people
            HospitalRating = 60;
            AddReport("A good amount of hospitals");
        }
        else
        {
            //one hospital per 100 people or more
            HospitalRating = 100;
            AddReport("A very good amount of hospitals");
        }
        Debug.Log("Hospital rating:" + HospitalRating);
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
        Rating=((int)(100-HomeLessPercentage)+ShopRating+HospitalRating)/3;
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
