namespace NumMethods1.NumCore
{
    /// <summary>
    ///     Interface which is accepted by methods in MathCore class. And represents function.
    /// </summary>
    public interface IFunction
    {
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