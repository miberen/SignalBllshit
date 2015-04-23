using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;



namespace ConsoleApplication1
{

    class Program
    {
        
        public static List<string> FilterSignal(int size, List<double> original)
        {
            double[] orgArray = original.ToArray();
            List<String> newList = new List<string>(original.Count);
            string[] newArray = new string[original.Count];

            for (int i = size/2; i < (original.Count - 1) - (size/2); i++)
            {
                double value = 0;            
                for (int y = i - size/2; y < i + (size/2); y++)
                {
                    int o = i - size/2;
                    value += orgArray[y];
                }
                newArray[i] = Convert.ToString(value/size);
            }

            for (int i = 0; i < size/2; i++)
            {
                newArray[i] = newArray[size/2];
            }

            for (int i = original.Count-(size/2)-1; i < original.Count; i++)
            {

                newArray[i] = newArray[original.Count-(size/2)-2];
            }
            newList = newArray.ToList();

            return newList;
        }

        public static List<string> JoinTimNum(List<string> list1, List<string> list2)
        {
            List<string> done = new List<string>();

            for (int i = 0; i < list2.Count-1; i++)
            {
                done.Add(Convert.ToDouble(list1[i])/1000 + "\t" + list2[i]);              
            }

            return done;
        }

        public static List<string> JoinTimNumIBI(List<string> list1, List<string> list2, List<int> index)
        {
            List<string> done = new List<string>();
            int currentValue = Convert.ToInt32(list2[0]);
            int count = 0;

            for (int i = 0; i < list1.Count - 1; i++)
            {
                if (i < index[count])
                {
                    done.Add(Convert.ToDouble(list1[i]) / 1000 + "\t" + currentValue);
                }
                else if (count < list2.Count-1)
                {
                    count++;
                    currentValue = Convert.ToInt32(list2[count]);                    
                }
            }
            
            return done;
        }
        static void Main(string[] args)
        {
            System.Globalization.CultureInfo customCulture = (System.Globalization.CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
            customCulture.NumberFormat.NumberDecimalSeparator = ".";
            System.Threading.Thread.CurrentThread.CurrentCulture = customCulture;

            int blockSize = 50;
            int _thresh = 850;
            List<string> _IBI = new List<string>();
            int currentBeat = 0;
            int previousBeat = 0;
            List<int> IBItimings = new List<int>();
            List<int> IBImeasurements = new List<int>();
            List<int> IBIIndex = new List<int>();
            List<double> EDAnumbers = new List<double>();
            List<double> EDATimings = new List<double>();
            List<string> EDAnew = new List<string>();


            string[] lines = System.IO.File.ReadAllLines(@"C:\Users\AnonUser\Documents\SignalBllshit\data\dataKiril.txt");

            foreach (string line in lines)
            {
                if (line.Contains("H"))
                {
                    string line2 = line.Trim(new char[]{'H', ','});
                    string[] words = line2.Split('\t');
                    IBItimings.Add(Convert.ToInt32(words[0]));
                    IBImeasurements.Add(Convert.ToInt32(words[1]));
                    Array.Clear(words,0, words.Length);
                }
                if (line.Contains("E"))
                {
                    string line2 = line.Trim(new char[] { 'E', ',' });
                    string[] words = line2.Split('\t');
                    EDATimings.Add(Convert.ToDouble(words[0]));
                    EDAnumbers.Add(Convert.ToDouble(words[1]));
                    Array.Clear(words, 0, words.Length);
                }
            }

            for (int i = 0; i < IBImeasurements.Count - 1; i++)
            {
                if (IBImeasurements[i] >= _thresh && i > (currentBeat + 20) )
                {
                    IBIIndex.Add(i);
                    previousBeat = currentBeat;
                    currentBeat = i;
                    int timeSince = IBItimings[currentBeat] - IBItimings[previousBeat];
                    _IBI.Add(Convert.ToString(timeSince));
                }
            }
            EDAnew = FilterSignal(blockSize, EDAnumbers);

            File.WriteAllLines(@"C:\Users\AnonUser\Documents\SignalBllshit\data\dataIBI.txt", _IBI);
            File.WriteAllLines(@"C:\Users\AnonUser\Documents\SignalBllshit\data\dataIBITimNum.txt", JoinTimNumIBI(IBItimings.ConvertAll(delegate(int i) { return i.ToString(); }), _IBI, IBIIndex));
            File.WriteAllLines(@"C:\Users\AnonUser\Documents\SignalBllshit\data\dataEDAtimings.txt", EDATimings.Select(Convert.ToString));
            File.WriteAllLines(@"C:\Users\AnonUser\Documents\SignalBllshit\data\dataEDAfiltered.txt", EDAnew);
            File.WriteAllLines(@"C:\Users\AnonUser\Documents\SignalBllshit\data\dataEDATimNum.txt", JoinTimNum(new List<string>(EDATimings.Select(Convert.ToString)), EDAnew));


            // Keep the console window open in debug mode.
            Console.WriteLine("Press any key to exit.");
            System.Console.ReadKey();
        }      
    }
}
