using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;

public class ReadData
{
    #region Gaze Variables
    int eyeDataTimeStampId = 0;
    int gazeOriginIdX = 5;
    int gazeOriginIdY = 6;
    int gazeOriginIdZ = 7;
    int gazeDirIdX = 8;
    int gazeDirIdY = 9;
    int gazeDirIdZ = 10;
    int gazeValue = 4;
    int hitPoint_XID = 12;
    int hitPoint_YID = 13;
    int hitPoint_ZID = 14;
    int modified_ID=62;
    int angularVelocity_ID=61;
    int classification_ID=63;
    int event_id=64;
    int eventDur_id=66;
    int fixation_XID=67;
    int fixation_YID=68;
    int fixation_ZID=69;
    int target_name = 15;




    //Data needed for Replay
    long eyeDataTimeStamp;
    DateTime timeStamp;
    float gazeOriginX;
    float gazeOriginY;
    float gazeOriginZ;
    float gazeDirX;
    float gazeDirY;
    float gazeDirZ;
    float hit_x;
    float hit_y;
    float hit_z;
    float fixation_x;
    float fixation_y;
    float fixation_z;
    float event_duration;
    int event_no;
    string event_name;
    float angluar_v;




    //save all data into List to utilize them in the Update Method
    ArrayList gazeData = new ArrayList();
    public List<Vector3> gazeOrigin = new List<Vector3>();
    public List<string> target = new List<string>();
    public List<Vector3> gazeDir = new List<Vector3>();
    public List<Vector3> hitPoint = new List<Vector3>();
    public List<Vector3> fixationPoint = new List<Vector3>();
    public List<DateTime> eyeDataTime = new List<DateTime>();
    public List<long> eyeUnixTime = new List<long>();
    public List<float> eventDuration = new List<float>();
    public List<int> eventNo = new List<int>();
    public List<string> eventName = new List<string>();
    public List<float> angularV = new List<float>();

    public List<string> sphereHit;

    #endregion

    public Vector3 position;
    public Quaternion rotation;

    #region read ArettData from csv

