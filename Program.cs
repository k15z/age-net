using System;
using System.IO;
using System.Drawing;

namespace agenet
{
   class MainClass
   {
      static Network network;
      static float[][] table;

      public static void Main (string[] args)
      {
         trainNetwork ();
      }

      static void trainNetwork() {
         network = new Network (new int[]{3996, 999, 333, 100});

         Console.WriteLine ("Building output table.");
         table = new float[100][];
         for (int age = 10; age < 90; age++) {
            table[age] = new float[100];
            table [age] [age - 2] = 0.1f;
            table [age] [age - 1] = 0.3f;
            table [age] [age] = 1.0f;
            table [age] [age + 1] = 0.3f;
            table [age] [age + 2] = 0.1f;
         }

         Console.WriteLine ("Reading image data set.");
         int n = 0;
         int N = 70*75;
         float[][] input = new float[N][];
         float[][] output = new float[N][];
         for (int age = 16; age <= 85; age++) {
            string[] images = Directory.GetFiles ("img/" + age);
            for (int sample = 1; sample <= 50; sample++) {
               input[n] = imageReduce(images[sample]);
               output[n] = table[age];
               n++;
            }
         }

         Console.WriteLine ("Attempting to learn " + N + " models.");
         if (network.train (N, input, output))
         {
            Console.WriteLine ("Yay!");
            Utils.stringify (new StreamWriter ("agenet.txt"), network);
         } else
            Console.WriteLine ("Uh oh...");
      }

      static float[] imageReduce(string file) {
         Image img = Image.FromFile (file);
         Bitmap bmp = new Bitmap (img);
         int cnt = 0;
         float[] data = new float[3996];
         for (int x = 0; x < bmp.Width; x++)
            for (int y = 0; y < bmp.Height; y++) {
               data [cnt++] = bmp.GetPixel(x,y).GetBrightness();
            }
         return data;
      }

   }
}
