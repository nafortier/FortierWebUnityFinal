using System;

public static class JsonArrayWrapper 
{
    [Serializable]

    private class Wrapper<T>
    {
        public T[] items;
    }

    public static T[] FromJsonArray<T>(string jsonArray)
    {
        var wrapped = "{\"items\":" + jsonArray + "}";
        return UnityEngine.JsonUtility.FromJson<Wrapper<T>>(wrapped).items;
    }
}
