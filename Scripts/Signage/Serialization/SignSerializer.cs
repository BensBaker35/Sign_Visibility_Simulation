using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using RIT.RochesterLOS.Events;
using RIT.RochesterLOS.Services;
using UnityEngine;

namespace RIT.RochesterLOS.Signage.Serialization
{
    public sealed class SignSerializer
    {

        //private static string SignDataFileLoc = "/Data/";
        private static string SignDataFileName = "SignPlaces.csv";
        private static ITextSerialization serialization;

        static SignSerializer()
        {
            serialization = (ITextSerialization)ServiceLocator.GetService<ITextSerialization>();
            var config = (IConfigurationService)ServiceLocator.GetService<IConfigurationService>();
            var currentSaveDirectory = config.GetConfigValue("data_directory");
            SignDataFileName = currentSaveDirectory + (string)config.GetConfigValue("active_sign_data");
            Debug.Log($"SIGN SERIALIZER: file - {SignDataFileName}");
        }

        internal static void Serialize(List<SignData> data)
        {
            //CheckInstance();
            var lines = new List<string>();
            foreach (var sign in data)
            {
                var line = $"{sign.Lat}, {sign.Lon}, {sign.Elev}, {sign.Type}, {sign.Name}";
                lines.Add(line);
            }

            serialization.SaveLines(SignDataFileName, lines);
        }

        internal static async Task<SignData[]> Deserialize()
        {
            string[] data;
#if SIGN_SIM_DEMO
            Debug.Log("Using fake data");
            data = new string[]{
                "43.1551264396326, -77.6055556830554, 162.943313598633, OBSTACLE,   From Placement",
                "43.1572189561807, -77.6061114788337, 166.493301391602, OBSTACLE,  From Placement",
                "43.1572197813584, -77.6060086719294, 166.234985351563, BASE, From Placement"
            };
            
#else
            data = await serialization.ReadLinesAsnyc(SignDataFileName);
#endif
            if(data == null || data.Length == 0) return new SignData[0];

            var signs = new SignData[data.Length];
            for (var i = 0; i < data.Length; i++)
            {
                var line = data[i];
                var values = line.Split(',');

                try
                {
                    signs[i] = new SignData()
                    {
                        Lat = double.Parse(values[0]),
                        Lon = double.Parse(values[1]),
                        Elev = double.Parse(values[2]),
                        Type = !string.IsNullOrEmpty(values[3]) ? (SignType)Enum.Parse(typeof(SignType), values[3], true) : SignType.BASE,
                        Name = !string.IsNullOrEmpty(values[4]) ? values[4] : "Sign " + values[3],

                    };
                }
                catch (Exception e)
                {
                    Debug.LogError($"Failed to parse: \n{e}\n{line}, {i}");
                }
            }
            Debug.Log($"Sign Serializer: Sings Count - {signs.Length}");
            return signs;
        }
    }
}
