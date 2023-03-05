﻿using Atlas.Atlas;
using Atlas.Atlas.Settings;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Atlas
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            Stopwatch timer = new Stopwatch();
            timer.Start();

            CommunicationManager communicationManager = new CommunicationManager();
            await communicationManager.getCommand();

            timer.Stop();
            String timeTaken = timer.Elapsed.ToString(@"m\:ss\.fff");
            Debug.WriteLine($"[*] (MAIN) Time taken: {timeTaken}");
        }
    }
}