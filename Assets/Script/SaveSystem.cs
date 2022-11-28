using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace StayAwayGameScript
{
    public static class SaveSystem
    {
        public static void Save(string path, object data)
        {
            var json = JsonUtility.ToJson(data, true);
            File.WriteAllText(path, json);
        }

        public static T Load<T>(string path)
        {
            return JsonUtility.FromJson<T>(File.ReadAllText(path));
        }
    }
}