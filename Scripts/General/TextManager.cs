using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class TextManager : MonoBehaviour
{
    public static string[] GetTextArray(string address) 
    {
        Debug.Log(address);
        string[] result;

        TextAsset text = (TextAsset)Resources.Load(address, typeof(TextAsset));
        if (text == null)
            return null;
        using (System.IO.StringReader reader = new System.IO.StringReader(text.ToString())) 
        {
            string arrayElement;
            List<string> list = new List<string>();

            while ((arrayElement = reader.ReadLine()) != null) { list.Add(arrayElement); }
            result = list.ToArray();
        }
        
        return result;
    }
    public static string[] GetTextArrayOnlySelected(string address, int[] selectedLineNumbers) 
    {
        List<string> result = new List<string>();
        TextAsset text = (TextAsset)Resources.Load(address, typeof(TextAsset));
        using (System.IO.StringReader reader = new System.IO.StringReader(text.ToString())) 
        {
            int loopTime = 0;
            int slnIndex = 0;
            string arrayElement;
            while((arrayElement = reader.ReadLine()) != null)
            {
                if (slnIndex == loopTime) { result.Add(arrayElement); slnIndex++; }                    
                loopTime++;
            }
        }

        return result.ToArray();
    }

    public class Address 
    {
        public static string Build(GeneralVariables.Language language, GeneralVariables.Scene scene, string fileName) 
        {
            string result = @"Texts\" + language + @"\";

            if (scene == GeneralVariables.Scene.Factory || scene == GeneralVariables.Scene.Underground_0 || scene == GeneralVariables.Scene.Underground_1 || scene == GeneralVariables.Scene.RailwayStation)
                result += @"GameScenes\";

            result += scene + @"\";
            result += fileName;

            return result;
        }
    }
}
