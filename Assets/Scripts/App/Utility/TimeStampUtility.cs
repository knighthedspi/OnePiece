using UnityEngine;
using System.Collections;
using System;

public class TimeStampUtility {

    public static int convertTimeToInt(DateTime time)
    {
        return (Int32)(time.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
    }
}