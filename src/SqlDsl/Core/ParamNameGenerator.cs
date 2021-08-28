namespace SqlDsl.Core
{
    public struct ParamNameGenerator
    {
        private int _nextIndex;

        private static readonly string[] Names = {"@p1", "@p2", "@p3", "@p4", "@p5", "@p6", "@p7", "@p8", "@p9", "@p10", "@p11", "@p12", "@p13", "@p14", "@p15", "@p16", "@p17", "@p18", "@p19", "@p20", "@p21", "@p22", "@p23", "@p24", "@p25", "@p26", "@p27", "@p28", "@p29", "@p30", "@p31", "@p32"};

        public string Next()
        {
            if (_nextIndex < Names.Length)
            {
                string name = Names[_nextIndex];
                _nextIndex++;
                return name;
            }

            _nextIndex++;
            var index = _nextIndex.ToString();
            return string.Concat("@p", index);
        }
    }
}