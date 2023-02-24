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
            //RIT.RochesterLOS.Serialization.Serializer.Instance.RegisterSerializationTarget<string[]>("/Data/");
            var config = (IConfigurationService)ServiceLocator.GetService<IConfigurationService>();
            SignDataFileName = (string)config.GetConfigValue("active_sign_data");
        }

        internal static void Serialize(List<SignData> data)
        {
            //CheckInstance();
            var lines = new List<string>();
            foreach(var sign in data)
            {
                var line = $"{sign.Lat}, {sign.Lon}, {sign.Elev}, {sign.Type}, {sign.Name}";
                lines.Add(line);
            }

            serialization.SaveLines(SignDataFileName, lines);
            //RIT.RochesterLOS.Serialization.Serializer.Instance.SaveObject<string[]>(lines.ToArray(), "SignPlaces");
        }

        internal static async Task<SignData[]> Deserialize()
        {
            var data = await serialization.ReadLinesAsnyc(SignDataFileName);

            var signs = new SignData[data.Length - 1]; //Don't care about first row headers
            for (var i = 1; i < data.Length; i++)
            {
                var line = data[i];
                var values = line.Split(',');

                try
                {
                    signs[i - 1] = new SignData()
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

            return signs;
        }
    }
}
