namespace ZappyLogger.Entities
{
    public class CellContent
    {
        #region Fields

        #endregion

        #region cTor

        public CellContent(string value, int x)
        {
            this.Value = value;
            this.CellPosX = x;
        }

        #endregion

        #region Properties

        public string Value { get; set; }

        public int CellPosX { get; set; }

        #endregion
    }
}