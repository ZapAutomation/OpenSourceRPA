namespace ZappyLogger.Classes
{
    /// <summary>
    /// Represents a command line parameter. 
    /// Parameters are words in the command line beginning with a hyphen (-).
    /// The value of the parameter is the next word in
    /// </summary>
    public class CmdLineParameter
    {
        #region Fields

        #endregion

        #region cTor

        /// <summary>
        /// Creates a new instance of this class.
        /// </summary>
        /// <param name="name">Name of parameter.</param>
        /// <param name="required">Require that the parameter is present in the command line.</param>
        /// <param name="helpMessage">The explanation of the parameter to add to the help screen.</param>
        public CmdLineParameter(string name, bool required, string helpMessage)
        {
            Name = name;
            Required = required;
            Help = helpMessage;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Returns the value of the parameter.
        /// </summary>
        public string Value { get; private set; } = "";

        /// <summary>
        /// Returns the help message associated with the parameter.
        /// </summary>
        public string Help { get; } = "";

        /// <summary>
        /// Returns true if the parameter was found in the command line.
        /// </summary>
        public bool Exists { get; private set; } = false;

        /// <summary>
        /// Returns true if the parameter is required in the command line.
        /// </summary>
        public bool Required { get; } = false;

        /// <summary>
        /// Returns the name of the parameter.
        /// </summary>
        public string Name { get; }

        #endregion

        #region Public methods

        /// <summary>
        /// Sets the value of the parameter.
        /// </summary>
        /// <param name="value">A string containing a integer expression.</param>
        public virtual void SetValue(string value)
        {
            Value = value;
            Exists = true;
        }

        #endregion
    }
}