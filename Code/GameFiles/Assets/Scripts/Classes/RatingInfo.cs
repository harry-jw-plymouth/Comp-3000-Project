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

    int AirQualityRating = 0;
    int WastageRating = 0;
    int WaterPollutionRating= 0;
    // Add report to be displayed based on homeless info
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
    // return percentage of homeless NPCs
    public float GetHomeLessPercentage() { 
        return HomeLessPercentage;
    }
    // Set rating from 0-100 based on how many roads there are relative to NPCs 
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
    // set value from 0-100 and add report based on how many hospiatals there are relative to NPCs
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
    // set value from 0-100 and add report based on how many shops there are relative to NPCs
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
    // set value from 0-100 and add report based on how much entertainment there is relative to NPCs
    public void SetEntertainmentRating(int NumberOfNPCs,int NumberrOfEntertainment)
    {
        if (NumberrOfEntertainment == 0)
        {
            //No shops
            AddReport("No entertainment, build some as soon as possible");
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
            AddReport("A great amount of Entertainment!");
        }
    }
    // set value from 0-100 and add report based on power usage relative to power consumption
    public void SetPowerRating(int PowerGeneration,int PowerUsage)
    {
        double Generation = PowerGeneration;
        double Usage = PowerUsage;
        if ((Generation == 0))
        {
            PowerRating = 0;
            AddReport("Build a power plant as soon as possible, power generation critical");
        }
        else
        {
            if (Generation < Usage / 3.0)
            {
                PowerRating = 10;
                AddReport("Build a power plant as soon as possible, power generation critical");
            }
            else if (Generation <= Usage / 2.0) {
                PowerRating = 25;
                AddReport("Build a power plant as soon as possible,power generation low");
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
                AddReport("A great amount of power! Power levels stable");
            }
        }


    }
    // set value from 0-100 and add report based on how much the environment is effected by what the player has built
    public void SetEnviromentalEffectRating(int Ef, int NumberOfNPCs)
    {
        float EFPerPerson = (float)Ef / (float)NumberOfNPCs;
        if(EFPerPerson >300 )
        {
            EnviromentRating = 0;
            ReportList.Add("Your city is doing terrible damage to the enviornment, consider changing things soon");
        }
        else if (EFPerPerson > 250)
        {
            EnviromentRating = 20;
            ReportList.Add("Your city is having a terrible effect on the environment, consider changing buildings to more eco-friendly options");
        }
        else if (EFPerPerson > 200)
        {
            EnviromentRating = 35;
            ReportList.Add("Moderate environmental impact from the city, consider changing buildings to more eco-friendly options");
        }
        else if (EFPerPerson > 150)
        {
            EnviromentRating = 55;
            ReportList.Add("Alright city effects on environment, but still improvements can be made");
        }
        else if (EFPerPerson > 100)
        {
            EnviromentRating     = 75;
            ReportList.Add("Good eco friendliness overall, but more improvements can be made");
        }
        else
        {
            EnviromentRating = 100;
            ReportList.Add("Good enviromnetal effects, your city is doing a great job of co-existing with nature!");
        }
    }
    // set value from 0-100 and add report based on how much waste is currently built up relative to number of people in the city
    public int SetWastageRating(int WasteAmount,int NumberOfNPCs)
    {
        float WastePerPerson = (float)WasteAmount / (float)NumberOfNPCs;
        if (WastePerPerson > 50)
        {
            WastageRating = 0;
            ReportList.Add("Extremely high waste amount, build wastage facilities as soon as possible. Consider a recycling facility if possible");
        }
        else if (WastePerPerson > 40)
        {
            WastageRating = 20;
            ReportList.Add("high waste amount, more wastage facilities needed as soon as possible, consider a recycling facility if possible");
        }
        else if (WastePerPerson > 30)
        {
            WastageRating = 35;
            ReportList.Add("Moderate waste amount, consider adding more wastage facilities");
        }
        else if (WastePerPerson > 20)
        {
            WastageRating = 55;
            ReportList.Add("Moderate amount of waste, consider building new wastage facilities");
        }
        else if (WastePerPerson > 10)
        {
            WastageRating= 75;
            ReportList.Add("Alright waste amount, youre handling it well! But another recycling plant could help");
        }
        else
        {
            WastageRating = 100;
            ReportList.Add("Excellent managment of waste! To improve environmental friendliness further, consider swapping any regular facilities into recycling facilities! " );
        }
        return (int)WastePerPerson;
    }
    // return air qaulity rating
    public int GetAirQualityRating()
    {
        return AirQualityRating;
    }
    // set a value 0-100 rating how much air pollution exsits from buildings relative to the number of greenery  and set report accordingly
    public void SetAirQaulityRating(List<PlacedBuilding> Buildings, int NumberOfGreenery) { 
        int EffectFromBuildings = 0;
        for (int i = 0; i < Buildings.Count; i++)
        {
            EffectFromBuildings += Buildings[i].GetEnviromentalValue();
        }
        int EffectFromGreenery = (int)((float)NumberOfGreenery * 1.0f);

        int OverallEffects = EffectFromBuildings - EffectFromGreenery;
        //Higher is worse

        if (OverallEffects < 0) { 
            AirQualityRating = 100;
            AddReport("Amazing air quality! You've balanced infastructure and green spaces very well!");
        }
        else if (OverallEffects>=0 && OverallEffects<100)
        {
            AirQualityRating = 85;
            AddReport("Great air quality!But more geeen spaces never hurts a city");
        }
        else if (OverallEffects >= 100 && OverallEffects < 200)
        {
            AirQualityRating = 75;
            AddReport("Great air quality! But consider adding more green spaces");
        }
        else if (OverallEffects >= 200 && OverallEffects < 300)
        {
            AirQualityRating = 65;
            AddReport("Great air quality, But could see improvement. More green spaces definitely needed");
        }
        else if (OverallEffects >= 300 && OverallEffects < 400)
        {
            AirQualityRating = 55;
            AddReport("good air quality, but more greenery would improve it, consider adding some more soon");
        }
        else if (OverallEffects >= 400 && OverallEffects < 500)
        {
            AirQualityRating = 45;
            AddReport("Potentially fine air quality, but adding more greenery would improve it and ensure your citzens dont get sick");
        }
        else if (OverallEffects >= 500 && OverallEffects < 600)
        {
            AirQualityRating = 35;
            AddReport("Bad air quality, add more greenery whenever you can, your citzens may get sick");
        }
        else if (OverallEffects >= 600 && OverallEffects < 700)
        {
            AirQualityRating = 20;
            AddReport("Bad Air quality, add more greenery soon, your citzens health is effected");
        }
        else if (OverallEffects >= 700 && OverallEffects < 800)
        {
            AirQualityRating = 5;
            AddReport("Bad air quality, add more greenery soon, your citzens health is effected");
        }
        else
        {
            AirQualityRating = 0;
            AddReport("Air qaulity critical, your citzens health is effected. Build more green spaces as soon as possible");
        }



    }
    // set a value 0-100 rating how much greenery is in the city relative to the number of NPCs and set report accordingly
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
    // set a value 0-100 rating how many buildings are within reach of a power plant and add relevant report
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
    // calculate the rating overall for pollution in the city
    public int GetOverallRatingForPollution()
    {
               return (AirQualityRating + WastageRating + WaterPollutionRating+GreeneryRating) / 4;
    }
    // calculate the overall rating based on all indiviual ratings
    public void CalulcateRating()
    {
        Rating=(((int)(100-HomeLessPercentage)+ShopRating+
            HospitalRating+RoadRating+PowerRating+EntertainmentRating+
            EnviromentRating+PowerReachRating+TrainStationRating)/8)/2+GetOverallRatingForPollution()/2;
    }
    // set value 0=100 rating how mcuh water pollution exists relative to the amount of water in the map
    public void CalculateWaterPollutionRating(int WaterPollution, int WaterTiles)
    {
        float PollutionPerTile = (float)WaterPollution / (float)WaterTiles;
        if (PollutionPerTile > 40)
        {
            WaterPollutionRating = 0;
            ReportList.Add("Extremely high water pollution, consider buildings whith less pollution levels, this could get your citzens sick");
        }
        else if (PollutionPerTile > 30)
        {
            WaterPollutionRating = 20;
            ReportList.Add("high water pollution, consider buildings whith less pollution levels, this could get your citzens sick");
        }
        else if (PollutionPerTile > 20)
        {
            WaterPollutionRating = 35;
            ReportList.Add("Moderate water pollution, consider buildings whith less pollution levels soon");
        }
        else if (PollutionPerTile > 10)
        {
            WaterPollutionRating = 55;
            ReportList.Add("Alright water pollution levels, but could see improvement");
        }
        else if (PollutionPerTile > 5)
        {
            WaterPollutionRating = 75;
            ReportList.Add("Alright water pollution levels");
        }
        else
        {
            WaterPollutionRating = 100;
            ReportList.Add("Very good water pollution levels, your citzens are not at risk due to bad water!");
        }
    }
    // set value 0=100 rating how many train stations exist relative to the number of people in the city 
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
    // add report to list
    public void AddReport(string Report) {  
        ReportList.Add(Report); 
    }
    // get list of reports 
    public List<string> GetReport()
    {
        return ReportList;
    }
    // clear reports list so fresh batch can be added
    public void ClearReports()
    {
        ReportList.Clear();
    }
    // return current rating
    public float GetRating() { 
        return Rating;
    }
}
