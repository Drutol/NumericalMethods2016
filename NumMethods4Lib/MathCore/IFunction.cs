namespace NumMethods4Lib.MathCore
{
    public interface IFunction
    {
        bool EnableWeight { set; }

        /// <summary>
        ///     Function text representation for GUI.
        /// </summary>
        string TextRepresentation { get; }

        /// <summary>
        ///     Gets function value for given argument.
        /// </summary>
        /// <param name="x" />
        /// <returns></returns>
        double GetValue(double x);
    }
}
