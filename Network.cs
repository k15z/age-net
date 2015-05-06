using System;
using System.IO;

/**
 * author: Kevin Zhang
 * date: May 6th, 2015
 */
namespace agenet
{
   static class Utils
   {
      private static Random rng = new Random ();

      public static float random () {
         return (float)(rng.NextDouble ()) * 2.0f - 1.0f;
      }

      public static float sigmoid (float x)
      {
         return 1.0f / (1.0f + (float)Math.Exp (-x));
      }

      public static float dSigmoid (float x)
      {
         x = sigmoid (x);
         return x * (1.0f - x);
      }

      public static void stringify(StreamWriter o, Network n) {
         o.WriteLine(string.Join (" ", n.size));
         for (int layer = 0; layer < n.L - 1; layer++)
            for (int i = 0; i < n.size [layer]; i++)
               for (int j = 0; j < n.size [layer + 1]; j++)
                  o.Write(n.weight [layer] [i, j] + "\n");
         o.Close ();
      }

      public static Network destringify(StreamReader s) {
         int[] size = Array.ConvertAll(s.ReadLine().Split(' '), int.Parse);
         Network n = new Network (size);
         for (int layer = 0; layer < n.L - 1; layer++)
            for (int i = 0; i < n.size [layer]; i++)
               for (int j = 0; j < n.size [layer + 1]; j++)
                  n.weight [layer] [i, j] = float.Parse (s.ReadLine());
         s.Close ();
         return n;
      }
   }

   public class Network
   {
      const int MAX_ATTEMPT = 10000000;
      const int RESCORE_INTERVAL =   1;
      const float ERROR_MARGIN = 0.44f;
      const float LEARN_RATE =   0.01f;
      const float BACKUP_RATIO = 0.80f;

      public int L;
      public int[] size;
      public float[][] n_node;
      public float[][] a_node;
      public float[][] bkprop;
      public float[][,] weight;

      public Network (int[] design)
      {
         L = design.Length;
         size = design;

         n_node = new float[L][];
         a_node = new float[L][];
         bkprop = new float[L][];
         for (int l = 0; l < L; l++) {
            n_node [l] = new float[size [l]];
            a_node [l] = new float[size [l]];
            bkprop [l] = new float[size [l]];
         }

         weight = new float[L - 1][,];
         for (int l = 0; l < L - 1; l++) {
            weight [l] = new float[size [l], size [l + 1]];
            for (int j = 0; j < size [l]; j++)
               for (int k = 0; k < size [l + 1]; k++)
                  weight [l] [j, k] = Utils.random ();
         }
         
         return;
      }

      public float[] feed (float[] input)
      {
         a_node [0] = input;

         for (int l = 1; l < L; l++)
            for (int k = 0; k < size [l]; k++) {
               n_node [l] [k] = 0.0f;
               for (int j = 0; j < size [l - 1]; j++)
                  n_node [l] [k] += a_node [l - 1] [j] * weight [l - 1] [j, k];
               a_node [l] [k] = Utils.sigmoid (n_node [l] [k]);
            }

         return a_node [L - 1];
      }

      public bool train (int N, float[][] input, float[][] output)
      {
         int MAX_SCORE = N * size [L - 1];

         float error = 1.0f;
         int attempts = 0, score = 0;
         while (score < MAX_SCORE && attempts++ < MAX_ATTEMPT) {
            score = 0; error = 0.0f;
            for (int n = 0; n < N; n++) {
               var F = feed (input [n]);
               var T = output [n];

               if (attempts % RESCORE_INTERVAL == 0)
                  for (int k = 0; k < size [L - 1]; k++) {
                     if (Math.Abs (F [k] - T[k]) < ERROR_MARGIN)
                        score++;
                     error += Math.Abs (F [k] - T[k]);
                  }

               for (int i = 0; i < size [L - 1]; i++)
                  bkprop [L - 1] [i] = - (T [i] - F [i]) * Utils.dSigmoid (n_node [L - 1] [i]);
               for (int l = L - 2; l >= 0; l--) {
                  for (int j = 0; j < size [l]; j++) {
                     float value = 0.0f;
                     for (int i = 0; i < size [l + 1]; i++) {
                        value += bkprop [l + 1] [i] * weight [l] [j, i];
                        weight [l] [j, i] += LEARN_RATE * a_node [l] [j] * bkprop [l + 1] [i];
                     }
                     bkprop[l][j] = value * Utils.dSigmoid(n_node[l][j]);
                  }
               }
               if (n%100 == 0)
                  Console.Write(n + " ");
            }
            Console.WriteLine();

            if (attempts % RESCORE_INTERVAL == 0) {
               Console.Write ("Attempt: " + attempts + "/" + MAX_ATTEMPT + ", ");
               Console.Write ("Score: " + score + "/" + MAX_SCORE + ", ");
               Console.Write ("Error: " + error);
               Console.WriteLine ();
               if ((double)score / (double)MAX_SCORE > BACKUP_RATIO)
                  Utils.stringify(File.CreateText("temp.net"), this);
            }
         }

         return (score == MAX_SCORE);
      }
   }
}
