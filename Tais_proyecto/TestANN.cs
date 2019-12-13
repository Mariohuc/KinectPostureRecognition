using System;
using Apps.engine.KinectRecognition;
using Microsoft.Kinect;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Apps.engine.neuron.Utilities;
using ConnectToSQLServer;
using System.Text.RegularExpressions;

namespace Tais_proyecto
{
    class TestANN
    {
        static void Main(string[] args)
        {
            PostureRecognition<Skeleton, string> postureRecognition = new PostureRecognition<Skeleton, string>(PatternType.AnglePatternElbowKnee, DataTrainingType.DataTrainingFile, 100000);
            //1000000
            //Console.WriteLine("MOSTRANDO DATOS BRUTOS" + "---------------------------------------------------");
            //List<Pose> poselist = LoadData("TRAINING_DATA3");
            //printPoses(poselist);
            //Console.WriteLine("ITERACION : " + postureRecognition.training() + "-------------------------------------------------------\n");
            
            Console.WriteLine("TESTING" + "-------------------------------------------------------");

            postureRecognition = ReadWriteObjectFile.FromByteArray<PostureRecognition<Skeleton, string>>(Convert.FromBase64String(DbServices.GetTrainingRed("octavo", "posture")));

            String folderPath = "TESTING";
            int counter;
            foreach (string file in Directory.EnumerateFiles(folderPath, "*.TD"))
            {
                var readFile = File.ReadAllBytes(file);
                var skeletonList = ReadWriteObjectFile.FromByteArray<List<Skeleton>>(readFile);
                counter = 0;
                foreach (Skeleton skel in skeletonList)
                {
                    if (counter == 7) break;
                    var x = postureRecognition.Predict(skel);
                    if (x != null && x.Length > 0)
                        Console.Write(x + "\n");
                    counter++;
                }
            }
            

        }

        public static T FromByteArray<T>(byte[] data)
        {
            if (data == null)
                return default(T);
            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream(data))
            {
                object obj = bf.Deserialize(ms);
                return (T)obj;
            }
        }
        public static byte[] ToByteArray<T>(T obj)
        {
            if (obj == null)
                return null;
            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }

        public static List<Pose> LoadData( string folderPath)
        {
            List<Pose> poseslist = new List<Pose>();
            //String folderPath = "TRAINING_DATA3";
            foreach (string file in Directory.EnumerateFiles(folderPath, "*.TD"))
            {
                var readFile = File.ReadAllBytes(file);
                var skeletonList = ReadWriteObjectFile.FromByteArray<List<Skeleton>>(readFile);
                //Console.WriteLine("NRO: " + skeletonList.Count );
                foreach (Skeleton skel in skeletonList)
                {
                    var output = Regex.Replace(file.ToUpper(), @"[\d]*?\.TD", string.Empty).ToUpper().Split(new string[] { "\\" }, StringSplitOptions.None)[1];
                    //outputData.Add(DataTypeResolver<OutputDataType>.CastObject(output));
                    //inputData.Add(DataTypeResolver<InputDataType>.CastObject(skel));
                    //Console.WriteLine("FILE: " + output);
                    Pose posetemp = new Pose();
                    posetemp.InputData = skel;
                    posetemp.OutputData = output;
                    poseslist.Add(posetemp);
                }

            }
            return poseslist;
        }

        public static void printPoses(List<Pose> pl)
        {
            int counter = 1;
            foreach (Pose ps in pl)
            {
                Console.WriteLine("Nro Pose: " + counter + " ---------------------------------------------");
                foreach (Joint j in ps.InputData.Joints)
                { // all joins from a skeleton object
                    Console.WriteLine("X: " + j.Position.X + " ,Y: " + j.Position.Y + " ,Z: " + j.Position.Z + " Tipo: " + j.JointType);
                }
                Console.WriteLine("Pose salida: " + ps.OutputData);
                counter++;
            }
        }
    }
}
