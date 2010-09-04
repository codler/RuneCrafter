using System;
using System.Windows.Forms;
using System.Threading;

namespace RuneCrafter
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        
        static void Main(string[] args)
        {
            //Thread oThread = new Thread(new ThreadStart(Beta));

            // Start the thread
            //oThread.Start();
            /*
            Thread oThread2 = new Thread(new ThreadStart(Beta));

            // Start the thread
            oThread2.Start();
            */
            using (Game1 game = new Game1(args))
            {
                game.Run();
                //Application.Run(new Form1());
            }
            
        }

        //public static void Beta2()
        //{
        //    using (Game1 game = new Game1())
        //    {
        //        game.Run();
        //    }
        //}

        public static void Beta()
        {
            //Application.Run(new Form1());
        }
    }

}


