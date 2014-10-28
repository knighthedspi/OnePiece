using UnityEngine;
using System.Collections;

public class RandomUtility {

    public const int miss = -1;

    public static int RandomIndexUsingWeight(int[] weightTable) {
        int sum = 0;
        foreach(int n in weightTable) sum += n;
        int rand = Random.Range(0, sum);
        for(int i = 0; i < weightTable.Length; i++) {
            if(rand < weightTable[i]) return i;
            rand -= weightTable[i];
        }
        Debug.LogWarning("Invalid!");
        return miss;
    }

    public static int RandomIndex(float[] probabilityTable) {
        float rand = Random.value;
        for (int i = 0; i < probabilityTable.Length; i++) {
            if (rand < probabilityTable[i]) return i;
            rand -= probabilityTable[i];
        }
        return miss;
    }

    public static bool IsHitPercent(float percent) {
        if (percent >= 1f) return true;
        if (percent <= 0f) return false;
        return Random.value <= percent;
    }
}