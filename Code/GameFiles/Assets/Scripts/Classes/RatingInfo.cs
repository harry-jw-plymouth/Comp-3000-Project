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
    int RoadRating = 0;
    int PowerRating = 0;
    int EntertainmentRating = 0;
    int EnviromentRating = 0;
    int PowerReachRating = 0;
    int TrainStationRating = 0;
    int GreeneryRating = 0;
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
    public void SetRoadRating(int NumberOfRoads,int NumberOfBuildings)
    {
       // Debug.Log("Number of roads: " + NumberOfRoads);
       // Debug.Log("Number of buildings:" + NumberOfBuildings);
        float RoadsPerBuilding = (float)NumberOfRoads / NumberOfBuildings;
        if (NumberOfRoads == 0 )
        {
            //No Roads
            RoadRating = 0;
            AddReport("No Roads, build one as soon as possible");
        }
        else if ((RoadsPerBuilding < 1f))
        {
            //less roads than buildings or very few roads built
            RoadRating = 10;
            AddReport("More road needed as soon as possible");
        }
        else if ((RoadsPerBuilding < 1.5f &&RoadsPerBuilding>=1f) ||NumberOfRoads<3)
        {
            //less roads than buildings or very few roads built
            RoadRating = 15;
            AddReport("More road needed as soon as possible");
        }
        else if (RoadsPerBuilding < 1.5f && RoadsPerBuilding >= 2f)
        {
            //1-2 roads oer building
            RoadRating = 25;
            AddReport("More road needed as soon as possible");
        }
        else if (RoadsPerBuilding < 2f && RoadsPerBuilding >= 1.5f)
        {
            //2-4 roads per building
            RoadRating = 50;
            AddReport("More roads needed somewhat soon");
        }
        else if (RoadsPerBuilding < 2.5f && RoadsPerBuilding >= 2f)
        {
            //4-5 roads per building
            RoadRating = 75;
            AddReport("almost enough roads");
        }
        else if (RoadsPerBuilding<3.33f && RoadsPerBuilding>=2.5f)
        {
            //5-7 roads per building
            RoadRating = 75;
            AddReport("A good amount of roads");
        }
        else
        {
            //7 or more roads per building
            RoadRating = 100;
            AddReport("Enough roads");
        }
       // Debug.Log("Value:"+RoadsPerBuilding);
       // Debug.Log("road rating:" + RoadRating);
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
            HospitalRating = 55;
            AddReport("More hospitals needed somewhat soon");
        }
        else if ((float)(NumberOfNPCs /120) >= NumberOfHospitals && (float)(NumberOfNPCs / 100) < NumberOfHospitals)
        {
            //one hospital per 120-100 people
            HospitalRating = 75;
            AddReport("A good amount of hospitals");
        }
        else
        {
            //one hospital per 100 people or more
            HospitalRating = 100;
            AddReport("A very good amount of hospitals");
        }
        //Debug.Log("Hospital rating:" + HospitalRating);
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
    public void SetEntertainmentRating(int NumberOfNPCs,int NumberrOfEntertainment)
    {
        if (NumberrOfEntertainment == 0)
        {
            //No shops
            AddReport("No entertainment, build one as soon as possible");
            EntertainmentRating = 0;
        }
        else if ((float)(NumberOfNPCs / 80) >= NumberrOfEntertainment && (float)(NumberOfNPCs / 60) < NumberrOfEntertainment)
        {
            //one venue per 80-60 people
            ShopRating = 30;
            AddReport("More Entertainment needed soon");
        }
        else if ((float)(NumberOfNPCs / 60) >= NumberrOfEntertainment && (float)(NumberOfNPCs / 40) < NumberrOfEntertainment)
        {
            //one venue per 60-40 people
            EntertainmentRating = 60;
            AddReport("More Entertainment needed soon");
        }
        else if ((float)(NumberOfNPCs / 40) >= NumberrOfEntertainment && (float)(NumberOfNPCs / 30) < NumberrOfEntertainment)
        {
            //one venue per 40-20 people
            EntertainmentRating = 60;
            AddReport("A good amount of entertainment");
        }
        else
        {
            //one venue per 30 people or more
            EntertainmentRating = 100;
            AddReport("A good amount of Entertainment");
        }
    }
    public void SetPowerRating(int PowerGeneration,int PowerUsage)
    {
        double Generation = PowerGeneration;
        double Usage = PowerUsage;
        if ((Generation == 0))
        {
            PowerRating = 0;
            AddReport("Build a power plant as soon as possible");
        }
        else
        {
            if (Generation < Usage / 3.0)
            {
                PowerRating = 10;
                AddReport("Build a power plant as soon as possible");
            }
            else if (Generation <= Usage / 2.0) {
                PowerRating = 25;
                AddReport("Build a power plant as soon as possible");
            }
            else if (Generation <= Usage / 1.3)
            {
                PowerRating = 45;
                AddReport("Build a power plant soon");
            }
            else if (Generation<=Usage)
            {
                PowerRating = 65;
                AddReport("More power needed");
            }
            else if (Generation <= Usage*1.33)
            {
                PowerRating = 85;
                AddReport("A good amount of power");
            }
            else
            {
                PowerRating = 100;
                AddReport("A good amount of power");
            }
        }


    }
    public void SetEnviromentalEffectRating(int Ef, int NumberOfNPCs)
    {
        float EFPerPerson = (float)Ef / (float)NumberOfNPCs;
        if(EFPerPerson >300 )
        {
            EntertainmentRating = 0;
            ReportList.Add("Extremely high enviromnetal effects");
        }
        else if (EFPerPerson > 250)
        {
            EntertainmentRating = 20;
            ReportList.Add("high enviromnetal effects");
        }
        else if (EFPerPerson > 200)
        {
            EntertainmentRating = 35;
            ReportList.Add("Moderate enviromnetal effects");
        }
        else if (EFPerPerson > 150)
        {
            EntertainmentRating = 55;
            ReportList.Add("Alright enviromnetal effects");
        }
        else if (EFPerPerson > 100)
        {
            EntertainmentRating = 75;
            ReportList.Add("Alright enviromnetal effects");
        }
        else
        {
            EntertainmentRating = 100;
            ReportList.Add("good enviromnetal effects");
        }

    }
    public void SetGreeneryRating(int NumberOfGreenery, int NumberOfNatureAreas,int NumberOfNPCs) { 
        int OverallGreeneryAmount=NumberOfGreenery+NumberOfNatureAreas*10;
        if (OverallGreeneryAmount == 0)
        { 
            GreeneryRating = 0;
            AddReport("No greenery, add some very soon");
        }
        else if(OverallGreeneryAmount < NumberOfNPCs)
        {
            GreeneryRating = 5;
            AddReport("Very little greenery, add some soon");
        }
        else if ( OverallGreeneryAmount<NumberOfNPCs*2 && OverallGreeneryAmount >= NumberOfNPCs)
        {
            GreeneryRating = 15;
            AddReport("Very little greenery, add some soon");
        }
        else if (OverallGreeneryAmount < NumberOfNPCs * 4 && OverallGreeneryAmount >= NumberOfNPCs*2)
        {
            GreeneryRating = 25;
            AddReport("Not enough greenery, add some soon");
        }
        else if (OverallGreeneryAmount < NumberOfNPCs * 10 && OverallGreeneryAmount >= NumberOfNPCs * 4)
        {
            GreeneryRating = 35;
            AddReport("Not enough greenery, add some soon");
        }
        else if (OverallGreeneryAmount < NumberOfNPCs * 15 && OverallGreeneryAmount >= NumberOfNPCs * 10)
        {
            GreeneryRating = 45;
            AddReport("Not enough greenery, add some soon");
        }
        else if (OverallGreeneryAmount < NumberOfNPCs * 20 && OverallGreeneryAmount >= NumberOfNPCs * 15)
        {
            GreeneryRating = 55;
            AddReport("An alright amount of greenery, but more would be an improvement");
        }
        else if (OverallGreeneryAmount < NumberOfNPCs * 22 && OverallGreeneryAmount >= NumberOfNPCs * 20)
        {
            GreeneryRating = 65;
            AddReport("An alright amount of greenery, but more would be an improvement");
        }
        else if (OverallGreeneryAmount < NumberOfNPCs * 23 && OverallGreeneryAmount >= NumberOfNPCs * 22)
        {
            GreeneryRating = 75;
            AddReport("A  very good amount of greenery, but more would be an improvement");
        }
        else if (OverallGreeneryAmount < NumberOfNPCs * 24 && OverallGreeneryAmount >= NumberOfNPCs * 23)
        {
            GreeneryRating = 85;
            AddReport("A  very good amount of greenery, but more would be an improvement");
        }
        else if (OverallGreeneryAmount < NumberOfNPCs * 25 && OverallGreeneryAmount >= NumberOfNPCs * 24)
        {
            GreeneryRating = 95;
            AddReport("A very good amount of greenery");
        }
        else if (OverallGreeneryAmount >= NumberOfNPCs * 25)
        {
            GreeneryRating = 100;
            AddReport("Amazing amount of greenery!");
        }

    }

    public void SetPowerReachRating(List<PlacedBuilding> Buildings)
    {
        int total = 0;
        for (int i = 0; i < Buildings.Count; i++)
        {
            if (Buildings[i].GetIfInRangeOfPowerPlant())
            {
                total++;
            }
        }
        float Percentage = ((float)total /( float)(Buildings.Count)) * 100;
        if(Percentage < 10)
        {
            PowerReachRating = 0;
            ReportList.Add("Very bad power reach, build a power plant close to other buildings as soon as possible");
        }
        else if (Percentage < 25)
        {
            PowerReachRating = 15;
            ReportList.Add("Very bad power reach, build a power plant close to other buildings when you can");
        }
        else if (Percentage < 40)
        {
            PowerReachRating = 35;
            ReportList.Add("bad power reach, build a power plant close to other buildings soon");
        }
        else  if (Percentage < 50)
        {
            PowerReachRating = 55;
            ReportList.Add("okay power reach, build a power plant close to other buildings soon");
        }
        else if (Percentage < 70)
        {
            PowerReachRating = 70;
            ReportList.Add("good power reach");
        }
        else if (Percentage < 90)
        {
            PowerReachRating = 85;
            ReportList.Add("Very good power reach");
        }
        else if (Percentage >= 90)
        {
            PowerReachRating = 100;
            ReportList.Add("Amazing power reach");
        }
    }
    public void CalulcateRating()
    {
        Rating=((int)(100-HomeLessPercentage)+ShopRating+HospitalRating+RoadRating+PowerRating+EntertainmentRating+EnviromentRating+PowerReachRating+TrainStationRating)/9;
    }
    public void SetTrainStationRating(int NumberOfNPCs, int NumberOfTrainStation)
    {
        if (NumberOfTrainStation == 0)
        {
            //No stations
            TrainStationRating = 0;
            AddReport("No train stations, build one as soon as possible");
        }
        else if ((float)(NumberOfNPCs / 200) >= NumberOfTrainStation && (float)(NumberOfNPCs / 150) < NumberOfTrainStation)
        {
            //one shop per 40-30 people
            TrainStationRating = 30;
            AddReport("Another stations needed very soon");
        }
        else if ((float)(NumberOfNPCs / 150) >= NumberOfTrainStation && (float)(NumberOfNPCs / 120) < NumberOfTrainStation)
        {
            //one train stations per 150-120 people
            TrainStationRating = 55;
            AddReport("More stations needed somewhat soon");
        }
        else if ((float)(NumberOfNPCs / 120) >= NumberOfTrainStation && (float)(NumberOfNPCs / 100) < NumberOfTrainStation)
        {
            //one Train station per 120-100 people
            TrainStationRating = 75;
            AddReport("A good amount of train stations");
        }
        else
        {
            //one train station per 100 people or more
            TrainStationRating = 100;
            AddReport("A very good amount of train stations");
        }
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
