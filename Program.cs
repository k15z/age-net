using System;
using System.IO;
using System.Drawing;

namespace agenet
{
   class MainClass
   {
      static Network network;

      public static void Main (string[] args)
      {
         if (args.Length == 0)
            trainNetwork ();
         else
            testNetwork (args[0]);
      }

      /* Testing */
      static void testNetwork(string saved) {
         Console.WriteLine ("Restoring network from file.");
         network = Utils.destringify(new StreamReader(saved));
         Console.WriteLine ("Ready!");
         while (true) {
            string file = Console.ReadLine ();
            float[] result = network.feed (imageReduce (file));
            Console.WriteLine (file + " > " + binaryOutput(result));
         }
      }

      static int binaryOutput(float[] output) {
         int age = 0;
         for (int i = 0; i < 7; i++)
            if (output[i] > 0.5f)
               age = (age | (1 << i));
         return age;
      }

      /* Training */
      static void trainNetwork() {
         network = new Network (new int[]{3996, 1331, 696, 7});

         Console.WriteLine ("Reading image data set.");
         int n = 0;
         int N = 70*50;
         float[][] input = new float[N][];
         float[][] output = new float[N][];
         for (int age = 16; age <= 85; age++) {
            string[] images = Directory.GetFiles ("img/" + age);
            for (int sample = 1; sample <= 50; sample++) {
               input[n] = imageReduce(images[sample]);
               output[n] = outputBinary(age);
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

      static float[] outputBinary(int age) {
         float[] f = new float[7];
         for (int i = 0; i < 7; i++)
            f [i] = (age & (1 << i));
         return f;
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
