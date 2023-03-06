using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Atlas
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            Debug.WriteLine("[+] (Main) Starting Atlas\n");
            Stopwatch timer = new Stopwatch();
            timer.Start();

            CommunicationManager communicationManager = new CommunicationManager();
            await communicationManager.getCommand();

            timer.Stop();
            String timeTaken = timer.Elapsed.ToString(@"m\:ss\.fff");
            Debug.WriteLine($"\n[*] (Main) Time taken: {timeTaken}");
        }
    }
}