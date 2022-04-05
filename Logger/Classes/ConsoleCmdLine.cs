using System;

namespace ZappyLogger.Classes
{
    /// <summary>
    /// Represents a CmdLine object to use with console applications.
    /// The -help parameter will be registered automatically.
    /// Any errors will be written to the console instead of generating exceptions.
    /// </summary>
    public class ConsoleCmdLine : CmdLine
    {
        #region cTor

        public ConsoleCmdLine()
        {
            base.RegisterParameter(new CmdLineString("help", false, "Prints the help screen."));
        }

        #endregion

        #region Public methods

        public new string[] Parse(string[] args)
        {
            string[] ret = null;
            string error = "";
            try
            {
                ret = base.Parse(args);
            }
            catch (CmdLineException ex)
            {
                error = ex.Message;
            }

            if (this["help"].Exists)
            {
                //foreach(string s in base.HelpScreen().Split('\n'))
                //    Console.WriteLine(s);
                Console.WriteLine(base.HelpScreen());
                System.Environment.Exit(0);
            }

            if (error != "")
            {
                Console.WriteLine(error);
                Console.WriteLine("Use -help for more information.");
                System.Environment.Exit(1);
            }
            return ret;
        }

        #endregion
    }
}