    // public List<string> fileName = new List<string>();
    public void readGazeData(string fileName)
    {
        int count = 0;
        gazeOrigin.Clear();
        gazeDir.Clear();
        hitPoint.Clear();
        eyeDataTime.Clear();
        fixationPoint.Clear();
        target.Clear();
        eventDuration.Clear();
        eventNo.Clear();
        eventName.Clear();
        angularV.Clear();
        eyeUnixTime.Clear();

        // Loading the dataset from Unity's Resources folder
        var dataset = Resources.Load<TextAsset>("CSVFiles/GazeData/Fixation/" + fileName);

        string[] dataLines = dataset.text.Split('\n');
    

        //Debug.Log("DataLinesLength"+dataLines.Length);
        foreach (string line in dataLines.Skip(1))
        {
            string[] column = line.Split(',');

         
            count += 1;
            if (column.Length >= 4)
            {
                
                if (!(column[gazeValue] == "FALSE" && column[modified_ID] == "FALSE"))
                {

                    //parseStringToLong(column[eyeDataTimeStampId], eyeDataTimeStamp);
                
                    //eyeDataTimeStamp = column[eyeDataTimeStampId];
                    eyeDataTimeStamp=long.Parse(column[eyeDataTimeStampId], CultureInfo.InvariantCulture);
                    eyeDataTime.Add(UnixToUtc(eyeDataTimeStamp));
                    eyeUnixTime.Add(eyeDataTimeStamp);

                    //  gazeOriginX = parseStringToFloat(column[gazeOriginIdX], gazeOriginX);
                    // gazeOriginY = parseStringToFloat(column[gazeOriginIdY], gazeOriginY);
                    // gazeOriginZ = parseStringToFloat(column[gazeOriginIdZ], gazeOriginZ);
                    if (column[gazeOriginIdX] != "NA")
                    {
                        gazeOriginX = float.Parse(column[gazeOriginIdX], CultureInfo.InvariantCulture);
                        gazeOriginY = float.Parse(column[gazeOriginIdY], CultureInfo.InvariantCulture);
                        gazeOriginZ = float.Parse(column[gazeOriginIdZ], CultureInfo.InvariantCulture);
                    }
                    
                   // Debug.Log("GazeoriginX"+gazeOriginX);
                    //Vector3 temp = new Vector3((gazeOriginX / 100000f), (gazeOriginY / 100000f), (gazeOriginZ / 100000f));
                  //  Debug.Log("GazeX"+gazeOriginX+"lineNO"+count);
                  //  Debug.Log("GazeY" + gazeOriginY + "lineNO" + count);
                  //  Debug.Log("GazeZ" + gazeOriginZ + "lineNO" + count);
                    gazeOrigin.Add(new Vector3((gazeOriginX), (gazeOriginY ), (gazeOriginZ)));

                  
            
                    if (column[gazeDirIdX] != "NA")
                    {
                        gazeDirX = float.Parse(column[gazeDirIdX], CultureInfo.InvariantCulture);
                        gazeDirY = float.Parse(column[gazeDirIdY], CultureInfo.InvariantCulture);
                        gazeDirZ = float.Parse(column[gazeDirIdZ], CultureInfo.InvariantCulture);
                    }
               

                    gazeDir.Add(new Vector3((gazeDirX), (gazeDirY), (gazeDirZ)));


                    if (column[hitPoint_XID]!="NA")
                    {
                        hit_x = float.Parse(column[hitPoint_XID], CultureInfo.InvariantCulture);
                        hit_y = float.Parse(column[hitPoint_YID], CultureInfo.InvariantCulture);
                        hit_z = float.Parse(column[hitPoint_ZID], CultureInfo.InvariantCulture);

                     
                    }
                   
                    hitPoint.Add(new Vector3((hit_x), (hit_y), (hit_z)));

                    if (column[fixation_XID]!="NA")
                    {
                        fixation_x = float.Parse(column[fixation_XID], CultureInfo.InvariantCulture);
                        fixation_y = float.Parse(column[fixation_YID], CultureInfo.InvariantCulture);
                        fixation_z = float.Parse(column[fixation_ZID], CultureInfo.InvariantCulture);
                    }
                 //   Debug.Log("FixationPoint"+"x :"+fixation_x+"y :"+fixation_y+"z :"+fixation_z);
                    fixationPoint.Add(new Vector3(fixation_x,fixation_y,fixation_z));

                    if (column[eventDur_id]!="NA")
                    {
                        event_duration = float.Parse(column[eventDur_id], CultureInfo.InvariantCulture);
                        eventDuration.Add(event_duration);
                    }
                    else
                    {
                        eventDuration.Add(0);
                    }
                    //  Debug.Log("Event DUration"+event_duration);
                
                    if (column[event_id] != "NA")
                    {
                        event_no = int.Parse(column[event_id], CultureInfo.InvariantCulture);
                        eventNo.Add(event_no);
                    }
                    else
                    {
                        eventNo.Add(0);
                    }
                  //  Debug.Log("Event ID" + event_no);
                    if (column[classification_ID]!="NA")
                    {
                        event_name = column[classification_ID];
                        event_name = event_name.TrimStart('"');
                        event_name = event_name.TrimEnd('"');
                        eventName.Add(event_name);
                    }
                    else
                    {
                        eventName.Add("NA");
                    }
                    //   Debug.Log("Event Class" + event_name);

                   // Debug.Log("Angularvelocity"+column[angularVelocity_ID]);
                    if (column[angularVelocity_ID]!="NA"&& column[angularVelocity_ID] != "Inf" )
                    {
                        angluar_v = float.Parse(column[angularVelocity_ID],CultureInfo.InvariantCulture);
                        angularV.Add(angluar_v);
                    }
                    else
                    {
                        angularV.Add(0);
                    }
                }
            }
        }
    }

