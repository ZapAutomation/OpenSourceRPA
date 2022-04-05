using System;
using System.Collections.Generic;

/*
 * Taken from https://cmdline.codeplex.com/
 * 
 */

namespace ZappyLogger.Classes
{
    /// <summary>
    /// Provides a simple strongly typed interface to work with command line parameters.
    /// </summary>
    public class CmdLine
    {
        #region Fields

        // A private dictonary containing the parameters.
        private readonly Dictionary<string, CmdLineParameter> parameters = new Dictionary<string, CmdLineParameter>();

        #endregion

        #region cTor

        /// <summary>
        /// Creats a new empty command line object.
        /// </summary>
        public CmdLine()
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Returns a command line parameter by the name.
        /// </summary>
        /// <param name="name">The name of the parameter (the word after the initial hyphen (-).</param>
        /// <returns>A reference to the named comman line object.</returns>
        public CmdLineParameter this[string name]
        {
            get
            {
                if (!parameters.ContainsKey(name))
                {
                    throw new CmdLineException(name, "Not a registered parameter.");
                }
                return parameters[name];
            }
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Registers a parameter to be used and adds it to the help screen.
        /// </summary>
        /// <param name="p">The parameter to add.</param>
        public void RegisterParameter(CmdLineParameter parameter)
        {
            if (parameters.ContainsKey(parameter.Name))
            {
                throw new CmdLineException(parameter.Name, "Parameter is already registered.");
            }
            parameters.Add(parameter.Name, parameter);
        }

        /// <summary>
        /// Registers parameters to be used and adds hem to the help screen.
        /// </summary>
        /// <param name="p">The parameter to add.</param>
        public void RegisterParameter(CmdLineParameter[] parameters)
        {
            foreach (CmdLineParameter p in parameters)
            {
                RegisterParameter(p);
            }
        }


        /// <summary>
        /// Parses the command line and sets the value of each registered parmaters.
        /// </summary>
        /// <param name="args">The arguments array sent to main()</param>
        /// <returns>Any reminding strings after arguments has been processed.</returns>
        public string[] Parse(string[] args)
        {
            int i = 0;

            List<string> new_args = new List<string>();

            while (i < args.Length)
            {
                if (args[i].Length > 1 && args[i][0] == '-')
                {
                    // The current string is a parameter name
                    string key = args[i].Substring(1, args[i].Length - 1).ToLower();
                    string value = "";
                    i++;
                    if (i < args.Length)
                    {
                        if (args[i].Length > 0 && args[i][0] == '-')
                        {
                            // The next string is a new parameter, do not nothing
                        }
                        else
                        {
                            // The next string is a value, read the value and move forward
                            value = args[i];
                            i++;
                        }
                    }
                    if (!parameters.ContainsKey(key))
                    {
                        throw new CmdLineException(key, "Parameter is not allowed.");
                    }

                    if (parameters[key].Exists)
                    {
                        throw new CmdLineException(key, "Parameter is specified more than once.");
                    }

                    parameters[key].SetValue(value);
                }
                else
                {
                    new_args.Add(args[i]);
                    i++;
                }
            }


            // Check that required parameters are present in the command line. 
            foreach (string key in parameters.Keys)
            {
                if (parameters[key].Required && !parameters[key].Exists)
                {
                    throw new CmdLineException(key, "Required parameter is not found.");
                }
            }

            return new_args.ToArray();
        }

        /// <summary>
        /// Generates the help screen.
        /// </summary>
        public string HelpScreen()
        {
            int len = 0;
            foreach (string key in parameters.Keys)
            {
                len = Math.Max(len, key.Length);
            }

            string help = "\nParameters:\n\n";
            foreach (string key in parameters.Keys)
            {
                string s = "-" + parameters[key].Name;
                while (s.Length < len + 3)
                {
                    s += " ";
                }
                s += parameters[key].Help + "\n";
                help += s;
            }
            return help;
        }

        #endregion
    }
}