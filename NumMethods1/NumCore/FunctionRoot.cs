namespace NumMethods1.NumCore
{
    /// <summary>
    ///     This class is yielded as a result of MathCore methods.
    /// </summary>
    public class FunctionRoot
    {
        public string Group { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public int Iterated { get; set; }
        public string Method_Used { get; set; }
    }
}