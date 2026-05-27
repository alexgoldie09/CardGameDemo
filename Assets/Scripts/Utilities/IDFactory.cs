using UnityEngine;

public static class IDFactory
{
    private static int Count;
    
    /// <summary>
    /// Returns a unique ID. The first time this method is called,
    /// it will return 1, then 2, and so on.
    /// </summary>
    /// <returns></returns>
    public static int GetUniqueID()
    {
        // Count++ has to go first, otherwise - unreachable code.
        Count++;
        return Count;
    }
    
    /// <summary>
    /// Resets the ID counter. After calling this method,
    /// the next call to GetUniqueID() will return 1 again.
    /// </summary>
    public static void ResetIDs() => Count = 0;
}