    #endregion
    public void readGazeDataCalibration(string fileName)
    {
        int count = 0;
        gazeOrigin.Clear();
        gazeDir.Clear();
        hitPoint.Clear();
        eyeDataTime.Clear();
        target.Clear();
        // Loading the dataset from Unity's Resources folder
        var dataset = Resources.Load<TextAsset>("CSVFiles/GazeData/" + fileName);

        string[] dataLines = dataset.text.Split('\n');


       
        foreach (string line in dataLines.Skip(1))
        {
            string[] column = line.Split(',');


            count += 1;
            if (column.Length >= 4)
            {

                if (!(column[gazeValue] == "FALSE"))
                {

                  
                
                    eyeDataTimeStamp = long.Parse(column[eyeDataTimeStampId], CultureInfo.InvariantCulture);
                    eyeDataTime.Add(UnixToUtc(eyeDataTimeStamp));
                    eyeUnixTime.Add(eyeDataTimeStamp);

                 
                    if (column[gazeOriginIdX] != "NA")
                    {
                        gazeOriginX = float.Parse(column[gazeOriginIdX], CultureInfo.InvariantCulture);
                        gazeOriginY = float.Parse(column[gazeOriginIdY], CultureInfo.InvariantCulture);
                        gazeOriginZ = float.Parse(column[gazeOriginIdZ], CultureInfo.InvariantCulture);
                    }

           
                    gazeOrigin.Add(new Vector3((gazeOriginX), (gazeOriginY), (gazeOriginZ)));



                    if (column[gazeDirIdX] != "NA")
                    {
                        gazeDirX = float.Parse(column[gazeDirIdX], CultureInfo.InvariantCulture);
                        gazeDirY = float.Parse(column[gazeDirIdY], CultureInfo.InvariantCulture);
                        gazeDirZ = float.Parse(column[gazeDirIdZ], CultureInfo.InvariantCulture);
                    }


                    gazeDir.Add(new Vector3((gazeDirX), (gazeDirY), (gazeDirZ)));


                    if (column[hitPoint_XID] != "NA")
                    {
                        hit_x = float.Parse(column[hitPoint_XID], CultureInfo.InvariantCulture);
                        hit_y = float.Parse(column[hitPoint_YID], CultureInfo.InvariantCulture);
                        hit_z = float.Parse(column[hitPoint_ZID], CultureInfo.InvariantCulture);


                    }

                    hitPoint.Add(new Vector3((hit_x), (hit_y), (hit_z)));
                    Debug.Log("filemame"+fileName);
                    Debug.Log("Hit"+"x:"+hit_x+"y"+hit_y+"z"+hit_z);
                    target.Add(column[target_name]);
                }
            }
        }
    }

    public void readWorldData(string path)
    {
        var dataset = Resources.Load<TextAsset>("CSVFiles/AnchorFile/" + path);

        string[] dataLines = dataset.text.Split('\n');

        string[] column = dataLines[1].Split(',');


        float pX = float.Parse(column[3], CultureInfo.InvariantCulture);
        float pY = float.Parse(column[4], CultureInfo.InvariantCulture);
        float pZ = float.Parse(column[5], CultureInfo.InvariantCulture);

        position = new Vector3(pX, pY, pZ);


        float rX = float.Parse(column[6], CultureInfo.InvariantCulture);
        float rY = float.Parse(column[7], CultureInfo.InvariantCulture);
        float rZ = float.Parse(column[8], CultureInfo.InvariantCulture);
        float rW = float.Parse(column[9], CultureInfo.InvariantCulture);


        rotation = new Quaternion(rX, rY, rZ, rW);
     
    }

    public List<Vector3> getGazeOrigin()
    {
       
        return gazeOrigin;
    }
    public List<Vector3> getGazeDir()
    {
        return gazeDir;
    }

    public List<Vector3> getHitPoint() { 
        return hitPoint;
    }
    public List<DateTime> getEyeDataTime()
    {
        return eyeDataTime;
    }

    public List<long> getUnixTime()
    {
        return eyeUnixTime;
    }

    DateTime UnixToUtc(long unixTime)
    {
        DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeMilliseconds(unixTime);
        DateTime dateTime = dateTimeOffset.UtcDateTime;

        return dateTime;
    }

    long parseStringToLong(String str, long output)
    {
        long.TryParse(str, out output);
        return output;
    }

    float parseStringToFloat(String str, float output)
    {
        float.TryParse(str, out output);

        return output;
    }

}
