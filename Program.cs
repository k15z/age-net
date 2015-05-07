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
         else if (args.Length == 1)
            testNetwork (args[0]);
         else if (args.Length == 2)
            testNetwork (args[0], args[1]);
      }

      /* Testing 1 */
      static void testNetwork(string saved) {
         network = Utils.destringify(new StreamReader(saved));
         float error = 0.0f;
         const int tests = 5;
         for (int age = 16; age <= 85; age++) {
            float ave = 0.0f;
            string[] images = Directory.GetFiles ("img/" + age);
            for (int i = 0; i < tests; i++) {
               int sample = (new Random()).Next(images.Length);
               float[] result = network.feed (imageReduce (images[sample]));
               float output = ((result[0]+1.5f)/3.0f)*100.0f;
               // Console.WriteLine (images[sample] + " > " + output);
               error += Math.Abs (output - age);
               ave += output;
            }
            Console.WriteLine (age + " -> " + ave/(float)tests);
         }
         Console.WriteLine ("Average Error: " + error/(70.0f*(float)tests));
      }

      /* Testing 2 */
      static void testNetwork(string saved, string file) {
         network = Utils.destringify(new StreamReader(saved));
         float[] result = network.feed (imageReduce (file));
         Console.WriteLine (file + " > " + ((result[0]+1.5f)/3.0f)*100.0f);
      }

            /* Training */
      static void trainNetwork() {
         if (File.Exists ("age-net.txt")) {
            network = Utils.destringify (new StreamReader ("age-net.txt"));
            Console.WriteLine ("Network restored.");
         } else {
            network = new Network (new int[]{ 3996, 1111, 999, 555, 1 });
            Console.WriteLine ("Network created.");
         }

         const int start = 5;
         const int end = 25;

         Console.WriteLine ("Reading image data set.");
         int n = 0;
         int N = 70*(end - start);
         float[][] input = new float[N][];
         float[][] output = new float[N][];
         for (int age = 16; age <= 85; age++) {
            string[] images = Directory.GetFiles ("img/" + age);
            for (int sample = start; sample < end; sample++) {
               input[n] = imageReduce(images[sample]);
               output[n] = new float[] { 3.0f*(((float)age)/(100.0f))-1.5f };
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
               data [cnt++] = bmp.GetPixel(x,y).GetBrightness()*4.0f - 2.0f;
            }
         return data;
      }
   }
}
