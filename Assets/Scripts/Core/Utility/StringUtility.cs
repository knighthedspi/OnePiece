using UnityEngine;
using System.Collections;

public class StringUtility {

    public static string ToStringSafe(object obj) {
        return obj == null ? string.Empty : obj.ToString();
    }
}
