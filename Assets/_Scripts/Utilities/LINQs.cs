using System.Collections.Generic;

public static class LINQs
{
    public static List<T2> CastTo<T1, T2>(this List<T1> list) where T2 : class 
    {
        List<T2> newList = new List<T2>();
        foreach (var value in list)
        {
            newList.Add(value as T2);
        }
        return newList;
    }
}